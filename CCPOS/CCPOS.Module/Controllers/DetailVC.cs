using CCPOS.Module.BusinessObjects.Sales;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

namespace CCPOS.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.

    /// <summary>
    /// To be used for broad alterations across all detail views to open
    /// </summary>
    public partial class DetailVC : ViewController
    {
        public DetailVC()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetViewType = ViewType.DetailView;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            //below is hiding some default active controllers for each DetailView. 
            Frame.GetController<RecordsNavigationController>()?.Active.SetItemValue("hide", false);
            Frame.GetController<ResetViewSettingsController>()?.Active.SetItemValue("hide", false);
            //check for opening of a sale
            if (View.Id == "Sale_DetailView")
            {
                //If NewOnlineSale is true, should be first time opening a sale created from Helcim Order Sync
                if ((View.CurrentObject as Sale).NewOnlineSale)
                {
                    //set to false, assume they are handling the sale, and it shouldn't show up in alerts anymore.
                    (View.CurrentObject as Sale).NewOnlineSale = false;
                    //commit change so they potentially dont get confused if they close it with out touching anything, and it asks about saving.
                    View.ObjectSpace.CommitChanges();
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
        }
    }
}
