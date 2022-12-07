namespace CCPOS.Module.Win.Controllers.SaleControllers
{
    partial class PrintReceiptVC
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
            this.cmdPrintReceipt = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.cmdPrintFoodReceipt = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // cmdPrintReceipt
            // 
            this.cmdPrintReceipt.Caption = "Print Receipt";
            this.cmdPrintReceipt.Category = "View";
            this.cmdPrintReceipt.ConfirmationMessage = null;
            this.cmdPrintReceipt.Id = "cmdPrintReceipt";
            this.cmdPrintReceipt.ToolTip = null;
            this.cmdPrintReceipt.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.cmdPrintReceipt_Execute);
            // 
            // cmdPrintFoodReceipt
            // 
            this.cmdPrintFoodReceipt.Caption = "Print Food Receipt";
            this.cmdPrintFoodReceipt.Category = "View";
            this.cmdPrintFoodReceipt.ConfirmationMessage = null;
            this.cmdPrintFoodReceipt.Id = "cmdPrintFoodReceipt";
            this.cmdPrintFoodReceipt.ToolTip = null;
            this.cmdPrintFoodReceipt.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.cmdPrintFoodReceipt_Execute);
            // 
            // PrintReceiptVC
            // 
            this.Actions.Add(this.cmdPrintReceipt);
            this.Actions.Add(this.cmdPrintFoodReceipt);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction cmdPrintReceipt;
        private DevExpress.ExpressApp.Actions.SimpleAction cmdPrintFoodReceipt;
    }
}
