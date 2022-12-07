using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors;
using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace CCPOS.Win
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if EASYTEST
            DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif
            WindowsFormsSettings.LoadApplicationSettings();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DevExpress.Utils.ToolTipController.DefaultController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
            DevExpress.ExpressApp.BaseObjectSpace.ThrowExceptionForNotRegisteredEntityType = true;
            EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.IsAttached;
            if (Tracing.GetFileLocationFromSettings() == DevExpress.Persistent.Base.FileLocation.CurrentUserApplicationDataFolder)
            {
                Tracing.LocalUserAppDataPath = Application.LocalUserAppDataPath;
            }
            Tracing.Initialize();
            CCPOSWindowsFormsApplication winApplication = new CCPOSWindowsFormsApplication();
            SecurityStrategy security = (SecurityStrategy)winApplication.Security;
            security.RegisterXPOAdapterProviders();
            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null)
            {
                winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
#if EASYTEST
            if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
                winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
            }
#endif
#if DEBUG
            //if (System.Diagnostics.Debugger.IsAttached && winApplication.CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema)
            //{
            //    winApplication.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
            //}
#endif
            try
            {
                //I am hoping below will cause the Updater to run after a fresh install of CCPOS on winebar machine. Removing the Debugger.IsAttached from above I think should cuase it to hit...? We shall see.
                if (winApplication.CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema)
                {
                    winApplication.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
                }

                //set font and size. Probably can do a number of win form customizations to entire app at this point.
                WindowsFormsSettings.DefaultFont = new Font("Tahoma", 12.5f);

                winApplication.Setup();
                winApplication.Start();
            }
            catch (Exception e)
            {
                winApplication.StopSplash();
                winApplication.HandleException(e);
            }
        }
    }
}
