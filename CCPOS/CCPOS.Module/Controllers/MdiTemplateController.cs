using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Persistent.BaseImpl;
using DevExpress.XtraBars.Docking2010.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCPOS.Module.Controllers
{

    /// <summary>
    /// Used to perform any actions required on the Deactivation of a Tab View or Activiation of a Tab View
    /// </summary>
    public class MdiTemplateController : WinWindowTemplateController
    {
        //This acts as the auto save feature. Each time a Tabbed MDI View is changed, BEFORE or on DEACTIVATION commit the views changes
        //put in place to counter act object changed by another user error.
        protected override void OnDocumentDeactivated(object sender, DocumentEventArgs e)
        {
            base.OnDocumentDeactivated(sender, e);
            //Perform any work to be done before Tab view changed.
            //Commit object space each time tab is changing views.
            if (e.Document.Form is IViewHolder)
            {
                try
                {
                    //threw this in a try catch because some of the new scheduling module stuff was causing a rogue Null reference error when closing the application. Very strange.
                    ((IViewHolder)e.Document.Form)?.View?.ObjectSpace?.CommitChanges();
                    ((IViewHolder)e.Document.Form)?.View?.ObjectSpace?.Refresh();
                }
                catch (Exception)
                {
                    //Expecting a random null reference error in here. the null hits even after I do null checks on every object accessed/used
                    
                }
            }
        }

        protected override void OnDocumentActivated(DocumentEventArgs e)
        {
            base.OnDocumentActivated(e);
            //Perform any work upon entry of TabView
            //I believe having a refresh on opening, and an auto save/refresh on closing, will prevent any further object changed by another user error.
            if (e.Document.Form is IViewHolder)
            {
                try
                {
                    //threw this in a try catch because some of the new scheduling module stuff was causing a rogue Null reference error when closing the application. Very strange. On Closing of XafApplication Reservations_ListView throws a null error on Refresh?
                    BaseObject obj = (((IViewHolder)e.Document.Form)?.View?.CurrentObject as BaseObject);
                    //the refresh causes a warning window on NEW object creations. Refreshing before the object had ever been saved before.
                    if (obj.Session.IsNewObject(obj))
                        return;

                    ((IViewHolder)e.Document.Form)?.View?.ObjectSpace?.Refresh();
                }
                catch (Exception)
                {
                    //Expecting a random null reference error in here. the null hits even after I do null checks on every object accessed/used

                }
            }
        }
    }
}
