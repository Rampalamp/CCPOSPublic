using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using CCPOS.Module.BusinessObjects.Sales;
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

namespace CCPOS.Module.Win.Controllers.SaleControllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class SplitSaleVC : ViewController
    {
        public SplitSaleVC()
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

        private void cmdSplitSale_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //grab list of current sale line items, pop up in a separate window to select which ones to make in a new sale.
            //find by Item ID on Sale_DetailView. Cast to list prop editor and grab listview to get selected objects.
            DevExpress.ExpressApp.ListView saleList = ((View as DetailView).FindItem("SaleLineItems") as ListPropertyEditor).ListView;

            if (saleList.SelectedObjects.Count > 0)
            {
                if (XtraMessageBox.Show("Move selected SaleLineItems to new Sale?", "Split Sale?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    //save current changes
                    View.ObjectSpace.CommitChanges();
                    //remove each selected and append to new sale.
                    Sale curSale = (View.CurrentObject as Sale);
                    Sale newSale = View.ObjectSpace.CreateObject<Sale>();

                    foreach (SaleLineItem item in saleList.SelectedObjects)
                    {
                        curSale.SaleLineItems.Remove(item);
                        newSale.SaleLineItems.Add(item);
                    }

                    curSale.CalculateSale();

                    View.ObjectSpace.CommitChanges();
                    View.ObjectSpace.Refresh();

                    if (curSale.Receipt != null)
                    {
                        if (XtraMessageBox.Show("Current Sale has a Receipt attached to it. Would you like to create a new Receipt for Sale?", "Generate Receipt?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            curSale.CreateReceipt(curSale, View.ObjectSpace);
                        }
                    }

                    AppHelper.CreateNewWindowDetailView(newSale);
                }
            }
            else
            {
                //**LOOKS LIKE ONE ITEM IS DEFAULT SELECTED AND THIS LIKELY WILL NEVER EXECUTE... Ohwellz.
                //create pop up view with each line item to prompt a selection.
                List<CriteriaOperator> crit = new List<CriteriaOperator>();

                foreach (SaleLineItem item in saleList.SelectedObjects)
                {
                    crit.Add(new BinaryOperator("Oid", item.Oid));
                }

                using (CollectionSource cs = new CollectionSource(View.ObjectSpace, typeof(SaleLineItem)))
                {

                    DevExpress.ExpressApp.ListView view = Application.CreateListView("SaleLineItem_ListView", cs, false);

                    DialogController dc = new DialogController();
                    dc.AcceptAction.Execute += AcceptAction_Execute;
                    dc.CancelAction.Execute += CancelAction_Execute;
                    ShowViewParameters svp = new ShowViewParameters(view);
                    svp.TargetWindow = TargetWindow.NewModalWindow;
                    //svp.Context = TemplateContext.PopupWindow;
                    svp.Controllers.Add(dc);

                    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(this.Frame, null));
                }
            }
        }

        private void CancelAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //do nothing?
        }

        private void AcceptAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (e.SelectedObjects.Count > 0)
            {
                //make new sale, and swap products.
                Sale newSale = View.ObjectSpace.CreateObject<Sale>();

                foreach (SaleLineItem item in e.SelectedObjects)
                {
                    (View.CurrentObject as Sale).SaleLineItems.Remove(item);
                    newSale.SaleLineItems.Add(item);
                }

                View.ObjectSpace.CommitChanges();
                View.ObjectSpace.Refresh();

                AppHelper.CreateNewWindowDetailView(newSale);

            }
            else
            {
                XtraMessageBox.Show("Please select at least one item to split.");
            }
        }

        private void cmdSplitSaleEven_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            SplitSaleForm form = new SplitSaleForm();

            form.ShowDialog();

            if (form.splitSale)
            {
                if(form.Number > 0)
                {
                    //split sale button hit
                    //force a commit
                    View.ObjectSpace.CommitChanges();
                    Sale originalSale = (View.CurrentObject as Sale);
                    //adjust saleLineItems quantity first on original sale
                    foreach (SaleLineItem item in originalSale.SaleLineItems)
                    {
                        item.Quantity = item.Quantity / form.Number;
                    }
                    View.ObjectSpace.CommitChanges();
                    //reduce Number by 1 for the sale
                    for (int i = 0; i < form.Number - 1; i++)
                    {
                        //make new sale, and add sale line items.
                        Sale newSale = View.ObjectSpace.CreateObject<Sale>();

                        foreach (SaleLineItem item in originalSale.SaleLineItems)
                        {
                            SaleLineItem newItem = View.ObjectSpace.CreateObject<SaleLineItem>();
                            //need to refetch item.Product each time because it becomes seen as disposed otherwise.
                            newItem.Product = View.ObjectSpace.GetObject(item.Product);
                            newItem.Quantity = item.Quantity;
                            newItem.Pricing = item.Pricing;
                            //dunno if they will want discount copied over, putting it in for now
                            newItem.Discount = item.Discount;
                            
                            newSale.SaleLineItems.Add(newItem);
                        }

                        newSale.CalculateSale();

                        View.ObjectSpace.CommitChanges();
                        View.ObjectSpace.Refresh();

                        AppHelper.CreateNewWindowDetailView(newSale);
                    }
                }
                else
                {
                    XtraMessageBox.Show("Number of guests must be greater then 0.");
                }
            }
        }
    }
}
