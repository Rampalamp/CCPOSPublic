namespace CCPOS.Module.Win.Controllers.SaleControllers
{
    partial class SplitSaleVC
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
            this.cmdSplitSale = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.cmdSplitSaleEven = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // cmdSplitSale
            // 
            this.cmdSplitSale.Caption = "Split Sale Items";
            this.cmdSplitSale.Category = "View";
            this.cmdSplitSale.ConfirmationMessage = null;
            this.cmdSplitSale.Id = "cmdSplitSale";
            this.cmdSplitSale.ToolTip = null;
            this.cmdSplitSale.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.cmdSplitSale_Execute);
            // 
            // cmdSplitSaleEven
            // 
            this.cmdSplitSaleEven.Caption = "Split Sale Even";
            this.cmdSplitSaleEven.Category = "View";
            this.cmdSplitSaleEven.ConfirmationMessage = null;
            this.cmdSplitSaleEven.Id = "cmdSplitSaleEven";
            this.cmdSplitSaleEven.ToolTip = null;
            this.cmdSplitSaleEven.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.cmdSplitSaleEven_Execute);
            // 
            // SplitSaleVC
            // 
            this.Actions.Add(this.cmdSplitSale);
            this.Actions.Add(this.cmdSplitSaleEven);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction cmdSplitSale;
        private DevExpress.ExpressApp.Actions.SimpleAction cmdSplitSaleEven;
    }
}
