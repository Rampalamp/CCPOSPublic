namespace CCPOS.Module.Win.Controllers.EventControllers
{
    partial class NewCustomerVC
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cmdNewCustomer = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // cmdNewCustomer
            // 
            this.cmdNewCustomer.Caption = "New Customer";
            this.cmdNewCustomer.Category = "View";
            this.cmdNewCustomer.ConfirmationMessage = null;
            this.cmdNewCustomer.Id = "cmdNewCustomer";
            this.cmdNewCustomer.ToolTip = null;
            this.cmdNewCustomer.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.cmdNewCustomer_Execute);
            // 
            // NewCustomerVC
            // 
            this.Actions.Add(this.cmdNewCustomer);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction cmdNewCustomer;
    }
}
