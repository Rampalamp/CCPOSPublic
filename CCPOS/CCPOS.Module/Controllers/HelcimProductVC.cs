using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CCPOS.Module.BusinessObjects.Inventory;
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
using DevExpress.XtraEditors;

namespace CCPOS.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class HelcimProductVC : ViewController
    {
        public HelcimProductVC()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetObjectType = typeof(Product);
            TargetViewType = ViewType.ListView;
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

        private SimpleAction cmdUpdateHelcimProducts;
        private System.ComponentModel.IContainer components;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cmdUpdateHelcimProducts = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.cmdUpdateHelcimInventory = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // cmdUpdateHelcimProducts
            // 
            this.cmdUpdateHelcimProducts.Caption = "Update Helcim Products";
            this.cmdUpdateHelcimProducts.Category = "View";
            this.cmdUpdateHelcimProducts.ConfirmationMessage = null;
            this.cmdUpdateHelcimProducts.Id = "cmdUpdateHelcimProducts";
            this.cmdUpdateHelcimProducts.ToolTip = null;
            this.cmdUpdateHelcimProducts.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.cmdUpdateHelcimProducts_Execute);
            // 
            // cmdUpdateHelcimInventory
            // 
            this.cmdUpdateHelcimInventory.Caption = "Update Helcim Inventory";
            this.cmdUpdateHelcimInventory.Category = "View";
            this.cmdUpdateHelcimInventory.ConfirmationMessage = null;
            this.cmdUpdateHelcimInventory.Id = "cmdUpdateHelcimInventory";
            this.cmdUpdateHelcimInventory.ToolTip = null;
            this.cmdUpdateHelcimInventory.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.cmdUpdateHelcimInventory_Execute);
            // 
            // HelcimProductVC
            // No need to add actions Helcim no longer in use.
            //this.Actions.Add(this.cmdUpdateHelcimProducts);
            //this.Actions.Add(this.cmdUpdateHelcimInventory);

        }

        private void cmdUpdateHelcimProducts_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //XtraMessageBox.Show("Function not yet enabled.");
            if (this.View.SelectedObjects.Count > 0)
            {
                if (XtraMessageBox.Show("Would you like to start a Helcim Create or Update Products background process on the selected products? You will be notified upon completion.", "Proceed with Helcim Update?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    //init list to send off
                    IList<Product> products = new List<Product>();

                    foreach (Product product in this.View.SelectedObjects)
                    {
                        products.Add(product);
                    }
                    //commented out until Helcim fixes their shit.
                    //HelcimHelper.StartHelcimSync(true, "updateProducts", products);
                    //HelcimHelper.UpdateHelcimInventory(products, this.View.ObjectSpace);
                }
            }
            else
            {
                XtraMessageBox.Show("Please select at least one product to Update/Create on Helcim.");
            }
        }

        private SimpleAction cmdUpdateHelcimInventory;

        private void cmdUpdateHelcimInventory_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (XtraMessageBox.Show("Would you like to start a Helcim Inventory Quantity Update background process? You will be notified upon completion.", "Proceed with Helcim Inventory Quantity Update?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //init list to send off
                IList<Product> products = this.View.ObjectSpace.GetObjects<Product>();

                HelcimHelper.StartHelcimSync(true, "resetProductStock", products);
                //HelcimHelper.UpdateHelcimInventory(products, this.View.ObjectSpace);
            }
        }
    }
}
