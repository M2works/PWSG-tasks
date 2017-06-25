using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace PWSG___lab6
{
    public partial class Wind1 : Form
    {

        public abstract class shape
        {
            public enum Kształt { Linia, Koło}
            public Kształt Shape { get; set; }
            public Pen Pen { get; set; }
        }

        public class Line : shape
        {
            public Line(Tuple<Point,Point> ends, Pen pen)
            {
                Shape = Kształt.Linia;
                Ends = ends;
                Pen = pen;
            }
            public static bool operator ==(Line c1, Line c2)
            {
                if (c1.Pen.Equals(c2.Pen) && c1.Ends.Equals(c2.Ends) && c1.Shape.Equals(c2.Shape))
                    return true;
                return false;
            }

            public static bool operator !=(Line c1, Line c2)
            {
                if (!(c1 == c2))
                    return true;
                return false;
            }
            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            public Tuple<Point, Point> Ends { get; set; }
        }

        public class Circle : shape
        {
            public Circle(Rectangle rect, Pen pen)
            {
                Shape = Kształt.Koło;
                Rect = rect;
                Pen = pen;
            }
            public static bool operator ==(Circle c1, Circle c2)
            {
                if (c1.Pen.Color.Equals(c2.Pen.Color))
                    if(c1.Rect.Equals(c2.Rect))
                        if(c1.Shape.Equals(c2.Shape))
                    return true;
                return false;
            }

            public static bool operator !=(Circle c1, Circle c2)
            {
                if (!(c1 == c2))
                    return true;
                return false;
            }
            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public Rectangle Rect { get; set; }
        }
        public class MyPanel : System.Windows.Forms.Panel
        {
            public MyPanel()
            {
                DoubleBuffered = true;
                //this.SetStyle(
                //    System.Windows.Forms.ControlStyles.UserPaint |
                //    System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                //    System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
                //    true);
            }
        }
        public Wind1()
        {
            InitializeComponent();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;

            pb1.Dock = DockStyle.Fill;
            pb1.BorderStyle = BorderStyle.FixedSingle;
            tableLayoutPanel1.Controls.Add(pb1);
            tableLayoutPanel1.SetCellPosition(pb1, new TableLayoutPanelCellPosition(0,0));

            pb2.Dock = DockStyle.Fill;
            pb2.BorderStyle = BorderStyle.FixedSingle;
            tableLayoutPanel2.Controls.Add(pb2);
            tableLayoutPanel2.SetCellPosition(pb2, new TableLayoutPanelCellPosition(0,1));
            tableLayoutPanel2.SetColumnSpan(pb2, 2);
            tableLayoutPanel2.SetRowSpan(pb2, 2);
            
            radioButton1.Checked = true;
            radioButton2.Checked = false;

            Bitmap bm = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            Bitmap bm2 = new Bitmap(pb1.Width, pb1.Height);
            Bitmap bm3 = new Bitmap(pb2.Width, pb2.Height);
            pictureBox2.Image = bm;
            pb1.BackgroundImage = bm2;
            pb2.BackgroundImage = bm3;
            
        }

        MyPanel mp1 = new MyPanel();
        MyPanel mp2 = new MyPanel();

        PictureBox pb1 = new PictureBox();
        PictureBox pb2 = new PictureBox();
        
        public static Color color;
        public static string name;
        public static bool isCircle;
        public static int radius;
        public static shape obiekt;
        public static int actualIndex;
        public static bool clicked=false;

        byte slider1Value, slider2Value, slider3Value;

        private bool animationStarted = false;
        private bool needsRestarting = false;
        private int wholeTime = 20;
        private int maxTime = 20;
        private int timeSpan = 0;
        private bool wasPaused = false;
        private DateTime dt1;
        private bool run = false;
        private bool BWworks = false;

        private Point _origin = Point.Empty;
        private Point _terminus = Point.Empty;

        private Point leaveButton = Point.Empty;

        private List<Button> buttons = new List<Button>();

        private List<shape> shapes = new List<shape>();
        private List<shape> shapesEnd = new List<shape>();
        private List<shape> animationShapes = new List<shape>();

        bool firstClick = true;
        bool drawLines = true;
        Pen selectedColor = new Pen(System.Drawing.Color.Black,2);
        int circleIndex = 0, lineIndex=0;

        protected void OnMouseDown(object sender, MouseEventArgs e)
        {
            Graphics g = Graphics.FromImage(pictureBox2.Image);
            Graphics g3 = Graphics.FromImage(pb1.BackgroundImage);
            Graphics g4 = Graphics.FromImage(pb2.BackgroundImage);
            if (!firstClick && drawLines)
            {
                _terminus = e.Location;
                firstClick = true;
                
                shapes.Add(new Line(new Tuple<Point, Point>(_origin, _terminus), selectedColor));                
                shapesEnd.Add(new Line(new Tuple<Point, Point>(_origin, _terminus), selectedColor));
                animationShapes.Add(new Line(new Tuple<Point, Point>(_origin, _terminus), selectedColor));

                g.DrawLine(selectedColor, _origin, _terminus);
                g3.DrawLine(selectedColor, _origin, _terminus);
                g4.DrawLine(selectedColor, _origin, _terminus);
                String[] str = { String.Format("Line {0}", lineIndex++), String.Format("{0}", selectedColor.Color)};
                ListViewItem lvi = new ListViewItem(str);
                lvi.Tag = new Line(new Tuple<Point, Point>(_origin, _terminus), selectedColor);
                listView1.Items.Add(lvi);
                listView2.Items.Add((ListViewItem)lvi.Clone());
                listView1.Items[listView1.Items.Count - 1].Tag = new Line(new Tuple<Point, Point>(_origin, _terminus), selectedColor);
            }
            else
            {
                if(!firstClick)
                {
                    _terminus = e.Location;
                    firstClick = true;
                    int length = (int)Math.Sqrt(Math.Abs(_origin.X - _terminus.X) * Math.Abs(_origin.X - _terminus.X) + Math.Abs(_origin.Y - _terminus.Y) * Math.Abs(_origin.Y - _terminus.Y));
                    Size size = new Size(2 * length, 2 * length);
                    Rectangle rect = new Rectangle(new Point(_origin.X - (int)length, _origin.Y - (int)length), size);
                    g.DrawEllipse(selectedColor, rect);
                    g3.DrawEllipse(selectedColor, rect);
                    g4.DrawEllipse(selectedColor, rect);


                    shapes.Add(new Circle(rect, selectedColor));
                    shapesEnd.Add(new Circle(rect, selectedColor));
                    animationShapes.Add(new Circle(rect, selectedColor));

                    String[] str = { String.Format("Circle {0}", circleIndex++), String.Format("{0}", selectedColor.Color) };
                    ListViewItem lvi = new ListViewItem(str);
                    lvi.Tag = new Circle(rect, selectedColor);
                    listView1.Items.Add(lvi);
                    listView2.Items.Add((ListViewItem)lvi.Clone());
                    listView1.Items[listView1.Items.Count - 1].Tag= new Circle(rect, selectedColor);

                }
                else
                {
                    _origin = e.Location;
                    firstClick = false;
                }
            }
            pictureBox2.Invalidate();
        }


        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keys.Delete == e.KeyCode)
            {
                if ((ListView)sender == listView1)
                    deleteFromTheList(listView1, shapes);
                else
                    deleteFromTheList(listView2, shapesEnd);
            }
            pictureBox2.Invalidate();
            pb1.Invalidate();
        }

        private void deleteFromTheList(ListView lv,List<shape> shapesArg)
        {
            List<shape> toRemove = new List<shape>();
            List<ListViewItem> toRemoveLVI = new List<ListViewItem>();
            foreach (ListViewItem listViewItem in (lv.SelectedItems))
            {
                if (listViewItem.Tag is Line)
                {
                    Line line = (Line)listViewItem.Tag;
                    foreach (shape sth in shapesArg)
                    {
                        if (sth is Line)
                        {
                            Line l = (Line)sth;
                            if (line == l)
                            {
                                toRemove.Add(l);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Circle cc = (Circle)listViewItem.Tag;
                    foreach (shape sth in shapesArg)
                    {
                        if (sth is Circle)
                        {
                            Circle c = (Circle)sth;
                            if (cc == c)
                            {
                                toRemove.Add(c);
                                break;
                            }
                        }
                    }
                }
                toRemoveLVI.Add(listViewItem);
            }

            for (int i = 0; i < toRemoveLVI.Count; i++)
            {
                int index = lv.Items.IndexOf(toRemoveLVI[i]);

                listView1.Items.RemoveAt(index);
                listView2.Items.RemoveAt(index);
            }

            for (int i = 0; i < toRemove.Count; i++)
            {
                int index = shapesArg.IndexOf(toRemove[i]);

                shapes.RemoveAt(index);
                shapesEnd.RemoveAt(index);
            }

            Graphics g = Graphics.FromImage(pictureBox2.Image);
            Graphics g3 = Graphics.FromImage(pb1.BackgroundImage);
            Graphics g4 = Graphics.FromImage(pb2.BackgroundImage);

            Color color = System.Drawing.Color.White;
            g.Clear(color);
            g3.Clear(color);
            g4.Clear(color);
            for (int i = 0; i < shapesArg.Count; i++)
            {
                if (shapesArg[i].Shape == shape.Kształt.Linia)
                {
                    Line l = shapes[i] as Line;
                    Line l2 = shapesEnd[i] as Line;
                    g.DrawLine(l.Pen, l.Ends.Item1, l.Ends.Item2);
                    g3.DrawLine(l2.Pen, l2.Ends.Item1, l2.Ends.Item2);
                    g4.DrawLine(l.Pen, l.Ends.Item1, l.Ends.Item2);
                }
                else
                {
                    Circle c = shapes[i] as Circle;
                    Circle c2 = shapesEnd[i] as Circle;
                    g.DrawEllipse(c.Pen, c.Rect);
                    g3.DrawEllipse(c2.Pen, c2.Rect);
                    g4.DrawEllipse(c.Pen, c.Rect);
                }
            }
            animationShapes = new List<shape>(shapes);
        }

        protected void OnMouseMove(object sender, MouseEventArgs e)
        {
            Graphics g = Graphics.FromImage(pictureBox2.Image);
            Graphics g3 = Graphics.FromImage(pb1.BackgroundImage);
            Graphics g4 = Graphics.FromImage(pb2.BackgroundImage);
            if (!firstClick)
            {

                Color color = System.Drawing.Color.White;
                g.Clear(color);
                g3.Clear(color);
                g4.Clear(color);
                for (int i = 0; i < shapes.Count; i++)
                {
                    if (shapes[i].Shape == shape.Kształt.Linia)
                    {
                        Line l = shapes[i] as Line;
                        g.DrawLine(l.Pen, l.Ends.Item1, l.Ends.Item2);
                        g3.DrawLine(l.Pen, l.Ends.Item1, l.Ends.Item2);
                        g4.DrawLine(l.Pen, l.Ends.Item1, l.Ends.Item2);
                    }
                    else
                    {
                        Circle c = shapes[i] as Circle;
                        g.DrawEllipse(c.Pen, c.Rect);
                        g3.DrawEllipse(c.Pen, c.Rect);
                        g4.DrawEllipse(c.Pen, c.Rect);
                    }
                }
                _terminus = e.Location;
                if(drawLines)
                    g.DrawLine(new Pen(System.Drawing.Color.Red,2), _origin, _terminus);
                else
                {
                    int length = (int)Math.Sqrt(Math.Abs(_origin.X - _terminus.X) * Math.Abs(_origin.X - _terminus.X) + Math.Abs(_origin.Y - _terminus.Y) * Math.Abs(_origin.Y - _terminus.Y));
                    Size size = new Size(2 * length, 2 * length);
                    Rectangle rect = new Rectangle(new Point(_origin.X - (int)length, _origin.Y - (int)length), size);
                    g.DrawEllipse(new Pen(System.Drawing.Color.Red, 2), rect);
                }
            }
            pictureBox2.Invalidate();

        }
        private void Picker_click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            // See if user pressed ok.
            if (result == DialogResult.OK)
            {
                pictureBox1.BackColor = colorDialog1.Color;
                selectedColor = new Pen(colorDialog1.Color,2);
            }
        }

        private void radioButton1_Click(object sender, MouseEventArgs e)
        {
            if(!drawLines)
            {
                drawLines = true;
                if (radioButton2.Checked)
                    radioButton2.Checked = false;
                radioButton1.Checked = true;
            }
        }
        private void radioButton2_Click(object sender, MouseEventArgs e)
        {
            if (drawLines)
            {
                drawLines = false;
                if (radioButton1.Checked)
                    radioButton1.Checked = false;
                radioButton2.Checked = true;
            }
        }


        private void listview1_activate(object sender, EventArgs e)
        {            
            int index=0;
            isCircle = false;
            foreach (ListViewItem LVI in ((ListView)sender).SelectedItems)
            {
                name = LVI.SubItems[0].Text;
                if (LVI.Tag is Circle)
                {
                    isCircle = true;
                    Circle c = (Circle)LVI.Tag;
                    foreach (shape sth in shapesEnd)
                    {
                        if (sth is Circle)
                        {
                            Circle cc = (Circle)sth;
                            if (c == cc)
                                break;
                        }
                        index++;
                    }
                }
                else
                {
                    Line line = (Line)LVI.Tag;
                    foreach (shape sth in shapesEnd)
                    {
                        if (sth is Line)
                        {
                            Line l = (Line)sth;
                            if (line == l)
                                break;
                        }
                        index++;
                    }
                }
            }

            if(isCircle)
            {
                Circle c=(Circle)shapesEnd[index];
                obiekt = new Circle(new Rectangle(new Point(c.Rect.Top, c.Rect.Left), new Size(c.Rect.Width, c.Rect.Height)),new Pen(shapesEnd[index].Pen.Color,2));
            }
            else
            {
                Line l = (Line)shapesEnd[index];
                obiekt = new Line(new Tuple<Point, Point>(l.Ends.Item1, l.Ends.Item2), new Pen(l.Pen.Color,2));
            }

            obiekt = shapesEnd[index];
            Form2 frm = new Form2();
            Pen pen = shapesEnd[index].Pen;
            frm.slider1Value = pen.Color.R;
            frm.slider2Value = pen.Color.B;
            frm.slider3Value = pen.Color.G;

            DialogResult dr=frm.ShowDialog();
            if (dr != DialogResult.Cancel && dr!=DialogResult.Abort)
            {
                string[] str = new string[] { name, shapes[index].Pen.Color.ToString() };
                listView1.Items[index] = new ListViewItem(str);

                string[] str2 = new string[] { name, color.ToString() };
                listView2.Items[index] = new ListViewItem(str2);
                if (isCircle)
                {
                    Circle c = (Circle)shapesEnd[index];
                    listView2.Items[index].Tag = new Circle(new Rectangle(new Point(c.Rect.Left + c.Rect.Width / 2 - radius, c.Rect.Top + c.Rect.Height / 2 - radius), new Size(2 * radius, 2 * radius)), new Pen(color, 2));
                    shapesEnd[index] = new Circle(new Rectangle(new Point(c.Rect.Left + c.Rect.Width / 2 - radius, c.Rect.Top + c.Rect.Height / 2 - radius), new Size(2 * radius, 2 * radius)), new Pen(color,2));
                }
                else
                {
                    shapesEnd[index].Pen = new Pen(color,2);
                    listView2.Items[index].Tag = shapesEnd[index];
                }

                Graphics g3 = Graphics.FromImage(pb1.BackgroundImage);
                Color backColor = System.Drawing.Color.White;
                g3.Clear(backColor);
                for (int i = 0; i < shapesEnd.Count; i++)
                {
                    if (shapesEnd[i].Shape == shape.Kształt.Linia)
                    {
                        Line l = shapes[i] as Line;
                        Line l2 = shapesEnd[i] as Line;
                        g3.DrawLine(l2.Pen, l2.Ends.Item1, l2.Ends.Item2);
                    }
                    else
                    {
                        Circle c = shapes[i] as Circle;
                        Circle c2 = shapesEnd[i] as Circle;
                        g3.DrawEllipse(c2.Pen, c2.Rect);
                    }
                }
                pb1.Invalidate();
            }
        }

        private void selectFunction(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            pb1.Controls.Clear();
            buttons.Clear();
            pb1.Invalidate();
            Graphics g3 = Graphics.FromImage(pb1.BackgroundImage);
            foreach (ListViewItem LVI in ((ListView)sender).SelectedItems)
            {
                if (LVI.Tag is Circle)
                {
                    Circle c = (Circle)LVI.Tag;
                    Rectangle rect = c.Rect;
                    Button bn = new Button();
                    pb1.Controls.Add(bn);
                    bn.Location = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
                    bn.Size = new Size(8, 8);
                    bn.BackColor = System.Drawing.Color.DarkViolet;
                    bn.MouseEnter += smallbutton_enter;
                    bn.MouseLeave += smallbutton_leave;
                    bn.MouseClick += smallbutton_click;
                    bn.MouseDown += smallbutton_MouseDown;
                    bn.MouseMove += smallbutton_MouseMove;
                    bn.MouseUp += smallbutton_MouseUp;
                    bn.LocationChanged += smallbutton_locChange;
                    bn.FlatStyle = FlatStyle.Flat;
                    bn.FlatAppearance.BorderSize = 2;
                    bn.FlatAppearance.BorderColor = System.Drawing.Color.DarkViolet;
                    bn.BringToFront();
                    int index = 0;
                    for(int i=0;i<shapesEnd.Count;i++)
                        if(shapesEnd[i] is Circle)
                            if (c==shapesEnd[i] as Circle)
                            {
                                index = i;
                                break;
                            }
                    bn.Tag = index;
                    bn.Show();
                    buttons.Add(bn);
                }
                else
                {
                    Line l = (Line)LVI.Tag;
                    Button bn1 = new Button();
                    Button bn2 = new Button();
                    pb1.Controls.Add(bn1);
                    pb1.Controls.Add(bn2);
                    bn1.Location = new Point(l.Ends.Item1.X-4,l.Ends.Item1.Y-4);
                    bn2.Location = new Point(l.Ends.Item2.X-4, l.Ends.Item2.Y-4);
                    bn1.Size = new Size(8, 8);
                    bn2.Size = new Size(8, 8);
                    bn1.BackColor = System.Drawing.Color.DarkViolet;
                    bn2.BackColor = System.Drawing.Color.DarkViolet;
                    bn1.MouseEnter += smallbutton_enter;
                    bn1.MouseLeave += smallbutton_leave;
                    bn1.MouseClick += smallbutton_click;
                    bn1.MouseDown += smallbutton_MouseDown;
                    bn1.MouseMove += smallbutton_MouseMove;
                    bn1.MouseUp += smallbutton_MouseUp;
                    bn1.LocationChanged += smallbutton_locChange;
                    bn2.MouseEnter += smallbutton_enter;
                    bn2.MouseLeave += smallbutton_leave;
                    bn2.MouseClick += smallbutton_click;
                    bn2.MouseDown += smallbutton_MouseDown;
                    bn2.MouseMove += smallbutton_MouseMove;
                    bn2.MouseUp += smallbutton_MouseUp;
                    bn2.LocationChanged += smallbutton_locChange;
                    bn1.FlatStyle = FlatStyle.Flat;
                    bn2.FlatStyle = FlatStyle.Flat;
                    bn1.FlatAppearance.BorderSize = 2;
                    bn1.FlatAppearance.BorderColor = System.Drawing.Color.DarkViolet;
                    int index = 0;
                    for (int i = 0; i < shapesEnd.Count; i++)
                        if(shapesEnd[i] is Line)
                            if (l == shapesEnd[i] as Line)
                            {
                                index = i;
                                break;
                            }
                    bn1.Tag = index;
                    bn1.BringToFront();
                    bn1.Show();
                    buttons.Add(bn1);
                    bn2.FlatAppearance.BorderSize = 2;
                    bn2.FlatAppearance.BorderColor = System.Drawing.Color.DarkViolet;
                    for (int i = 0; i < shapesEnd.Count; i++)
                        if (shapesEnd[i] is Line)
                            if (l == shapesEnd[i] as Line)
                            {
                                index = i;
                                break;
                            }
                    bn2.Tag = index;
                    bn2.BringToFront();
                    bn2.Show();
                    buttons.Add(bn2);
                }
            }
        }
        private void smallbutton_enter(object sender, EventArgs e)
        {
            Button bn = (Button)sender;
            bn.BackColor = System.Drawing.Color.MediumVioletRed;
        }

        private void smallbutton_leave(object sender, EventArgs e)
        {
            Button bn = (Button)sender;
            bn.BackColor = System.Drawing.Color.DarkViolet;
        }

        private void smallbutton_click(object sender, MouseEventArgs e)
        {
            Button bn = (Button)sender;
            bn.BackColor = System.Drawing.Color.Violet;
        }

        private void smallbutton_locChange(object sender, EventArgs e)
        {
            Button bn = (Button)sender;
            int index = (int)bn.Tag;

            if(shapesEnd[index] is Line)
            {
                Line line = shapesEnd[index] as Line;
                line.Ends = new Tuple<Point,Point>(new Point(buttons[0].Location.X + 4, buttons[0].Location.Y + 4),new Point(buttons[1].Location.X + 4, buttons[1].Location.Y + 4));
                shapesEnd[index] = line;
                listView2.Items[index].Tag = line;
                
                Graphics g3 = Graphics.FromImage(pb1.BackgroundImage);

                Color color = System.Drawing.Color.White;
                g3.Clear(color);
                for (int i = 0; i < shapesEnd.Count; i++)
                {
                    if (shapesEnd[i].Shape == shape.Kształt.Linia)
                    {
                        Line l2 = shapesEnd[i] as Line;
                        g3.DrawLine(l2.Pen, l2.Ends.Item1, l2.Ends.Item2);
                    }
                    else
                    {
                        Circle c2 = shapesEnd[i] as Circle;
                        g3.DrawEllipse(c2.Pen, c2.Rect);
                    }
                }
            }
            else
            {
                Circle circle= shapesEnd[index] as Circle;
                circle.Rect = new Rectangle(new Point(buttons[0].Location.X + 4 - circle.Rect.Width / 2, buttons[0].Location.Y + 4 - circle.Rect.Height / 2), new Size(circle.Rect.Width,circle.Rect.Height));
                shapesEnd[index] = circle;
                listView2.Items[index].Tag = circle;
                
                Graphics g3 = Graphics.FromImage(pb1.BackgroundImage);

                Color color = System.Drawing.Color.White;
                g3.Clear(color);
                for (int i = 0; i < shapesEnd.Count; i++)
                {
                    if (shapesEnd[i].Shape == shape.Kształt.Linia)
                    {
                        Line l2 = shapesEnd[i] as Line;
                        g3.DrawLine(l2.Pen, l2.Ends.Item1, l2.Ends.Item2);
                    }
                    else
                    {
                        Circle c2 = shapesEnd[i] as Circle;
                        g3.DrawEllipse(c2.Pen, c2.Rect);
                    }
                }
            }
            pb1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Start")
            {
                animationStarted = true;
                button1.Text = "Pause";
                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                button1.Text = "Start";
                if (BWworks)
                {
                    backgroundWorker1.CancelAsync();
                    BWworks = false;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(animationStarted)
                needsRestarting = true;
            button1.Text = "Start";
            label3.Text = "FPS";
            if (BWworks)
            {
                backgroundWorker1.CancelAsync();
                BWworks = false;
            }

            Graphics g4 = Graphics.FromImage(pb2.BackgroundImage);
            Color toClean = System.Drawing.Color.White;
            g4.Clear(toClean);

            for (int k = 0; k < shapes.Count; k++)
            {
                if (shapes[k].Shape == shape.Kształt.Linia)
                {
                    Line l2 = shapes[k] as Line;
                    g4.DrawLine(l2.Pen, l2.Ends.Item1, l2.Ends.Item2);
                }
                else
                {
                    Circle c2 = shapes[k] as Circle;
                    g4.DrawEllipse(c2.Pen, c2.Rect);
                }
            }
            pb2.Invalidate();
        }

        private void smallbutton_MouseDown(object sender, MouseEventArgs e)
        {
                clicked = true;
        }

        private void smallbutton_MouseMove(object sender, MouseEventArgs e)
        {
            if (clicked)
            {
                Button bn = (Button)sender;
                bn.Location = new Point(e.X + bn.Left, e.Y + bn.Top);
            }
        }

        private void smallbutton_MouseUp(object sender, MouseEventArgs e)
        {
            clicked = false;
        }

        private void changeWholeTime(object sender, EventArgs e)
        {
            if (BWworks)
            {
                backgroundWorker1.CancelAsync();
                BWworks = false;
            }
            wholeTime = trackBar1.Value / 100 * maxTime;
            wasPaused = true;
        }

        // This event handler is where the time-consuming work is done.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BWworks = true;
            BackgroundWorker worker = sender as BackgroundWorker;
            Graphics g4 = Graphics.FromImage(pb2.BackgroundImage);
            DateTime startingTime = DateTime.Now;
            dt1 = DateTime.Now;
            TimeSpan ts = (dt1 - startingTime);
            if (!wasPaused)
                timeSpan = (int)ts.Seconds;

            while (timeSpan < wholeTime)
            {
                for (int j = 0; j < shapes.Count; j++)
                {
                    if (shapes[j] is Line)
                    {
                        Line l1 = (Line)shapes[j];
                        Line l2 = (Line)shapesEnd[j];

                        Tuple<Point, Point> createdTuple = new Tuple<Point, Point>(new Point((l1.Ends.Item1.X * (wholeTime - timeSpan) + l2.Ends.Item1.X * timeSpan) / wholeTime, (int)(l1.Ends.Item1.Y * (wholeTime - timeSpan) + l2.Ends.Item1.Y * timeSpan) / wholeTime), new Point((l1.Ends.Item2.X * (wholeTime - timeSpan) + l2.Ends.Item2.X * timeSpan) / wholeTime, (l1.Ends.Item2.Y * (wholeTime - timeSpan) + l2.Ends.Item2.Y * timeSpan) / wholeTime));

                        int red1 = l1.Pen.Color.R;
                        int green1 = l1.Pen.Color.G;
                        int blue1 = l1.Pen.Color.B;

                        int red2 = l2.Pen.Color.R;
                        int green2 = l2.Pen.Color.G;
                        int blue2 = l2.Pen.Color.B;

                        Color mixedColor = System.Drawing.Color.FromArgb((red1 * (wholeTime - timeSpan) + red2 * timeSpan) / wholeTime, (green1 * (wholeTime - timeSpan) + green2 * timeSpan) / wholeTime, (blue1 * (wholeTime - timeSpan) + blue2 * timeSpan) / wholeTime);

                        Pen pen = new Pen(mixedColor, 2);

                        animationShapes[j] = new Line(createdTuple, pen);
                    }

                    if (shapesEnd[j] is Circle)
                    {
                        Circle c1 = (Circle)shapes[j];
                        Circle c2 = (Circle)shapesEnd[j];

                        Rectangle rect = new Rectangle(new Point((c1.Rect.Top * (wholeTime - timeSpan) + c2.Rect.Top * timeSpan) / wholeTime, (c1.Rect.Left * (wholeTime - timeSpan) + c2.Rect.Left * timeSpan) / wholeTime), new Size((c1.Rect.Size.Width * (wholeTime - timeSpan) + c2.Rect.Size.Width * timeSpan) / wholeTime, (c1.Rect.Size.Height * (wholeTime - timeSpan) + c2.Rect.Size.Height * timeSpan) / wholeTime));

                        int red1 = c1.Pen.Color.R;
                        int green1 = c1.Pen.Color.G;
                        int blue1 = c1.Pen.Color.B;

                        int red2 = c2.Pen.Color.R;
                        int green2 = c2.Pen.Color.G;
                        int blue2 = c2.Pen.Color.B;

                        Color mixedColor = System.Drawing.Color.FromArgb((red1 * (wholeTime - timeSpan) + red2 * timeSpan) / wholeTime, (green1 * (wholeTime - timeSpan) + green2 * timeSpan) / wholeTime, (blue1 * (wholeTime - timeSpan) + blue2 * timeSpan) / wholeTime);
                        Pen pen = new Pen(mixedColor, 2);

                        animationShapes[j] = new Circle(rect, pen);
                    }
                }

                dt1 = DateTime.Now;
                ts = (dt1 - startingTime);
                timeSpan = (int)ts.Seconds;
                worker.ReportProgress(timeSpan / wholeTime * 100);

                Color toClean = System.Drawing.Color.White;
                g4.Clear(toClean);
                for (int k = 0; k < animationShapes.Count; k++)
                {
                    if (animationShapes[k].Shape == shape.Kształt.Linia)
                    {
                        Line l2 = animationShapes[k] as Line;
                        g4.DrawLine(l2.Pen, l2.Ends.Item1, l2.Ends.Item2);
                    }
                    else
                    {
                        Circle c2 = animationShapes[k] as Circle;
                        g4.DrawEllipse(c2.Pen, c2.Rect);
                    }
                }
                pb2.Invalidate();
            }
        }

        // This event handler updates the progress.
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            double diff = (DateTime.Now - dt1).Milliseconds;
            double frequency = 1000 / diff;
            Graphics g4 = Graphics.FromImage(pb2.BackgroundImage);
            label3.Text = "FPS: " + frequency.ToString();
        }

        // This event handler deals with the results of the background operation.
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timeSpan = 0;
            wasPaused = false;
            BWworks = false;
        }
    }
}
