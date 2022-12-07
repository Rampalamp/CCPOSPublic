namespace CCPOS.Module.Win.Controllers.SaleControllers
{
    partial class CompleteTransactionVC
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
            this.cmdCompleteTransaction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // cmdCompleteTransaction
            // 
            this.cmdCompleteTransaction.Caption = "Complete Transaction";
            this.cmdCompleteTransaction.Category = "View";
            this.cmdCompleteTransaction.ConfirmationMessage = null;
            this.cmdCompleteTransaction.Id = "cmdCompleteTransaction";
            this.cmdCompleteTransaction.ToolTip = null;
            this.cmdCompleteTransaction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.cmdCompleteTransaction_Execute);
            // 
            // CompleteTransactionVC
            // 
            this.Actions.Add(this.cmdCompleteTransaction);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction cmdCompleteTransaction;
    }
}
