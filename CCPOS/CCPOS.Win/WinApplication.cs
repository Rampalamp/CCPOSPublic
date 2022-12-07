using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Utils;
using DevExpress.ExpressApp.Xpo;

namespace CCPOS.Win
{
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Win.WinApplication._members
    public partial class CCPOSWindowsFormsApplication : WinApplication
    {
        #region Default XAF configuration options (https://www.devexpress.com/kb=T501418)
        static CCPOSWindowsFormsApplication()
        {
            DevExpress.Persistent.Base.PasswordCryptographer.EnableRfc2898 = true;
            DevExpress.Persistent.Base.PasswordCryptographer.SupportLegacySha512 = false;
            DevExpress.ExpressApp.Utils.ImageLoader.Instance.UseSvgImages = true;
        }
        private void InitializeDefaults()
        {
            LinkNewObjectToParentImmediately = false;
            OptimizedControllersCreation = true;
            UseLightStyle = true;
            SplashScreen = new DXSplashScreen(typeof(XafSplashScreen), new DefaultOverlayFormOptions());
            ExecuteStartupLogicBeforeClosingLogonWindow = true;
        }
        #endregion
        public CCPOSWindowsFormsApplication()
        {
            InitializeComponent();
            InitializeDefaults();
        }
        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
        {
            //Adjusted default settings, ThreadSafe for XPObjectSpaceProvider set to true for background workers
            args.ObjectSpaceProviders.Add(new XPObjectSpaceProvider(XPObjectSpaceProvider.GetDataStoreProvider(args.ConnectionString, args.Connection, true), true));
            args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider(TypesInfo, null));
        }
        private void CCPOSWindowsFormsApplication_CustomizeLanguagesList(object sender, CustomizeLanguagesListEventArgs e)
        {
            string userLanguageName = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            if (userLanguageName != "en-US" && e.Languages.IndexOf(userLanguageName) == -1)
            {
                e.Languages.Add(userLanguageName);
            }
        }
        private void CCPOSWindowsFormsApplication_DatabaseVersionMismatch(object sender, DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs e)
        {

            e.Updater.Update();
            e.Handled = true;

            //        if(System.Diagnostics.Debugger.IsAttached) {
            //            e.Updater.Update();
            //            e.Handled = true;
            //        }
            //        else {
            //string message = "The application cannot connect to the specified database, " +
            //	"because the database doesn't exist, its version is older " +
            //	"than that of the application or its schema does not match " +
            //	"the ORM data model structure. To avoid this error, use one " +
            //	"of the solutions from the https://www.devexpress.com/kb=T367835 KB Article.";

            //if(e.CompatibilityError != null && e.CompatibilityError.Exception != null) {
            //	message += "\r\n\r\nInner exception: " + e.CompatibilityError.Exception.Message;
            //}
            //throw new InvalidOperationException(message);
            //        }

        }
    }
}
