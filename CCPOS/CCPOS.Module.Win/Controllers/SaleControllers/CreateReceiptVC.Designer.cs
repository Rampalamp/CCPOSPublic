namespace CCPOS.Module.Win.Controllers.SaleControllers
{
    partial class CreateReceiptVC
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
            this.cmdCreateReceipt = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // cmdCreateReceipt
            // 
            this.cmdCreateReceipt.Caption = "Create Receipt";
            this.cmdCreateReceipt.Category = "View";
            this.cmdCreateReceipt.ConfirmationMessage = null;
            this.cmdCreateReceipt.Id = "cmdCreateReceipt";
            this.cmdCreateReceipt.ToolTip = null;
            this.cmdCreateReceipt.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.cmdCreateReceipt_Execute);
            // 
            // CreateReceiptVC
            // 
            this.Actions.Add(this.cmdCreateReceipt);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction cmdCreateReceipt;
    }
}
