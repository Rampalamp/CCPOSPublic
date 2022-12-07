using CCPOS.Module.BusinessObjects.Inventory;
using CCPOS.Module.BusinessObjects.Sales;
using CCPOS.Module.Utils;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win;
using DevExpress.XtraEditors;
using HidLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CCPOS.Module.Win.Controllers.ScannerControllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.

    public partial class ProductScanVC : ViewController
    {
        public WinWindow CurrentView { get; set; }
        public Product SelectedSaleLineItemProduct { get; set; }
        public ProductScanVC()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            //TargetObjectType = typeof(Product);
            //Going to keep the view Id's specific for now
            TargetViewId = "Product_ListView;Product_DetailView;Sale_ListView;Sale_DetailView";
            //TargetViewType = ViewType.Any;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            //check if scanner is closed, if so open and set ReadReport call back.
            if (MyScanner.Scanner != null)
            {
                if (!MyScanner.Scanner.IsOpen)
                {
                    MyScanner.Scanner.OpenDevice();
                    MyScanner.Scanner.ReadReport(ScannerInput);
                }
            }
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
            //was attempting to catch last view open, but if the views were open on last close, only 1 VC exists on the screen. so it will be finicky more and more its used. fukit
            //int cn = (AppHelper.Application.ShowViewStrategy as MdiShowViewStrategy).Inspectors.Where(x => ("Product_ListView;Product_DetailView;Sale_ListView;Sale_DetailView").Contains(x.View.Id)).Count();
            //var agg = (AppHelper.Application.MainWindow.Template as Form).MdiParent;

            //if (MyScanner.OpenViewControllers == 1)
            //{
            //    MyScanner.Scanner.CloseDevice();
            //    MyScanner.Scanner.Dispose();
            //}

        }

        private void ScannerInput(HidReport report)
        {
            // process your data here
            MyScanner.SetScanTextValue(report.Data);

            ProductScanned(MyScanner.ScanTextValue);

        }

        private void ProductScanned(string barCode)
        {
            //NEED TO CAST MainWindow.Template as a Form, this should give us the main thread the entire windows app is running on. Then use the Invoke method on the Form to do the rest of the work to prevent cross threading when new view is created. Winner Winner Chicken Dinner.
            if (AppHelper.Application.MainWindow != null)
            {
                (AppHelper.Application.MainWindow.Template as Form).Invoke(new Action(() =>
                {
                    CurrentView = (AppHelper.Application.ShowViewStrategy as MdiShowViewStrategy).GetActiveInspector();

                    if (CurrentView != null)
                    {
                        //If user creates new Sale using devex button then scan a product before ever initially saving Sale, then when grabbing the curSale to add product to, the newly created space doesnt find it and returns a null. Call Commit if CurrentView is found before any more scanner logic runs.
                        CurrentView.View?.ObjectSpace.CommitChanges();

                        if (CurrentView.View?.ObjectTypeInfo?.Type?.Name == "Product")
                        {
                            //Do a search on product barcode, do something if not found, do something if found.

                            IObjectSpace space = AppHelper.Application.CreateObjectSpace();
                            IList<Product> foundProducts = space.GetObjects<Product>(new BinaryOperator("Barcode", barCode));

                            if (foundProducts.Count > 1)
                            {
                                //show modal pop up list view with collection of returns products?
                                MultiProductFoundPopUp(space, foundProducts);

                            }
                            else if (foundProducts.Count == 1)
                            {
                                Product prod = foundProducts.FirstOrDefault();

                                ShowViewParameters svp = new ShowViewParameters(AppHelper.Application.CreateDetailView(space, prod));
                                svp.CreateAllControllers = true;
                                svp.TargetWindow = TargetWindow.NewWindow;

                                AppHelper.Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));

                            }
                            else
                            {
                                //none found - Prompt to ask if they want to create a new one, then append the Barcode.
                                if (XtraMessageBox.Show("No product found for barcode " + barCode + ". Would you like to create one?", "No Product Found", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    Product newProd = space.CreateObject<Product>();
                                    newProd.Barcode = barCode;

                                    ShowViewParameters svp = new ShowViewParameters(AppHelper.Application.CreateDetailView(space, newProd));
                                    svp.CreateAllControllers = true;
                                    svp.TargetWindow = TargetWindow.NewWindow;

                                    AppHelper.Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
                                }
                                else
                                {
                                    //do nothing?
                                }
                            }

                        }
                        else if (CurrentView.View?.ObjectTypeInfo?.Type?.Name == "Sale")
                        {
                            //do work
                            IObjectSpace space = AppHelper.Application.CreateObjectSpace();
                            IList<Product> foundProducts = space.GetObjects<Product>(new BinaryOperator("Barcode", barCode));
                            if (CurrentView.View?.Id == "Sale_ListView")
                            {

                                if (XtraMessageBox.Show("Would you like to create a new sale?", "Create Sale", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    Sale newSale = space.CreateObject<Sale>();
                                    space.CommitChanges();

                                    ProductScannedForSale(space, newSale, foundProducts);

                                    ShowViewParameters svp = new ShowViewParameters(AppHelper.Application.CreateDetailView(space, newSale));
                                    svp.CreateAllControllers = true;
                                    svp.TargetWindow = TargetWindow.NewWindow;

                                    AppHelper.Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));

                                }
                                else
                                {
                                    //do nothing?
                                }
                            }
                            else if (CurrentView.View?.Id == "Sale_DetailView")
                            {
                                //create SaleLineItem
                                if (CurrentView.View?.CurrentObject is Sale)
                                {
                                    Sale curSale = space.GetObjectByKey<Sale>((CurrentView.View?.CurrentObject as Sale).Oid);

                                    ProductScannedForSale(space, curSale, foundProducts);
                                }
                            }
                        }
                    }
                }));

            }
            //prime scanner to accept another barcode read
            if (MyScanner.Scanner.IsOpen)
            {
                MyScanner.Scanner.ReadReport(ScannerInput);
            }

        }

        private void ProductScannedForSale(IObjectSpace space, Sale sale, IList<Product> foundProducts)
        {
            if (foundProducts.Count > 1)
            {
                MultiProductFoundPopUp(space, foundProducts);
                if (SelectedSaleLineItemProduct != null)
                {
                    AddSaleLineItemWithProduct(space, space.GetObject(SelectedSaleLineItemProduct), sale);
                }
            }
            else if (foundProducts.Count == 1)
            {
                Product product = foundProducts.FirstOrDefault();
                //Don't think I need to do any quantity validation here? Perhaps give warning on Sale OnSave()
                AddSaleLineItemWithProduct(space, product, sale);

            }
            else if (foundProducts.Count == 0)
            {
                XtraMessageBox.Show("No product found.");
            }
        }

        private void AddSaleLineItemWithProduct(IObjectSpace space, Product product, Sale curSale)
        {
            SaleLineItem newItem = space.CreateObject<SaleLineItem>();
            newItem.Product = product;
            newItem.Sale = curSale;
            curSale.SaleLineItems.Add(newItem);
            //need to call .Save() on the current sale for the Sale.cs OnSaving() to trigger on the space.CommitChanges()
            curSale.Save();
            space.CommitChanges();

            CurrentView.View.ObjectSpace.Refresh();

        }

        private void MultiProductFoundPopUp(IObjectSpace space, IList<Product> foundProducts)
        {
            List<CriteriaOperator> crit = new List<CriteriaOperator>();

            foreach (Product product in foundProducts)
            {
                crit.Add(new BinaryOperator("Oid", product.Oid));
            }

            using (CollectionSource cs = new CollectionSource(space, typeof(Product)))
            {

                DevExpress.ExpressApp.ListView view = AppHelper.Application.CreateListView("Product_ListView", cs, false); ;

                DialogController dc = new DialogController();
                dc.AcceptAction.Execute += AcceptAction_Execute;
                dc.CancelAction.Execute += CancelAction_Execute;
                dc.FrameAssigned += new EventHandler(DialogController_FrameAssigned);
                ShowViewParameters svp = new ShowViewParameters(view);
                svp.TargetWindow = TargetWindow.NewModalWindow;
                //svp.Context = TemplateContext.PopupWindow;
                svp.Controllers.Add(dc);

                AppHelper.Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(this.Frame, null));
            }
        }
        void DialogController_FrameAssigned(object sender, EventArgs e)
        {
            ListViewProcessCurrentObjectController listViewProcessCurrentObjectController = ((Controller)sender).Frame.GetController<ListViewProcessCurrentObjectController>();
            listViewProcessCurrentObjectController.CustomProcessSelectedItem += new EventHandler<CustomProcessListViewSelectedItemEventArgs>(listViewProcessCurrentObjectController_CustomProcessSelectedItem);
        }
        void listViewProcessCurrentObjectController_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            if (CurrentView.View?.ObjectTypeInfo?.Type?.Name == "Product")
            {
                IObjectSpace space = AppHelper.Application.CreateObjectSpace();
                object obj = space.GetObject(e.InnerArgs.CurrentObject);
                e.InnerArgs.ShowViewParameters.CreatedView = AppHelper.Application.CreateDetailView(space, obj);
                e.InnerArgs.ShowViewParameters.CreateAllControllers = true;
                e.InnerArgs.ShowViewParameters.TargetWindow = TargetWindow.NewWindow;
                e.Handled = true;
            }
            else if (CurrentView.View?.ObjectTypeInfo?.Type?.Name == "Sale")
            {
                IObjectSpace space = AppHelper.Application.CreateObjectSpace();
                object obj = space.GetObject(e.InnerArgs.CurrentObject);
                SelectedSaleLineItemProduct = obj as Product;
                e.Handled = true;
            }
        }
        private void CancelAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (CurrentView.View?.ObjectTypeInfo?.Type?.Name == "Product")
            {
                //do nothing?
            }
            else if (CurrentView.View?.ObjectTypeInfo?.Type?.Name == "Sale")
            {
                SelectedSaleLineItemProduct = null;
            }
            //throw new NotImplementedException();
        }

        private void AcceptAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (CurrentView.View?.ObjectTypeInfo?.Type?.Name == "Product")
            {
                if (e.SelectedObjects.Count == 1)
                {
                    if (e.CurrentObject is Product)
                    {
                        IObjectSpace space = AppHelper.Application.CreateObjectSpace();
                        Product product = space.GetObjectByKey<Product>((e.CurrentObject as Product).Oid);

                        e.ShowViewParameters.CreatedView = AppHelper.Application.CreateDetailView(space, product);
                        e.ShowViewParameters.CreateAllControllers = true;
                        e.ShowViewParameters.TargetWindow = TargetWindow.NewWindow;
                    }

                }
                else if (e.SelectedObjects.Count > 1)
                {
                    foreach (var item in e.SelectedObjects)
                    {
                        if (item is Product)
                        {
                            IObjectSpace space = AppHelper.Application.CreateObjectSpace();
                            Product product = space.GetObjectByKey<Product>((item as Product).Oid);

                            DetailView dv = AppHelper.Application.CreateDetailView(space, product);
                            //e.ShowViewParameters.CreatedView = dv;
                            //e.ShowViewParameters.CreateAllControllers = true;
                            //e.ShowViewParameters.TargetWindow = TargetWindow.NewWindow;

                            ShowViewParameters viewP = new ShowViewParameters();
                            viewP.CreatedView = dv;
                            viewP.TargetWindow = TargetWindow.NewWindow;
                            //Show the newly created view parameters/detail view.
                            AppHelper.Application.ShowViewStrategy.ShowView(viewP, new ShowViewSource(AppHelper.Application.MainWindow, null));
                        }
                    }
                }
            }
            else if (CurrentView.View?.ObjectTypeInfo?.Type?.Name == "Sale")
            {
                if (CurrentView.View?.Id == "Sale_DetailView")
                {
                    if (e.SelectedObjects.Count == 1)
                    {
                        SelectedSaleLineItemProduct = e.CurrentObject as Product;
                    }
                    else
                    {
                        XtraMessageBox.Show("Please select one.");
                    }
                }
            }

            //throw new NotImplementedException();
        }

    }
}
