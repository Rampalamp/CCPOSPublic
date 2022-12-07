using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CCPOS.Module.BusinessObjects.Sales;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.XtraEditors;

namespace CCPOS.Module.Win.Controllers.SaleControllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class CreateReceiptVC : ViewController
    {
        public CreateReceiptVC()
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

        private void cmdCreateReceipt_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if((e.CurrentObject as Sale).Receipt != null)
            {
                if (XtraMessageBox.Show("A receipt has already been generated for this sale. Would you like to create a new one?", "Generate New Receipt?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    (e.CurrentObject as Sale).CreateReceipt((e.CurrentObject as Sale), View.ObjectSpace);
                }
            }
            else
            {
                (e.CurrentObject as Sale).CreateReceipt((e.CurrentObject as Sale), View.ObjectSpace);
            }   
        }
    }
}
