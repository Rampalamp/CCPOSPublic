namespace CCPOS.Module.Win.Controllers.PaymentControllers
{
    partial class PaymentVC
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
            this.cmdPayBalance = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // cmdPayBalance
            // 
            this.cmdPayBalance.Caption = "Pay Balance";
            this.cmdPayBalance.Category = "View";
            this.cmdPayBalance.ConfirmationMessage = null;
            this.cmdPayBalance.Id = "cmdPayBalance";
            this.cmdPayBalance.ToolTip = null;
            this.cmdPayBalance.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.cmdPayBalance_Execute);
            // 
            // PaymentVC
            // 
            this.Actions.Add(this.cmdPayBalance);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction cmdPayBalance;
    }
}
