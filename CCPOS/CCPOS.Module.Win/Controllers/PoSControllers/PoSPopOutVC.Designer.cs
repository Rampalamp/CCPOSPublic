namespace CCPOS.Module.Win.Controllers.PoSControllers
{
    partial class PoSPopOutVC
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
            this.cmdPoSPopOut = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // cmdPoSPopOut
            // 
            this.cmdPoSPopOut.Caption = "PointOfSale";
            this.cmdPoSPopOut.Category = "Tools";
            this.cmdPoSPopOut.ConfirmationMessage = null;
            this.cmdPoSPopOut.Id = "cmdPoSPopOut";
            this.cmdPoSPopOut.ToolTip = null;
            this.cmdPoSPopOut.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.cmdPoSPopOut_Execute);
            // 
            // PoSPopOutVC
            // 
            this.Actions.Add(this.cmdPoSPopOut);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction cmdPoSPopOut;
    }
}
