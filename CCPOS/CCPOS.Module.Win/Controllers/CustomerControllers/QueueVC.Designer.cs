namespace CCPOS.Module.Win.Controllers.CustomerControllers
{
    partial class QueueVC
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
            this.cmdStartQueue = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.cmdEndQueue = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // cmdStartQueue
            // 
            this.cmdStartQueue.Caption = "Start Queue";
            this.cmdStartQueue.Category = "View";
            this.cmdStartQueue.ConfirmationMessage = null;
            this.cmdStartQueue.Id = "cmdStartQueue";
            this.cmdStartQueue.ToolTip = null;
            this.cmdStartQueue.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.cmdStartQueue_Execute);
            // 
            // cmdEndQueue
            // 
            this.cmdEndQueue.Caption = "Remove From Queue";
            this.cmdEndQueue.Category = "View";
            this.cmdEndQueue.ConfirmationMessage = null;
            this.cmdEndQueue.Id = "cmdEndQueue";
            this.cmdEndQueue.ToolTip = null;
            this.cmdEndQueue.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.cmdEndQueue_Execute);
            // 
            // QueueVC
            // 
            this.Actions.Add(this.cmdStartQueue);
            this.Actions.Add(this.cmdEndQueue);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction cmdStartQueue;
        private DevExpress.ExpressApp.Actions.SimpleAction cmdEndQueue;
    }
}
