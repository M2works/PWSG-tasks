using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Puzzle
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            
            base.OnKeyDown(e);

            if (e.Modifiers == Keys.None && e.KeyCode == Keys.Escape)
            {
                Cancel_Click(null, e);
            }
            if (e.Modifiers == Keys.None && e.KeyCode == Keys.Enter)
            {
                OK_Click(null, e);
            }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            LiT.life = (int)LifesNUD.Value;
            LiT.time = (int)TimeNUD.Value;
            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
