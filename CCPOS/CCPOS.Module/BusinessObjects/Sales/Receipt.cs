using CCPOS.Module.BusinessObjects.Sales.Payment;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;

namespace CCPOS.Module.BusinessObjects.Sales
{
    [DefaultProperty("File")]
    //[DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Receipt : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Receipt(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            DateCreated = DateTime.Now;
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            Sale?.Save();
        }
        //grab all required sale line items for receipt from linked sale object.
        private Sale _Sale;
        public Sale Sale
        {
            get { return _Sale; }
            set { SetPropertyValue<Sale>(nameof(Sale), ref _Sale, value); }
        }

        private FileData _File;
        public FileData File
        {
            get { return _File; }
            set { SetPropertyValue<FileData>(nameof(File), ref _File, value); }
        }


        private string _LocalPath;
        [Browsable(false)]
        public string LocalPath
        {
            get { return _LocalPath; }
            set { SetPropertyValue<string>(nameof(LocalPath), ref _LocalPath, value); }
        }

        private string _ReceiptText;
        [Size(SizeAttribute.Unlimited)]
        public string ReceiptText
        {
            get { return _ReceiptText; }
            set { SetPropertyValue<string>(nameof(ReceiptText), ref _ReceiptText, value); }
        }

        private DateTime _DateCreated;
        public DateTime DateCreated
        {
            get { return _DateCreated; }
            set { SetPropertyValue<DateTime>(nameof(DateCreated), ref _DateCreated, value); }
        }

        public void GenerateReceipt(IObjectSpace space)
        {
            if (Sale != null)
            {
                string localDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                string savePath = localDocs + @"\TheWineBar\Receipts\" + DateTime.Now.ToString("yyyy-dd-M");
                //create local dir if dont exist
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                //for certain line appending I need to reduce edge amount, depends on length of initial string.
                const int DETAILS_EDGE_PAD = 30;
                const int TOTALS_EDGE_PAD = 35;
                const int DISC_EDGE_PAD = 33;

                StringBuilder sb = new StringBuilder();
                //The string built looks nice when printed from receipt printer, but not so nice when viewed in .txt file. Very strange.
                //likely has to do with Font/sizing that is used in the printer settings, w/e in theory shouldnt ever need to look at
                //the receipt .txts
                //I append blank new lines as a splitter used in the actual print function
                #region ReceiptStringBuilder
                //If customer made comment on online sale or manual input I guess, add it above this line, should show below WineBar info details.
                if (!String.IsNullOrEmpty(Sale.CustomerComment))
                    sb.AppendLine("Customer Comment: " + Sale.CustomerComment);

                sb.AppendLine("*************************************************");
                foreach (SaleLineItem item in Sale.SaleLineItems)
                {
                    sb.AppendLine(item.Product?.Name);

                    string breakDown = item.Quantity > 0 ? item.Pricing + " " + item.Quantity.ToString("0.##") + "x " + item.SalePrice.ToString("0.00") : string.Empty;

                    sb.Append(String.Format("{0,-20}", breakDown));

                    sb.AppendLine(String.Format("{0," + DETAILS_EDGE_PAD + "}", item.Total.ToString("0.00")));

                    if (item.Discount > 0)
                    {
                        sb.Append(String.Format("DISCOUNT {0:D2}%", item.Discount));
                        sb.Append(String.Format("{0," + DISC_EDGE_PAD + "}", (-item.DiscountAmount).ToString("0.00")));
                        //sb.Append(string.Format("{0:0.00}", -item.DiscountAmount).PadLeft(DISC_EDGE_PAD));
                        sb.AppendLine();
                    }
                    sb.AppendLine();
                }
                sb.AppendLine("*************************************************");
                //total sale discount
                if (Sale.SaleLineItemsDiscountTotal > 0)
                {
                    //subtotal before discount - Add total discount, since SubTotal should already have the discount removed from it...
                    sb.Append(String.Format("{0, -15}", "SUBTOTAL"));
                    sb.Append(String.Format("{0," + TOTALS_EDGE_PAD + "}", (Sale.SubTotal + Sale.SaleLineItemsDiscountTotal).ToString("0.00")));
                    sb.AppendLine();

                    sb.Append(String.Format("{0, -15}", "DISCOUNT($)"));
                    sb.Append(String.Format("{0," + TOTALS_EDGE_PAD + "}", (-Sale.SaleLineItemsDiscountTotal).ToString("0.00")));
                    sb.AppendLine();
                }
                //request to show current payment line items on sale, mainly for partial cash payments, but think we should display em all
                foreach (PaymentLineItem item in Sale.PaymentLineItems)
                {
                    //need to test with printer that paddings are good.
                    sb.Append(String.Format("{0,-18}", "PAID - " + item.PaymentMethod));
                    sb.Append(String.Format("{0," + TOTALS_EDGE_PAD + "}", item.Amount.ToString("0.00")));
                    sb.AppendLine();
                }
                //subtotal after discount - Discount should already be calculated in subtotal for sale from lineItem totals.
                sb.Append(String.Format("{0,-15}", "SUBTOTAL"));
                sb.Append(String.Format("{0," + TOTALS_EDGE_PAD + "}", Sale.SubTotal.ToString("0.00")));
                sb.AppendLine();
                //tax amount
                sb.Append(String.Format("{0,-13}", "TAX(HST 13%)"));
                sb.Append(String.Format("{0," + TOTALS_EDGE_PAD + "}", Sale.Tax.ToString("0.00")));
                sb.AppendLine();
                //total - Using Balance due to account for partial payments before hand.
                sb.Append(String.Format("{0,-20}", "TOTAL"));
                sb.Append(String.Format("{0," + TOTALS_EDGE_PAD + "}", Sale.Balance.ToString("0.00")));
                sb.AppendLine();
                #endregion

                Sale.Receipt.ReceiptText = sb.ToString();
                //write receipt string to txt
                string fileName = String.Format("Sale#{0}_{1}.txt", Sale.SaleNumber.Number, DateTime.Now.ToString("hh-mm-ss-ffftt"));
                string finalSave = String.Format(savePath + @"\{0}", fileName);

                using (StreamWriter sw = new StreamWriter(finalSave, false))
                {
                    sw.Write(ReceiptText);
                }
                //check if exists/created properly
                if (System.IO.File.Exists(finalSave))
                {
                    //create new file attachment for receipt
                    File = space.CreateObject<FileData>();
                    using (Stream st = System.IO.File.OpenRead(finalSave))
                    {
                        File.LoadFromStream(fileName, st);
                    }
                }
            }
            else
            {
                XtraMessageBox.Show("No Sale Attached To Receipt Object.");
            }
        }

        public void PrintReceipt()
        {
            PrintDocument printDoc = new PrintDocument();

            printDoc.PrintPage += PrintDoc_PrintPage;

            printDoc.PrinterSettings.PrinterName = "POS-80C";

            printDoc.Print();
        }

        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            float WIDTH = 280f;
            float OFFSET = 0;
            //wine bar general info
            StringBuilder info = new StringBuilder();
            info.AppendLine("100 Bronte Road, Unit 09");
            info.AppendLine("Oakville, ON, L6L 6L5");
            info.AppendLine("1-905-469-8555");
            info.AppendLine("thewinebaroakville.com");
            info.AppendLine("info@thewinebaroakville.com");
            info.AppendLine("SALE #" + Sale.SaleNumber.Number);
            info.AppendLine(DateTime.Now.ToString());

            SizeF layoutArea = new SizeF(WIDTH, 0);
            #region PrintLogo
            PrintText printTextLogo = new PrintText("THE WINE BAR", new Font("Monospace", 20, FontStyle.Bold));
            printTextLogo.StringFormat.Alignment = StringAlignment.Center;
            SizeF stringSizeLogo = e.Graphics.MeasureString(printTextLogo.Text, printTextLogo.Font, layoutArea, printTextLogo.StringFormat);

            RectangleF rectf = new RectangleF(new PointF(), new SizeF(WIDTH, stringSizeLogo.Height));

            e.Graphics.DrawString(printTextLogo.Text, printTextLogo.Font, Brushes.Black, rectf, printTextLogo.StringFormat);
            //incrememnt OFFSET with last printed height
            OFFSET += stringSizeLogo.Height;
            #endregion

            #region PrintInfo
            PrintText printTextInfo = new PrintText(info.ToString(), new Font("Monospace", 10));
            printTextInfo.StringFormat.Alignment = StringAlignment.Center;
            SizeF stringSizeInfo = e.Graphics.MeasureString(printTextInfo.Text, printTextInfo.Font, layoutArea, printTextInfo.StringFormat);

            rectf = new RectangleF(new PointF(0, OFFSET), new SizeF(WIDTH, stringSizeInfo.Height));

            e.Graphics.DrawString(printTextInfo.Text, printTextInfo.Font, Brushes.Black, rectf, printTextInfo.StringFormat);
            //incrememnt OFFSET with last printed height
            OFFSET += stringSizeInfo.Height;
            #endregion

            #region PrintDetails
            PrintText printTextDetails = new PrintText(ReceiptText, new Font("Monospace", 10));
            SizeF stringSizeDetails = e.Graphics.MeasureString(printTextDetails.Text, printTextDetails.Font, layoutArea, printTextDetails.StringFormat);
            //set new rectf Y starting point to current offset
            rectf = new RectangleF(new PointF(0, OFFSET), new SizeF(WIDTH, stringSizeDetails.Height));

            e.Graphics.DrawString(printTextDetails.Text, printTextDetails.Font, Brushes.Black, rectf, printTextDetails.StringFormat);
            #endregion
        }
    }
    //printing helper class
    public class PrintText
    {
        public PrintText(string text, Font font) : this(text, font, new StringFormat()) { }

        public PrintText(string text, Font font, StringFormat stringFormat)
        {
            Text = text;
            Font = font;
            StringFormat = stringFormat;
        }

        public string Text { get; set; }

        public Font Font { get; set; }

        /// <summary> Default is horizontal string formatting </summary>
        public StringFormat StringFormat { get; set; }
    }
}