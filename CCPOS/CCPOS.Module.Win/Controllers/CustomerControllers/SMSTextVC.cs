using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCPOS.Module.BusinessObjects.Customer;
using CCPOS.Module.Utils;
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

namespace CCPOS.Module.Win.Controllers.CustomerControllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class SMSTextVC : ViewController
    {
        public SMSTextVC()
        {
            InitializeComponent();

            TargetObjectType = typeof(Customer);

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

        private void cmdSMSText_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            Customer cust = View.CurrentObject as Customer;

            EmailHelper.SendSMSToCustomer(cust, "WineBarOakville", "Order Is Ready");
        }
    }
}
