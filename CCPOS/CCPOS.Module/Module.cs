using CCPOS.Module.Utils;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.BaseImpl;
using HidLibrary;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CCPOS.Module
{
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
    public sealed partial class CCPOSModule : ModuleBase
    {
        public CCPOSModule()
        {
            InitializeComponent();
            BaseObject.OidInitializationMode = OidInitializationMode.AfterConstruction;
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
        {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new ModuleUpdater[] { updater };
        }
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            //set global font and size **THIS HAS BEEN MOVED TO Program.cs in CCPOS.Win, right before Setup is called.
            //DevExpress.XtraEditors.WindowsFormsSettings.DefaultFont = new Font("Tahoma", 11);
            #region CodeExampleForVCSpecificFontAndSizeAdjustments
            //if (this.View is DetailView)
            //{
            //    //cast detail view main Control to LayoutControl and adjust appearance fontsize delta
            //    (this.View.Control as DevExpress.XtraLayout.LayoutControl).Appearance.Control.FontSizeDelta = 5;

            //    //do something for each control item caption so sizes match, the above line does not adjust caption size.
            //    foreach (object obj in (this.View.Control as DevExpress.XtraLayout.LayoutControl).Items)
            //    {
            //        if (obj is LayoutControlItem)
            //        {
            //            LayoutControlItem layoutControlItem = (LayoutControlItem)obj;
            //            //Customize the current LayoutItem's settings
            //            layoutControlItem.AppearanceItemCaption.FontSizeDelta = 5;
            //        }
            //    }
            //}
            //else if (this.View is ListView)
            //{

            //}

            #endregion
            //set up AppHelper
            AppHelper.Application = application;
            AppHelper.VendorId = 0x05E0;

            //set up HelcimHelper (Helcim no longer used)
            //HelcimHelper.Client = new RestClient("https://secure.myhelcim.com/api/");
            //HelcimHelper.Client.Timeout = -1;

            //init scanner
            MyScanner.Scanner = HidDevices.Enumerate(0x05E0).FirstOrDefault();
            if (MyScanner.Scanner != null)
            {
                //MyScanner.Scanner.CloseDevice();
                MyScanner.Scanner.Inserted += Scanner_Inserted;
                MyScanner.Scanner.Removed += Scanner_Removed;
                //MyScanner.Scanner.MonitorDeviceEvents = true;
                //Set the call back when in a ListView or DetailView controller?
                //MyScanner.Scanner.ReadReport(ScannerInit);
            }
        }

        #region ScannerEvents
        private void ScannerInit(HidReport hidReport)
        {
            //donothing
        }
        private void Scanner_Removed()
        {
            // throw new NotImplementedException();
        }

        private void Scanner_Inserted()
        {
            // throw new NotImplementedException();
        }
        #endregion

    }
}
