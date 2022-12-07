using CCPOS.Module.BusinessObjects.Sales;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;

namespace CCPOS.Module.Win.Controllers.SaleControllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class CompleteTransactionVC : ViewController
    {
        public CompleteTransactionVC()
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

        private void cmdCompleteTransaction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if((e.CurrentObject as Sale).Receipt != null)
            {
                if (XtraMessageBox.Show("Complete and close Sale?", "Complete and close?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    //set DateComplete field, close window.
                    (e.CurrentObject as Sale).DateComplete = DateTime.Now;

                    View.ObjectSpace.CommitChanges();

                    View.Close();
                }
            }
            else
            {
                XtraMessageBox.Show("A Receipt must be created before completing a Sale.");
            }
           
            #region Old Complete Transaction Function - Now done in CreateReceiptVC
            //if (e.CurrentObject is Sale)
            //{
            //    Sale curSale = e.CurrentObject as Sale;

            //    if (curSale.Balance == 0)
            //    {
            //        if (curSale.Receipt != null)
            //        {
            //            if (XtraMessageBox.Show("A receipt has already been generated for this sale. Would you like to create a new one?", "Generate New Receipt?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //            {
            //                curSale.CreateReceipt(curSale, View.ObjectSpace);
            //            }
            //        }
            //        else
            //        {
            //            curSale.CreateReceipt(curSale, View.ObjectSpace);
            //        }
            //    }
            //    else
            //    {
            //        XtraMessageBox.Show("Balance must be zero to complete transaction.");
            //    }
            //}
            #endregion
        }

    }
}
