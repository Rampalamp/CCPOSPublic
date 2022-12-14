using CCPOS.Module.Win.WinForms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;

namespace CCPOS.Module.Win.Controllers.PoSControllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class PoSPopOutVC : ViewController
    {
        public PoSPopOutVC()
        {
            InitializeComponent();
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

        private void cmdPoSPopOut_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            PoSForm pos = new PoSForm();
            pos.Show();
        }
    }
}
