using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PWSG___lab6
{
    public partial class Form2 : Form
    {
        public byte slider1Value, slider2Value, slider3Value;
        public string colorToReturn;
        
        private void button1_Click(object sender, EventArgs e)
        {
            Wind1.name = textBox1.Text;
            Wind1.color = pictureBox1.BackColor;
            if (Wind1.isCircle)
            {
                bool isPossible = int.TryParse(textBox2.Text, out Wind1.radius);
                if (!isPossible)
                {
                    MessageBox.Show("Wrong input!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
            }
            DialogResult=DialogResult.OK;
            Close();
        }

        private void PaintPicturebox(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.FromArgb(trackBar1.Value, trackBar3.Value, trackBar2.Value);
        }

        public Form2()
        {
            InitializeComponent();
            Tag = Wind1.obiekt;
            if (Wind1.isCircle)
            {
                textBox2.Visible = true;
                label5.Visible = true;
                Wind1.Circle c = (Wind1.Circle)Tag;
                textBox2.Text = (c.Rect.Width/2).ToString();
            }
            trackBar1.Value = slider1Value;
            trackBar2.Value = slider2Value;
            trackBar3.Value = slider3Value;
            Color color = Color.FromArgb(trackBar1.Value, trackBar3.Value, trackBar2.Value);
            pictureBox1.BackColor = color;
            textBox1.Text = Wind1.name;
        }


    }
}
