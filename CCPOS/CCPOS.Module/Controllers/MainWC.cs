using CCPOS.Module.BusinessObjects.Customer;
using CCPOS.Module.BusinessObjects.Sales;
using CCPOS.Module.Utils;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.BaseImpl;
using DevExpress.XtraBars.Alerter;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.Tabbed;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CCPOS.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppWindowControllertopic.aspx

    /// <summary>
    /// Starts up Helcim and Alert timers. Also inits the app to start in full screened
    /// </summary>
    public partial class MainWC : WindowController
    {

        //USING THIS WINDOW CONTROLLER TO SET MAIN WINDOW FRAME TO MAXIMIZE ON SCREEN ON LAUNCH AND START HELCIM SYNC TIMER
        //CAN USE FOR ANYTHING ON MAIN WINDOW I THINK...
        private Timer helcimTimer;
        static MainWC instance = null;
        private Timer alertTimerCore;
        List<AlertQueueItem> alertQueue = new List<AlertQueueItem>();
        public bool isBusy { get; set; }
        int maxAlerts = 5;

        public MainWC()
        {
            InitializeComponent();
            // Target required Windows (via the TargetXXX properties) and create their Actions.
            TargetWindowType = WindowType.Main;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target Window.

            Window.TemplateChanged += Window_TemplateChanged;
        }

        private void Window_TemplateChanged(object sender, EventArgs e)
        {
            if (Window.Template is Form && Window.Template is ISupportStoreSettings)
                ((ISupportStoreSettings)Window.Template).SettingsReloaded += OnFormReadyForCustomizations;
        }

        private void OnFormReadyForCustomizations(object sender, EventArgs e)
        {
            ((Form)sender).WindowState = FormWindowState.Maximized;
            #region CoreTimers (Unused right now)
            ////init alert/notification timer
            //InitAlertTimerCore();
            ////start helcim timer to run while app is open. ONlY FOR LIVE CONN STRING
            //if (ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString.Contains("WineBarLive"))
            //{
            //    InitHelcimTimer();
            //    //perform one initial connection call at start up. I fear that a PHPSESSID is generated but expires from over night or maybe minutes
            //    HelcimHelper.PostToHelcim(new Dictionary<string, string>
            //    {
            //        {"action","connectionTest"},
            //    });
            //}
            #endregion

        }

        private void InitHelcimTimer()
        {
            helcimTimer = new Timer();
            helcimTimer.Interval = 240000;
            helcimTimer.Tick += HelcimTimer_Tick;
            helcimTimer.Start();
        }

        private void HelcimTimer_Tick(object sender, EventArgs e)
        {
            HelcimHelper.StartHelcimSync(false, "newOrders");
        }

        protected void InitAlertTimerCore()
        {
            alertTimerCore = new Timer();
            alertTimerCore.Tick += AlertTimerCore_Tick;
            alertTimerCore.Interval = 120000;
            alertTimerCore.Start();
        }

        private void AlertTimerCore_Tick(object sender, EventArgs e)
        {
            if (!isBusy)
            {
                isBusy = true;
                try
                {
                    List<Guid> oids = new List<Guid>();
                    foreach (AlertQueueItem item in alertQueue)
                    {
                        if (item.Sale != null) oids.Add(item.Sale.Oid);
                        if (item.Customer != null) oids.Add(item.Customer.Oid);
                    }

                    using (IObjectSpace space = Application.CreateObjectSpace())
                    {
                        CriteriaOperator saleCriteria = new GroupOperator(GroupOperatorType.And, new CriteriaOperator[]
                        {
                            CriteriaOperator.Parse("[NewOnlineSale] = ?", true),
                            new NotOperator(new InOperator("Oid",oids))
                        });

                        IList saleAlerts = space.CreateCollection(typeof(Sale), saleCriteria);
                        //*****not using customer alerts just yet.
                        //CriteriaOperator customerCriteria = new GroupOperator(GroupOperatorType.And, new CriteriaOperator[]
                        //{
                        //    CriteriaOperator.Parse("[InQueue] = ?", true),
                        //    new NotOperator(new InOperator("Oid",oids))
                        //});

                        //IList customerAlerts = space.CreateCollection(typeof(Customer), customerCriteria);
                        //send in null for now.
                        IList customerAlerts = null;
                        AddAlerts(saleAlerts, customerAlerts);
                    }
                }
                finally
                {
                    isBusy = false;
                }
            }
        }

        private void AddAlerts(IList saleAlerts, IList customerAlerts)
        {
            try
            {
                if (saleAlerts != null)
                    foreach (Sale alert in saleAlerts)
                        alertQueue.Add(new AlertQueueItem() { Sale = alert });
                if (customerAlerts != null)
                    foreach (Customer alert in customerAlerts)
                        alertQueue.Add(new AlertQueueItem() { Customer = alert });

                ShowAlerts();

            }
            catch (Exception ex)
            {

                isBusy = false;
            }
        }

        private void RemoveAlert(Sale sale)
        {
            AlertQueueItem toRemove = null;

            foreach (AlertQueueItem item in alertQueue)
                if (item.Sale?.Oid == sale.Oid)
                    toRemove = item;

            alertQueue.Remove(toRemove);

        }

        private void RemoveAlert(Customer customer)
        {
            AlertQueueItem toRemove = null;

            foreach (AlertQueueItem item in alertQueue)
                if (item.Customer?.Oid == customer.Oid)
                    toRemove = item;

            alertQueue.Remove(toRemove);

        }

        private AlertControl CreateAlertControl(BaseObject obj)
        {
            CCPOSAlertControl newAlert = new CCPOSAlertControl();

            if (obj is Sale)
                newAlert.Sale = obj as Sale;
            if (obj is Customer)
                newAlert.Customer = obj as Customer;

            newAlert.ShowPinButton = true;
            newAlert.ShowCloseButton = true;
            newAlert.AutoFormDelay = int.MaxValue;

            //AlertButton dismiss = new AlertButton();
            //dismiss.Hint = "Dismiss";
            //dismiss.Name = "Dismiss";

            //AlertButton open = new AlertButton();
            //open.Hint = "Open";
            //open.Name = "Open";

            //newAlert.Buttons.Add(open);
            //newAlert.Buttons.Add(dismiss);

            //newAlert.ButtonClick += NewAlert_ButtonClick;
            newAlert.AlertClick += NewAlert_AlertClick;

            return newAlert;
        }

        private void NewAlert_AlertClick(object sender, AlertClickEventArgs e)
        {

            if ((sender as CCPOSAlertControl).Sale != null)
            {
                RemoveAlert((sender as CCPOSAlertControl).Sale);
                AppHelper.CreateNewWindowDetailView((sender as CCPOSAlertControl).Sale);
            }
            if ((sender as CCPOSAlertControl).Customer != null)
            {
                RemoveAlert((sender as CCPOSAlertControl).Customer);
                AppHelper.CreateNewWindowDetailView((sender as CCPOSAlertControl).Customer);
            }

            e.AlertForm.Dispose();

        }

        private void NewAlert_ButtonClick(object sender, AlertButtonClickEventArgs e)
        {
            switch (e.ButtonName)
            {
                case "Open":

                    if ((sender as CCPOSAlertControl).Sale != null)
                    {
                        RemoveAlert((sender as CCPOSAlertControl).Sale);
                        AppHelper.CreateNewWindowDetailView((sender as CCPOSAlertControl).Sale);
                    }
                    if ((sender as CCPOSAlertControl).Customer != null)
                    {
                        RemoveAlert((sender as CCPOSAlertControl).Customer);
                        AppHelper.CreateNewWindowDetailView((sender as CCPOSAlertControl).Customer);
                    }

                    break;
                case "Dismiss":
                    break;
                default:
                    break;
            }

            e.AlertForm.Dispose();
        }

        private void ShowAlerts()
        {
            int count = 0;

            foreach (AlertQueueItem item in alertQueue)
            {
                if (item.Sale != null)
                {
                    item.AlertControl = CreateAlertControl(item.Sale);
                    //sale alert info
                    StringBuilder sb = new StringBuilder();
                    foreach (SaleLineItem lineItem in item.Sale.SaleLineItems)
                        sb.AppendLine(String.Format("{0} x {1}", lineItem.Product?.Name, Convert.ToInt32(lineItem.Quantity)));

                    //throw date after sale line items
                    sb.AppendLine(item.Sale.HelcimDateCreated.ToString());

                    AlertInfo info = new AlertInfo(
                        String.Format("New Order - Sale#{0}", item.Sale.SaleNumber.ToString()),
                        sb.ToString()
                        );

                    item.AlertControl.Show(Frame.Template as Form, info);
                }

                if (item.Customer != null)
                {
                    item.AlertControl = CreateAlertControl(item.Customer);
                    //customer alert info
                    AlertInfo info = new AlertInfo(
                        String.Format("{0} - {1}", item.Customer?.FullName, item.Customer?.PhoneNumber),
                        String.Format("Queue Start - {0}", item.Customer?.QueueStartTime)
                        );

                    item.AlertControl.Show(Frame.Template as Form, info);
                }
                count++;
                if (count == maxAlerts)
                    break;
            }
        }

        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            //turn off scanner
            MyScanner.Scanner?.CloseDevice();

            if(helcimTimer != null)
            {
                helcimTimer.Stop();
                helcimTimer.Dispose();
                helcimTimer = null;
            }
            
            if (alertTimerCore != null)
            {
                alertTimerCore.Stop();
                alertTimerCore.Dispose();
                alertTimerCore = null;
            }

            if (instance == this) instance = null;

            //RunbackUpScript if conn is live.
            if (AppHelper.Application.ConnectionString.Contains("TheWineBarLive"))
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Scripts\TheWineBarLiveBackUpScript.sql"))
                {
                    string script = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Scripts\TheWineBarLiveBackUpScript.sql");
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                    {
                        Server server = new Server(new ServerConnection(conn));
                        server.ConnectionContext.ExecuteNonQuery(script);
                    }
                }
            }

            Window.TemplateChanged -= Window_TemplateChanged;

            base.OnDeactivated();
        }
    }
    public class AlertQueueItem
    {
        public Sale Sale { get; set; }
        public Customer Customer { get; set; }
        public AlertControl AlertControl { get; set; }
        public bool Shown { get; set; }
    }

    public class CCPOSAlertControl : AlertControl
    {
        public Sale Sale { get; set; }
        public Customer Customer { get; set; }
    }
}
