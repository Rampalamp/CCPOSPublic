using CCPOS.Module.BusinessObjects.Sales;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.XtraEditors;

namespace CCPOS.Module.Win.Controllers.SaleControllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class PrintReceiptVC : ViewController
    {
        public PrintReceiptVC()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            //TargetObjectType = typeof(BusinessObjects.Sales.Sale);

            TargetViewId = "Receipt_ListView;Receipt_DetailView;Sale_ListView;Sale_DetailView";

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

        private void cmdPrintReceipt_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

            if (e.SelectedObjects.Count == 1)
            {
                //Singular
                if (e.CurrentObject is Sale)
                {
                    if ((e.CurrentObject as Sale).Receipt != null)
                    {
                        (e.CurrentObject as Sale).Receipt.PrintReceipt();
                    }
                }
                else
                {
                    //executed directly on receipt object.
                    (e.CurrentObject as Receipt).PrintReceipt();
                }
            }
            else if (e.SelectedObjects.Count > 1)
            {
                //Print off multiple receipts
                foreach (var item in e.SelectedObjects)
                {
                    if (item is Sale)
                    {
                        if ((item as Sale).Receipt != null)
                        {
                            (item as Sale).Receipt.PrintReceipt();
                        }
                    }
                    else
                    {
                        //executed directly on receipt object.
                        (item as Receipt).PrintReceipt();
                    }
                }
            }
        }

        private void cmdPrintFoodReceipt_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count == 1)
            {

                if (e.CurrentObject is Sale)
                {
                    (e.CurrentObject as Sale).PrintFoodReceipt();
                }
            }
            else
            {
                XtraMessageBox.Show("Please Select One Sale.");
            }
        }
    }
}
