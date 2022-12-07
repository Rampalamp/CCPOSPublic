using CCPOS.Module.BusinessObjects.Customer;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using System;
using System.Collections.Generic;

namespace CCPOS.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class MyFilterControllerVC : ViewController
    {
        FilterController standardFilterController = null;

        public MyFilterControllerVC()
        {
            InitializeComponent();
            this.TargetObjectType = typeof(Customer);

        }
        protected override void OnActivated()
        {
            base.OnActivated();
            standardFilterController = Frame.GetController<FilterController>();
            if (standardFilterController != null)
            {
                standardFilterController.CustomGetFullTextSearchProperties += new EventHandler<CustomGetFullTextSearchPropertiesEventArgs>(standardFilterController_CustomGetFullTextSearchProperties);
            }

        }

        protected override void OnDeactivated()
        {
            if (standardFilterController != null)
            {
                standardFilterController.CustomGetFullTextSearchProperties -= standardFilterController_CustomGetFullTextSearchProperties;
                standardFilterController = null;
            }
            base.OnDeactivated();
        }

        void standardFilterController_CustomGetFullTextSearchProperties(object sender, CustomGetFullTextSearchPropertiesEventArgs e)
        {
            foreach (string property in GetFullTextSearchProperties())
            {
                e.Properties.Add(property);
            }
            e.Handled = true;
        }

        private List<string> GetFullTextSearchProperties()
        {
            List<string> searchProperties = new List<string>();

            searchProperties.Add("FullName");
            searchProperties.Add("PhoneNumber");
            searchProperties.Add("Email");
            searchProperties.Add("Address");

            // searchProperties.Add("LastName");
            return searchProperties;
        }

    }
}
