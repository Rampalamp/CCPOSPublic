using CCPOS.Module.BusinessObjects.Sales;
using CCPOS.Module.Utils;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;

namespace CCPOS.Module.Win.Controllers.SaleControllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class CustomerInvoiceVC : ViewController
    {
        public CustomerInvoiceVC()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetObjectType = typeof(Sale);
            TargetViewType = ViewType.DetailView;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void cmdEmailCustomerInvoice_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace((e.CurrentObject as Sale).Customer?.Email))
            {
                if (XtraMessageBox.Show(String.Format("Send invoice to {0}?", (e.CurrentObject as Sale).Customer?.Email), "Email Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    //unsure of speed, may need to use background workers
                    //BackgroundWorker bw = new BackgroundWorker();
                    //bw.DoWork += Bw_DoWork;
                    //bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
                    //bw.RunWorkerAsync(e.CurrentObject);
                    if (EmailHelper.SendInvoiceEmail(e.CurrentObject as Sale))
                    {
                        (e.CurrentObject as Sale).InvoiceEmailed = true;

                        XtraMessageBox.Show("Invoice Sent");
                    }
                    else
                    {
                        (e.CurrentObject as Sale).InvoiceEmailed = false;
                    }
                    this.View.ObjectSpace.CommitChanges();
                }
            }
            else
            {
                XtraMessageBox.Show("No customer associated with sale.");
            }
        }

        private void cmdCreateInvoice_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            string createdInvoice = EmailHelper.CreateInvoiceAttachmentFromSale(e.CurrentObject as Sale);

            if (XtraMessageBox.Show("Would you like view the created invoice?", "Invoice Created", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(createdInvoice);
            }
        }

        //private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    this.View.ObjectSpace.CommitChanges();
        //    //throw new NotImplementedException();
        //}

        //private void Bw_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    (AppHelper.Application.MainWindow.Template as Form).Invoke(new Action(() =>
        //    {
        //        if (EmailHelper.SendInvoiceEmail(e.Argument as Sale))
        //        {
        //            (e.Argument as Sale).InvoiceEmailed = true;
        //        }
        //        else
        //        {
        //            (e.Argument as Sale).InvoiceEmailed = false;
        //        }
        //    }));
        //    //throw new NotImplementedException();
        //}
    }
}
