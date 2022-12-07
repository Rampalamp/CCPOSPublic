using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win;
using DevExpress.Persistent.BaseImpl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CCPOS.Module.Utils
{
    public class AppHelper
    {
        public static XafApplication Application { get; set; }

        public static Int32 VendorId { get; set; }
        /// <summary>
        /// Refresh each open ListView
        /// </summary>
        public static void RefreshListViews()
        {
            WinShowViewStrategyBase strategy = Application.ShowViewStrategy as WinShowViewStrategyBase;
            //iterate through and refresh listviews;
            foreach (WinWindow window in strategy.Windows)
            {
                if (window.View is ListView)
                {
                    window.View.ObjectSpace.Refresh();
                }
            }
        }
        /// <summary>
        /// Refresh each open ListView *** BUGGED ON CERTAIN EDGE CASES NEED TO FIX
        /// </summary>
        public static void RefreshListViews(string[] viewIds)
        {
            WinShowViewStrategyBase strategy = Application.ShowViewStrategy as WinShowViewStrategyBase;
            //iterate through and refresh listviews;
            foreach (WinWindow window in strategy.Windows)
            {
                if (viewIds.Contains(window.View.Id))
                {
                    window.View.ObjectSpace.Refresh();
                }
            }
        }
        /// <summary>
        /// Refresh each open DetailView
        /// </summary>
        public static void RefreshDetailViews()
        {
            WinShowViewStrategyBase strategy = Application.ShowViewStrategy as WinShowViewStrategyBase;
            //iterate through and refresh listviews;
            foreach (WinWindow window in strategy.Windows)
            {
                if (window.View is DetailView)
                {
                    window.View.ObjectSpace.Refresh();
                }
            }
        }
        /// <summary>
        /// Creates new ObjectSpace/DetailView for parameter object
        /// </summary>
        /// <param name="baseObject">Object to create DetailView on.</param>
        public static void CreateNewWindowDetailView(object baseObject)
        {
            IObjectSpace space = Application.CreateObjectSpace();
            ShowViewParameters svp = new ShowViewParameters(Application.CreateDetailView(space, space.GetObject(baseObject)));
            svp.CreateAllControllers = true;
            svp.TargetWindow = TargetWindow.NewWindow;

            Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
        }

    }
}
