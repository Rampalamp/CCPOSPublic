using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCPOS.Module.BusinessObjects.Customer;
using CCPOS.Module.BusinessObjects.Events;
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

namespace CCPOS.Module.Win.Controllers.EventControllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class NewCustomerVC : ViewController
    {
        public NewCustomerVC()
        {
            InitializeComponent();
            TargetObjectType = typeof(Reservation);
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

        private void cmdNewCustomer_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            (e.CurrentObject as Reservation).Customer = View.ObjectSpace.CreateObject<Customer>();
            View.ObjectSpace.CommitChanges();
            View.ObjectSpace.Refresh();
        }
    }
}
