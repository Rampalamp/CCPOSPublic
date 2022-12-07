using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CCPOS.Module.Win.WinForms
{
    public partial class SplitSaleForm : MaterialForm
    {
        public bool splitSale;
        public decimal Number;

        public SplitSaleForm()
        {
            InitializeComponent();
            // Create a material theme manager and add the form to manage (this)
            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

            // Configure color schema
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Green400, Primary.Green500,
                Primary.Green500, Accent.Green100,
                TextShade.WHITE
            );
            this.partySize.KeyDown += PartySize_KeyDown;
            //pretty sure splitSale inits as false, next level paranoia
            splitSale = false;
            this.FormBorderStyle = FormBorderStyle.None;
            //set both max and min size to default size - prevent any resizing
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.CenterToScreen();
        }

        private void PartySize_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.btnSplitSale.PerformClick();
        }

        private void btnSplitSale_Click(object sender, EventArgs e)
        {
            splitSale = true;
            Number = this.partySize.Value;
            this.Close();
        }
    }
}
