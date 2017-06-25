using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;                         
using System.Runtime.Serialization;

namespace PWSG___lab3
{
    public partial class Form1 : Form
    {
        bool firstLoaded = false;
        bool secondLoaded = false;
        double trackbarValue;
        int operationsCount = 0;
        Bitmap bitmap1,bitmap2;
        bool upLoading = false;
        bool downLoading = false;
        bool justCreated = false;
        int imagesInSession = 0;
        bool saved = false;
        List<string> paths = new List<string>();
        string path;
        const string fileName = "dane_xml";
        PictureBox active=null;
        XmlDocument xd = new XmlDocument();
        

        public Form1()
        {
            InitializeComponent();
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker2.DoWork += backgroundWorker1_DoWork;
            this.MaximumSize = new Size(Screen.PrimaryScreen.Bounds.Width, 380);
            this.MinimumSize = new Size(353, 380);
            imageList1.ImageSize = new Size(150, 150);
            flowLayoutPanel1.DragDrop += new DragEventHandler(listView1_DragDrop);
            flowLayoutPanel1.DragEnter += new DragEventHandler(listView1_DragEnter);
            //prepareImages();

        }

        void listView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        void listView1_DragDrop(object sender, DragEventArgs e)
        {
            String[] imagePaths = (String[])e.Data.GetData(DataFormats.FileDrop);
            for (int i = 0; i < imagePaths.Length; i++)
                addComponent(imagePaths[i]);

        }

        public void addComponent(string path)
        {
            for (int i = 0; i < paths.Count; i++)
                if (paths[i] == path)
                    return;
            PictureBox pb = new PictureBox();
            Bitmap bm = new Bitmap(path);
            pb.Image = bm;
            pb.Width = pb.Height = 150;
            pb.Padding = new Padding(5, 5, 5, 5);
            pb.BackColor = Color.White;
            pb.SizeMode = PictureBoxSizeMode.StretchImage;
            pb.Click += activatePb;
            flowLayoutPanel1.Controls.Add(pb);
            paths.Add(path);
        }
        
        



        private void activatePb(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            if (pb.BackColor == Color.White)
            {
                if (active == null)
                {
                    pb.BackColor = Color.Orange;
                    active = pb;
                }
                else
                {
                    active.BackColor = Color.White;
                    active = pb;
                    active.BackColor = Color.Orange;
                }
            }
            else
            {
                pb.BackColor = Color.White;
                active=null;
            }
        }

        private void getPaths()
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<string>));
            FileStream fs = new FileStream("dane_xml", FileMode.Open);
            paths = (List<string>)xs.Deserialize(fs);
            fs.Close();
            Console.WriteLine();
            Console.WriteLine("Serializacja xml");
        }
        private void KeyEvent(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                flowLayoutPanel1.Controls.Remove(active);
                active = null;
            }

            if (e.KeyCode == Keys.F12)
            {
                if (!firstLoaded)
                {
                    Bitmap screenshot = new Bitmap(SystemInformation.VirtualScreen.Width,
                               SystemInformation.VirtualScreen.Height);
                    Graphics screenGraph = Graphics.FromImage(screenshot);
                    screenGraph.CopyFromScreen(SystemInformation.VirtualScreen.X,
                                               SystemInformation.VirtualScreen.Y,
                                               0,
                                               0,
                                               SystemInformation.VirtualScreen.Size,
                                               CopyPixelOperation.SourceCopy);

                    screenshot.Save("Screenshot.png", System.Drawing.Imaging.ImageFormat.Png);
                    pictureBox1.Image = screenshot;
                    pictureBox1.Show();
                }
                else
                {
                    Bitmap screenshot = new Bitmap(SystemInformation.VirtualScreen.Width,
                               SystemInformation.VirtualScreen.Height);
                    Graphics screenGraph = Graphics.FromImage(screenshot);
                    screenGraph.CopyFromScreen(SystemInformation.VirtualScreen.X,
                                               SystemInformation.VirtualScreen.Y,
                                               0,
                                               0,
                                               SystemInformation.VirtualScreen.Size,
                                               CopyPixelOperation.SourceCopy);

                    screenshot.Save("Screenshot.png", System.Drawing.Imaging.ImageFormat.Png);
                    pictureBox2.Image = screenshot;
                    pictureBox2.Show();
                }
                firstLoaded = true;
                if (firstLoaded && secondLoaded)
                    button1.Enabled = true;
            }
        }

        private void trackBar_mouseEnter(object sender, EventArgs e)
        {
            TrackBar tb = (TrackBar)sender;
            tb.Cursor = Cursors.IBeam;
        }

        private void trackBar_mouseLeave(object sender, EventArgs e)
        {
            TrackBar tb = (TrackBar)sender;
            tb.Cursor = Cursors.Hand;
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (operationsCount < 2)
            {
                int localCount;
                operationsCount++;

                Bitmap bm1 = (Bitmap)pictureBox1.Image.Clone();
                Bitmap bm2 = (Bitmap)pictureBox2.Image.Clone();

                BackgroundWorker BGW = (BackgroundWorker)sender;

                if (BGW == backgroundWorker1)
                {
                    localCount = 1;
                    bitmap1 = BitwiseBlend(bm1, bm2, localCount);
                }
                else
                {
                    localCount = 2;
                    bitmap2 = BitwiseBlend(bm1, bm2, localCount);
                }

                //if (operationsCount == 2) { }
                    //button1.Enabled = false;
            }
        }



        public Bitmap BitwiseBlend(Bitmap sourceBitmap, Bitmap blendBitmap, int localCount)
        {
            int height = (sourceBitmap.Height < blendBitmap.Height) ? sourceBitmap.Height : blendBitmap.Height;
            int width = (sourceBitmap.Width < blendBitmap.Width) ? sourceBitmap.Width : blendBitmap.Width;

            Bitmap bmp = new Bitmap(width,height);
            double alpha = trackbarValue;

            int pixelInTotal = width * height;
            int currentPixel = 0;
            int parts = 0;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    if(currentPixel*10>pixelInTotal)
                    {
                        parts++;
                        currentPixel = 0;
                        if (localCount == 1)
                            backgroundWorker1.ReportProgress(parts * 10);

                        else
                            backgroundWorker2.ReportProgress(parts * 10);
                        Thread.Sleep(500);
                    }
                    Color c1 = sourceBitmap.GetPixel(x, y);
                    Color c2 = blendBitmap.GetPixel(x, y);
                    Color gotColor = Color.FromArgb((int)(alpha * c1.R + (1 - alpha) * c2.R), (int)(alpha * c1.G + (1 - alpha) * c2.G), (int)(alpha * c1.B + (1 - alpha) * c2.B));
                    bmp.SetPixel(x, y, gotColor);
                    currentPixel++;
                }
            }

            return bmp;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar2.Value = e.ProgressPercentage;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (active != null)
            {
                if (pictureBox1 == (PictureBox)sender)
                {
                    pictureBox1.Image = (Image)active.Image.Clone();
                }
                else
                {
                    pictureBox2.Image = (Image)active.Image.Clone();
                }
            }
            else
            {
                PictureBox pb = (PictureBox)sender;
                OpenFileDialog dlg = new OpenFileDialog();

                dlg.Filter = "Images Files (*.jpg,*.bmp,*.png)|*.jpg;*.bmp;*.png|All files (*.*)|*.*";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string filetype = System.IO.Path.GetExtension(dlg.FileName);
                    if (filetype != ".bmp" && filetype != ".jpg" && filetype != ".png")
                    {
                        MessageBox.Show("Error!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Bitmap bm = new Bitmap(dlg.FileName);
                    pb.Image = bm;
                    pb.Show();
                    if (pb == pictureBox1)
                        firstLoaded = true;
                    else
                        secondLoaded = true;
                }

                if (firstLoaded && secondLoaded)
                    button1.Enabled = true;
                dlg.Dispose();
            }
        }

        private void prepareImages()
        {
            if (File.Exists("dane_xml"))
            {
                try
                {
                    xd.Load("dane_xml");
                }
                catch (Exception e)
                {
                    xd.LoadXml("");
                }
            }

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button1.Enabled = true;
            operationsCount--;
            BackgroundWorker BGW = (BackgroundWorker)sender;
            if (backgroundWorker1 == BGW)
            {
                progressBar1.Visible = false;
                progressBar1.Value = 0;
                BGW_createform(bitmap1);
                upLoading = false;
            }
            else
            {
                progressBar2.Visible = false;
                progressBar2.Value = 0;
                BGW_createform(bitmap2);
                downLoading = false;
            }
            if (operationsCount == 0)
                label2.Visible = false;
        }

        private void BGW_createform(Bitmap bm)
        {
            Form fm = new Form();
            PictureBox pb = new PictureBox();
            pb.Dock = DockStyle.Fill;
            pb.Image = bm;
            
            pb.SizeMode = PictureBoxSizeMode.StretchImage;
            fm.Controls.Add(pb);
            fm.Text = "Image Window";
            fm.BackgroundImage = bm;
            fm.Location = new Point(100, 100);
            fm.ClientSize = new Size(bitmap1.Size.Width, bitmap1.Size.Height);
            pb.MouseClick += RightClick;
            saved = false;
            justCreated = true;
            fm.Show();
        }

        private void RightClick(object sender, MouseEventArgs mea)
        {
            if(mea.Button==MouseButtons.Right)
            {
                PictureBox pb = (PictureBox)sender;
                Bitmap bm = (Bitmap)pb.Image;
                MenuItem save = new MenuItem("Save");
                save.Tag = bm;
                save.Click += saveClick;
                MenuItem Atl = new MenuItem("Add to library");
                Atl.Tag = bm;
                Atl.Click += AtlClick;
                MenuItem[] menuItems = new MenuItem[]{save, Atl};

                ContextMenu buttonMenu = new ContextMenu(menuItems);
                buttonMenu.Show((Control)sender, mea.Location);
            }
        }

        private void AtlClick(object sender, EventArgs e)
        {
            if (justCreated)
            {
                MenuItem mi = (MenuItem)sender;
                Bitmap bm = (Bitmap)mi.Tag;

                string ds = DateTime.Now.ToString();
                ds = ds.Substring(0, 9) + ds.Substring(11, 2) + ds.Substring(14, 2) + ds.Substring(17, 2);
                bm.Save(ds);
                addComponent(ds);
                justCreated = false;
            }
            else
            {
                if (!saved)
                    MessageBox.Show("Please save the item first.");
                else
                {
                    addComponent(path);
                }
            }
        }

        private void saveClick(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "NewImage" + imagesInSession + ".bmp";
            dlg.Filter = "Images Files (*.JPG,*.BMP,*.PNG)|*.jpg;*.bmp;*.png|All files (*.*)|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                MenuItem mi = (MenuItem)sender;
                Bitmap bm = (Bitmap)mi.Tag;
                bm.Save(dlg.FileName);
                path = System.IO.Path.GetFullPath(dlg.FileName);
                saved = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            trackbarValue = (double)trackBar1.Value/10;
            if (operationsCount == 1)
            {                
                if (upLoading)
                {
                    downLoading = true;
                    this.progressBar2.Visible = true;
                    backgroundWorker2.RunWorkerAsync();
                }
                else
                {
                    upLoading = true;
                    this.progressBar1.Visible = true;
                    backgroundWorker1.RunWorkerAsync();
                }
                button1.Enabled = false;
            }
            if (operationsCount == 0)
            {
                this.progressBar1.Visible = true;
                this.label2.Visible = true;
                upLoading = true;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            TrackBar tb = (TrackBar)sender;
            trackbarValue = (double)tb.Value / 10;
        }
    }
}
