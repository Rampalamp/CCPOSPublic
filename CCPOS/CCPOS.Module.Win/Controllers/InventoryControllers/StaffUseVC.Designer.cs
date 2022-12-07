namespace CCPOS.Module.Win.Controllers.InventoryControllers
{
    partial class StaffUseVC
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
            this.cmdCreateStaffLineItem = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // cmdCreateStaffLineItem
            // 
            this.cmdCreateStaffLineItem.Caption = "cmd Create Staff Line Item";
            this.cmdCreateStaffLineItem.Category = "View";
            this.cmdCreateStaffLineItem.ConfirmationMessage = null;
            this.cmdCreateStaffLineItem.Id = "cmdCreateStaffLineItem";
            this.cmdCreateStaffLineItem.ToolTip = null;
            this.cmdCreateStaffLineItem.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.cmdCreateStaffLineItem_Execute);
            // 
            // StaffUseVC
            // 
            this.Actions.Add(this.cmdCreateStaffLineItem);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction cmdCreateStaffLineItem;
    }
}
