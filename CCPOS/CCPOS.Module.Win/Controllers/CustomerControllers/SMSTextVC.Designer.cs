namespace CCPOS.Module.Win.Controllers.CustomerControllers
{
    partial class SMSTextVC
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
            this.cmdSMSText = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // cmdSMSText
            // 
            this.cmdSMSText.Caption = "SMS";
            this.cmdSMSText.Category = "Tools";
            this.cmdSMSText.ConfirmationMessage = null;
            this.cmdSMSText.Id = "cmdSMSText";
            this.cmdSMSText.ToolTip = null;
            this.cmdSMSText.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.cmdSMSText_Execute);
            // 
            // SMSTextVC
            // 
            this.Actions.Add(this.cmdSMSText);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction cmdSMSText;
    }
}
