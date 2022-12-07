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
    public partial class CreateStaffLineItem : MaterialForm
    {
        public CreateStaffLineItem()
        {
            InitializeComponent();

            // Create a material theme manager and add the form to manage (this)
            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            
            // Configure color schema
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Orange400, Primary.Orange500,
                Primary.Orange500, Accent.Orange100,
                TextShade.WHITE
            );
        }
    }
}
