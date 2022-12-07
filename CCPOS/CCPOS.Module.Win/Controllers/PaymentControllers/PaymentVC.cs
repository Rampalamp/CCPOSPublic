using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCPOS.Module.BusinessObjects.Sales;
using CCPOS.Module.BusinessObjects.Sales.Payment;
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

namespace CCPOS.Module.Win.Controllers.PaymentControllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class PaymentVC : ViewController
    {
        public PaymentVC()
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

        private void cmdPayBalance_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //create payment line item using remaining balance.
            Sale sale = this.View.CurrentObject as Sale;

            PaymentLineItem payment = this.View.ObjectSpace.CreateObject<PaymentLineItem>();
            payment.Amount = sale.Balance;
            
            sale.PaymentLineItems.Add(payment);

            this.View.ObjectSpace.CommitChanges();
            this.View.ObjectSpace.Refresh();
        }
    }
}
