namespace CCPOS.Module.Win.Controllers.SaleControllers
{
    partial class CustomerInvoiceVC
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
            this.cmdEmailCustomerInvoice = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.cmdCreateInvoice = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // cmdEmailCustomerInvoice
            // 
            this.cmdEmailCustomerInvoice.Caption = "Email Invoice";
            this.cmdEmailCustomerInvoice.Category = "View";
            this.cmdEmailCustomerInvoice.ConfirmationMessage = null;
            this.cmdEmailCustomerInvoice.Id = "cmdEmailCustomerInvoice";
            this.cmdEmailCustomerInvoice.ToolTip = null;
            this.cmdEmailCustomerInvoice.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.cmdEmailCustomerInvoice_Execute);
            // 
            // cmdCreateInvoice
            // 
            this.cmdCreateInvoice.Caption = "Create Invoice";
            this.cmdCreateInvoice.Category = "View";
            this.cmdCreateInvoice.ConfirmationMessage = null;
            this.cmdCreateInvoice.Id = "cmdCreateInvoice";
            this.cmdCreateInvoice.ToolTip = null;
            this.cmdCreateInvoice.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.cmdCreateInvoice_Execute);
            // 
            // CustomerInvoiceVC
            // 
            this.Actions.Add(this.cmdEmailCustomerInvoice);
            this.Actions.Add(this.cmdCreateInvoice);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction cmdEmailCustomerInvoice;
        private DevExpress.ExpressApp.Actions.SimpleAction cmdCreateInvoice;
    }
}
