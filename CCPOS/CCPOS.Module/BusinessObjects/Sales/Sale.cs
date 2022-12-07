using CCPOS.Module.BusinessObjects.Sales.Payment;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CCPOS.Module.BusinessObjects.Sales
{
    //[DefaultClassOptions]
    //**Using Notes as default property so they can use as a Table tag more or less.
    [DefaultProperty("Notes")]
    [VisibleInReports(true)]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Sale : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Sale(Session session)
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
            fullName = ObjectFormatter.Format("{Notes}-{Customer}-{SaleNumber}-{Total}", this, EmptyEntriesMode.RemoveDelimiterWhenEntryIsEmpty);

            if (SaleNumber == null)
                SaleNumber = new SaleNumber(Session);

            //also gets called on Discount(immediate post data) setter for if discount is altered on total bill.
            CalculateSale();

            base.OnSaving();

        }

        /// <summary>
        /// Calculate Sale With Sale.Discount
        /// </summary>
        /// <param name="Discount">Intended to be Sale Object Discount value</param>
        public void CalculateSale()
        {
            SaleLineItemsDiscountTotal = SaleLineItems.Sum(x => x.DiscountAmount);

            SubTotal = SaleLineItems.Sum(x => x.Total);

            Tax = SubTotal * 0.13m;

            Total = SubTotal + Tax;
            //round Total for odd calculations from Tax and subtotal, since its decimal Balance that would have rounded to 0.00 was not being seen as exactly 0.
            Total = Math.Round((SubTotal + Tax), 2, MidpointRounding.AwayFromZero);

            //amount received/change stuff, exclude any Void payment types
            AmountReceived = PaymentLineItems.Where(x => x.Type != PaymentType.Void).Sum(x => x.Amount);
            //calc balance and change
            CalculateBalanceAndChange(this);
        }

        /// <summary>
        /// Calculate Sale while excluding a to be deleted SaleLineItem
        /// </summary>
        /// <param name="excludeItem">Excluded Sale Line Item</param>
        public void CalculateSale(SaleLineItem excludeItem)
        {
            SaleLineItemsDiscountTotal = SaleLineItems.Where(x => x != excludeItem).Sum(x => x.DiscountAmount);

            SubTotal = SaleLineItems.Where(x => x != excludeItem).Sum(x => x.Total);

            Tax = SubTotal * 0.13m;

            Total = SubTotal + Tax;
            //round Total for odd calculations from Tax and subtotal, since its decimal Balance that would have rounded to 0.00 was not being seen as exactly 0.
            Total = Math.Round((SubTotal + Tax), 2, MidpointRounding.AwayFromZero);

            //amount received/change stuff, exclude any Void payment types
            AmountReceived = PaymentLineItems.Where(x => x.Type != PaymentType.Void).Sum(x => x.Amount);
            //calc balance and change
            CalculateBalanceAndChange(this);
        }

        /// <summary>
        /// Calculate Sale while excluding a to be deleted PaymentLineItem
        /// </summary>
        /// <param name="excludeItem">Excluded Payment Line Item</param>
        public void CalculateSale(PaymentLineItem excludeItem)
        {
            SaleLineItemsDiscountTotal = SaleLineItems.Sum(x => x.DiscountAmount);

            SubTotal = SaleLineItems.Sum(x => x.Total);

            Tax = SubTotal * 0.13m;

            Total = SubTotal + Tax;
            //round Total for odd calculations from Tax and subtotal, since its decimal Balance that would have rounded to 0.00 was not being seen as exactly 0.
            Total = Math.Round((SubTotal + Tax), 2, MidpointRounding.AwayFromZero);

            //amount received/change stuff, exclude any Void payment types
            AmountReceived = PaymentLineItems.Where(x => x.Type != PaymentType.Void && x != excludeItem).Sum(x => x.Amount);
            //calc balance and change
            CalculateBalanceAndChange(this);
        }

        /// <summary>
        /// Calculates Balance and Change of sale based on Amount Received.
        /// </summary>
        /// <param name="sale">Current sale being processed</param>
        private void CalculateBalanceAndChange(Sale sale)
        {
            sale.Balance = sale.Total - sale.AmountReceived;
            //check to see if Change is due
            if (sale.Balance < 0)
            {
                //If entering here Customer probably paid with cash, otherwise an over amount was charged to credit or debit
                //round and set to string to account for rounding of pennys on Change value
                string rawChange = Math.Round(sale.Balance, 2, MidpointRounding.AwayFromZero).ToString();
                //get last digit of decimal
                char pennyValue = rawChange.Last();
                //determine penny value
                if (pennyValue == '3' || pennyValue == '4')
                {
                    pennyValue = '5';
                }
                else if (pennyValue == '1' || pennyValue == '2')
                {
                    pennyValue = '0';
                }
                rawChange = rawChange.Substring(0, rawChange.Length - 1) + pennyValue;
                //set Change to Balance value and set Balance to 0
                sale.Change = Convert.ToDecimal(rawChange);
                sale.Balance = 0;
            }
            else
            {
                sale.Change = 0;
            }
        }

        /// <summary>
        /// Creates a new receipt object/receipt text for current sale.
        /// </summary>
        /// <param name="curSale">Sale to create new receipt on</param>
        /// <param name="space">Space to use fresh objects</param>
        public void CreateReceipt(Sale curSale, IObjectSpace space)
        {
            Receipt newReceipt = space.CreateObject<Receipt>();
            //ran into disposed object issues on curSale when running this from other VCs. Regrab current sale.
            Sale sale = space.GetObject(curSale);
            newReceipt.Sale = sale;
            sale.Receipt = newReceipt;
            //commit once before to set all values for sale and receipt.
            space.CommitChanges();
            //generate receipt document
            newReceipt.GenerateReceipt(space);
            //commit once more with receipt created
            space.CommitChanges();
            //Refresh to show new receipt object on Sale DetailView.
            space.Refresh();

            if (XtraMessageBox.Show("Would you like to print the receipt?", "Print Receipt?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                newReceipt.PrintReceipt();
            }
            else
            {
                //do nothing?
            }
        }
        //NEED SIZE UNLIMITED ABOVE PERSISNT FULL NAME TO HAVE THE DATA TYPE IN SQL CORRECT......... LULOLULZ
        [Size(SizeAttribute.Unlimited)]
        [Persistent("FullName")]
        private string fullName;
        [PersistentAlias("fullName")]
        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string FullName
        {
            get
            {
                return fullName;
            }
        }

        private SaleNumber _SaleNumber;
        [DevExpress.Xpo.Aggregated]
        public SaleNumber SaleNumber
        {
            get { return _SaleNumber; }
            set { SetPropertyValue<SaleNumber>(nameof(SaleNumber), ref _SaleNumber, value); }
        }

        #region Helcim Fields
        private string _HelcimOrderNumber;
        [Browsable(false)]
        public string HelcimOrderNumber
        {
            get { return _HelcimOrderNumber; }
            set { SetPropertyValue<string>(nameof(HelcimOrderNumber), ref _HelcimOrderNumber, value); }
        }

        private string _HelcimToken;
        [Browsable(false)]
        public string HelcimToken
        {
            get { return _HelcimToken; }
            set { SetPropertyValue<string>(nameof(HelcimToken), ref _HelcimToken, value); }
        }

        private string _HelcimOrderURL;
        public string HelcimOrderURL
        {
            get { return _HelcimOrderURL; }
            set { SetPropertyValue<string>(nameof(HelcimOrderURL), ref _HelcimOrderURL, value); }
        }

        private DateTime _HelcimDateCreated;
        public DateTime HelcimDateCreated
        {
            get { return _HelcimDateCreated; }
            set { SetPropertyValue<DateTime>(nameof(HelcimDateCreated), ref _HelcimDateCreated, value); }
        }
        #endregion

        private Customer.Customer _Customer;
        [Association]
        public Customer.Customer Customer
        {
            get { return _Customer; }
            set { SetPropertyValue<Customer.Customer>(nameof(Customer), ref _Customer, value); }
        }

        private DateTime _DateCreated;
        public DateTime DateCreated
        {
            get { return _DateCreated; }
            set { SetPropertyValue<DateTime>(nameof(DateCreated), ref _DateCreated, value); }
        }

        private Receipt _Receipt;
        public Receipt Receipt
        {
            get { return _Receipt; }
            set { SetPropertyValue<Receipt>(nameof(Receipt), ref _Receipt, value); }
        }

        private decimal _SubTotal;
        [ModelDefault("EditMask", "C")]
        public decimal SubTotal
        {
            get { return _SubTotal; }
            set { SetPropertyValue<decimal>(nameof(SubTotal), ref _SubTotal, value); }
        }

        private decimal _Total;
        [ModelDefault("EditMask", "C")]
        public decimal Total
        {
            get { return _Total; }
            set { SetPropertyValue<decimal>(nameof(Total), ref _Total, value); }
        }

        private DiscountType _DiscountType;
        [ImmediatePostData]
        public DiscountType DiscountType
        {
            get { return _DiscountType; }
            set
            {
                if (!IsLoading)
                {
                    if (value != null)
                    {
                        //Do work - likely set all SaleLineItems discount value. Unsure how andrew wants this set up.
                        foreach (SaleLineItem item in SaleLineItems)
                        {
                            if (item.Product != null && item.Product.ProductType != null)
                            {
                                //change of Discount should trigger to repopulate the salelineitem totals, dont think I need to call the method manually.
                                if (item.Product.ProductType.Name.Contains("Food"))
                                {
                                    item.Discount = value.FoodDiscount;
                                }
                                else if (item.Product.ProductType.Name.Contains("Wine"))
                                {
                                    item.Discount = value.WineDiscount;
                                }
                            }
                        }
                    }
                    else
                    {
                        //Set all back to 0 when Value is set back to null
                        foreach (SaleLineItem item in SaleLineItems)
                            item.Discount = 0;
                    }

                }
                SetPropertyValue<DiscountType>(nameof(DiscountType), ref _DiscountType, value);
            }
        }

        private decimal _SaleLineItemsDiscountTotal;
        [ModelDefault("EditMask", "C")]
        [ModelDefault("AllowEdit", "False")]
        [ModelDefault("Caption", "Discount($)")]
        public decimal SaleLineItemsDiscountTotal
        {
            get { return _SaleLineItemsDiscountTotal; }
            set { SetPropertyValue<decimal>(nameof(SaleLineItemsDiscountTotal), ref _SaleLineItemsDiscountTotal, value); }
        }

        private decimal _Tax;
        [ModelDefault("EditMask", "C")]
        [ModelDefault("Caption", "Tax(13%)")]
        public decimal Tax
        {
            get { return _Tax; }
            set { SetPropertyValue<decimal>(nameof(Tax), ref _Tax, value); }
        }

        private decimal _AmountReceived;
        [ModelDefault("EditMask", "C")]
        public decimal AmountReceived
        {
            get { return _AmountReceived; }
            set { SetPropertyValue<decimal>(nameof(AmountReceived), ref _AmountReceived, value); }
        }

        private decimal _Change;
        [ModelDefault("EditMask", "C")]
        public decimal Change
        {
            get { return _Change; }
            set { SetPropertyValue<decimal>(nameof(Change), ref _Change, value); }
        }

        private decimal _Balance;
        [ModelDefault("Caption", "Balance Due")]
        [ModelDefault("EditMask", "C")]
        public decimal Balance
        {
            get { return _Balance; }
            set { SetPropertyValue<decimal>(nameof(Balance), ref _Balance, value); }
        }

        private string _CustomerComment;
        [Size(SizeAttribute.Unlimited)]
        public string CustomerComment
        {
            get { return _CustomerComment; }
            set { SetPropertyValue<string>(nameof(CustomerComment), ref _CustomerComment, value); }
        }

        private string _Notes;
        [Size(SizeAttribute.Unlimited)]
        public string Notes
        {
            get { return _Notes; }
            set { SetPropertyValue<string>(nameof(Notes), ref _Notes, value); }
        }

        private bool _InvoiceEmailed;
        public bool InvoiceEmailed
        {
            get { return _InvoiceEmailed; }
            set { SetPropertyValue<bool>(nameof(InvoiceEmailed), ref _InvoiceEmailed, value); }
        }

        private bool _NewOnlineSale;
        public bool NewOnlineSale
        {
            get { return _NewOnlineSale; }
            set { SetPropertyValue<bool>(nameof(NewOnlineSale), ref _NewOnlineSale, value); }
        }

        private bool _OnlineSale;
        [ModelDefault("AllowEdit", "False")]
        public bool OnlineSale
        {
            get { return _OnlineSale; }
            set { SetPropertyValue<bool>(nameof(OnlineSale), ref _OnlineSale, value); }
        }

        private bool _ReadyForPickUp;
        public bool ReadyForPickUp
        {
            get { return _ReadyForPickUp; }
            set { SetPropertyValue<bool>(nameof(ReadyForPickUp), ref _ReadyForPickUp, value); }
        }

        private DateTime _DateComplete;
        public DateTime DateComplete
        {
            get { return _DateComplete; }
            set { SetPropertyValue<DateTime>(nameof(DateComplete), ref _DateComplete, value); }
        }

        [Association]
        public XPCollection<PaymentLineItem> PaymentLineItems
        {
            get { return GetCollection<PaymentLineItem>(nameof(PaymentLineItems)); }
        }

        //Unsure about how to use Orders/PurhcaseOrders, may just keep at as Sales / Saleline items. Semi business plan change due to COVID-19
        [Association]
        public XPCollection<SaleLineItem> SaleLineItems
        {
            get { return GetCollection<SaleLineItem>(nameof(SaleLineItems)); }
        }

        #region FoodPrintingFunctions
        public void GenerateFoodReceipt(StringBuilder sb)
        {
            const int DETAILS_EDGE_PAD = 30;
            const int TOTALS_EDGE_PAD = 35;
            const int DISC_EDGE_PAD = 33;

            //add food line items
            sb.AppendLine("*************************************************");
            foreach (SaleLineItem item in SaleLineItems)
            {
                //skip if its not a Food product type
                if (item.Product.ProductType != null)
                    if (!item.Product.ProductType.Name.Contains("Food"))
                        continue;

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
        }

        public void PrintFoodReceipt()
        {
            PrintDocument printFoodDoc = new PrintDocument();

            printFoodDoc.PrintPage += PrintFoodDoc_PrintPage;

            printFoodDoc.PrinterSettings.PrinterName = "POS-80C";

            printFoodDoc.Print();
        }

        private void PrintFoodDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            float WIDTH = 280f;
            float OFFSET = 0;
            SizeF layoutArea = new SizeF(WIDTH, 0);
            StringBuilder header = new StringBuilder();
            StringBuilder food = new StringBuilder();
            StringBuilder time = new StringBuilder();

            GenerateFoodReceipt(food);

            time.AppendLine("Printed: " + DateTime.Now);
            //add time stamp
            PrintText printTextTime = new PrintText(time.ToString(), new Font("Monospace", 11));
            SizeF stringSizeTime = e.Graphics.MeasureString(printTextTime.Text, printTextTime.Font, layoutArea, printTextTime.StringFormat);
            RectangleF rectf = new RectangleF(new PointF(0, OFFSET), new SizeF(WIDTH, stringSizeTime.Height));
            e.Graphics.DrawString(printTextTime.Text, printTextTime.Font, Brushes.Black, rectf, printTextTime.StringFormat);

            OFFSET += stringSizeTime.Height;

            //add notes
            if (!String.IsNullOrEmpty(Notes))
            {
                header.AppendLine("Notes: " + Notes);
                PrintText printTextHeader = new PrintText(header.ToString(), new Font("Monospace", 13, FontStyle.Bold));
                SizeF stringSizeHeader = e.Graphics.MeasureString(printTextHeader.Text, printTextHeader.Font, layoutArea, printTextHeader.StringFormat);
                rectf = new RectangleF(new PointF(0, OFFSET), new SizeF(WIDTH, stringSizeHeader.Height));
                e.Graphics.DrawString(printTextHeader.Text, printTextHeader.Font, Brushes.Black, rectf, printTextHeader.StringFormat);

                OFFSET += stringSizeHeader.Height;
            }
                
            PrintText printTextDetails = new PrintText(food.ToString(), new Font("Monospace", 10));
            SizeF stringSizeDetails = e.Graphics.MeasureString(printTextDetails.Text, printTextDetails.Font, layoutArea, printTextDetails.StringFormat);
            //set new rectf Y starting point to current offset
            rectf = new RectangleF(new PointF(0, OFFSET), new SizeF(WIDTH, stringSizeDetails.Height));

            e.Graphics.DrawString(printTextDetails.Text, printTextDetails.Font, Brushes.Black, rectf, printTextDetails.StringFormat);

        }
        #endregion

        #region NoLongerNeeded/Unused
        //private int _Discount;
        //[ModelDefault("DisplayFormat", "{0:N0}%")]
        //[ModelDefault("EditMask", "N0")]
        //[ImmediatePostData]
        //public int Discount
        //{
        //    get { return _Discount; }
        //    set
        //    {

        //        if (!IsLoading)
        //        {
        //            //called in onSave();
        //            CalculateSale(value);
        //        }

        //        SetPropertyValue<int>(nameof(Discount), ref _Discount, value);
        //    }
        //}

        //private decimal _DiscountAmount;
        //[ModelDefault("EditMask", "C")]
        //[ModelDefault("Caption", "Discount($)")]
        //public decimal DiscountAmount
        //{
        //    get { return _DiscountAmount; }
        //    set { SetPropertyValue<decimal>(nameof(DiscountAmount), ref _DiscountAmount, value); }
        //}
        #endregion

    }
}