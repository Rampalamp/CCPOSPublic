using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCPOS.Module.BusinessObjects.Inventory;
using CCPOS.Module.BusinessObjects.Staff;
using CCPOS.Module.Utils;
using CCPOS.Module.Win.WinForms;
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

namespace CCPOS.Module.Win.Controllers.InventoryControllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class StaffUseVC : ViewController
    {
        public StaffUseVC()
        {
            InitializeComponent();
            TargetObjectType = typeof(Product);
            // Target required Views (via the TargetXXX properties) and create their Actions.
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

        private void cmdCreateStaffLineItem_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (this.View.SelectedObjects.Count == 1)
            {
                IObjectSpace space = AppHelper.Application.CreateObjectSpace();
                
                StaffLineItem newItem = space.CreateObject<StaffLineItem>();

                newItem.Product = space.GetObject<Product>(this.View.CurrentObject as Product);

                ShowViewParameters svp = new ShowViewParameters(AppHelper.Application.CreateDetailView(space, newItem));
                svp.CreateAllControllers = true;
                svp.TargetWindow = TargetWindow.NewWindow;

                AppHelper.Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
            }
            else
            {
                XtraMessageBox.Show("Please select one product.");
            }
        }
    }
}
