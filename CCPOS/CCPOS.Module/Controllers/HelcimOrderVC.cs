using CCPOS.Module.BusinessObjects.Address;
using CCPOS.Module.BusinessObjects.Customer;
using CCPOS.Module.BusinessObjects.Inventory;
using CCPOS.Module.BusinessObjects.Sales;
using CCPOS.Module.Utils;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Xpo;
using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;
using System.Xml;

namespace CCPOS.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class HelcimOrderVC : ViewController
    {
        public HelcimOrderVC()
        {
            InitializeComponent();
            TargetObjectType = typeof(Sale);
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetViewType = ViewType.ListView;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            //helcim no longer in use, hide simple action
            this.Actions["cmdHelcimSync"].Active.SetItemValue("hide", false);
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

        private void cmdHelcimSync_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //XtraMessageBox.Show("Function not yet enabled.");
            if (XtraMessageBox.Show("Would you like to start a Helcim Orders Sync background process? You will be notified upon completion.","Proceed with Helcim Sync?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                HelcimHelper.StartHelcimSync(true, "newOrders");
            }
        }
    }
}
