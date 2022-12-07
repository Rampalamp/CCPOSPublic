using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using CCPOS.Module.BusinessObjects.Customer;
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

namespace CCPOS.Module.Win.Controllers.CustomerControllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class QueueVC : ViewController
    {
        Customer customer;
        public QueueVC()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetObjectType = typeof(Customer);
            TargetViewType = ViewType.DetailView;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            customer = View.CurrentObject as Customer;
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

        private void cmdStartQueue_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (customer != null)
            {
                if (!String.IsNullOrEmpty(customer.Email) || !String.IsNullOrEmpty(customer.PhoneNumber))
                {
                    if (!customer.InQueue)
                    {
                        if (customer.QueueStartTime != DateTime.MinValue)
                        {
                            //IN QUEUE WAS FALSE, CUSTOMER BEING PLACED INTO QUEUE AGAIN MOST LIKELY
                            if (XtraMessageBox.Show("Reset customer queue start time?", "Reset queue time?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                customer.QueueStartTime = DateTime.Now;
                                customer.InQueue = true;
                                View.ObjectSpace.CommitChanges();
                            }
                            else
                            {
                                //does not want to reset the queue and InQueue was false.
                                //do nothing?
                            }
                        }
                        else
                        {
                            //first queue of customer.
                            customer.QueueStartTime = DateTime.Now;
                            customer.InQueue = true;
                            View.ObjectSpace.CommitChanges();
                        }
                    }
                    else
                    {
                        //InQueue checked
                        if (XtraMessageBox.Show("Customer already in queue, reset queue start time?", "Reset queue time?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            customer.QueueStartTime = DateTime.Now;
                            View.ObjectSpace.CommitChanges();
                        }
                    }
                }
                else
                {
                    //no contact method found more or less
                    XtraMessageBox.Show("No email or phone number found on customer.");
                }
            }
        }

        private void cmdEndQueue_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (customer != null)
            {
                if (customer.InQueue)
                {
                    if (XtraMessageBox.Show("Remove customer from queue?", "Remove from queue?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        customer.InQueue = false;
                        View.ObjectSpace.CommitChanges();
                    }
                }
                else
                {
                    XtraMessageBox.Show("Customer not currently in queue.");
                }
            }
        }
    }
}
