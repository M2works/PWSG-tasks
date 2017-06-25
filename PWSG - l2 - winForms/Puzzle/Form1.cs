using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;



namespace Puzzle
{
    
    public partial class Form1 : Form
    {
        private const int ROW_COUNT = 4;
        private const int COLUMN_COUNT = 4;
        

        //liczba aktywnych przycisków
        int[] labelsIR;
        int[] labelsIC;
        

        bool active = false;
        
        private DateTime? gameStart = null;

        bool[] values=new bool[30];

        private bool gameActive = true;

        //liczba aktywnych przycisków
        int activeCounter = 0;

        bool wasEdited = false;
        //liczba oznaczonych aktywnych przycisków
        int selectedCounter = 0;
        
        int timeLeft;
        static int points = 0;
        public static int currentLifes = 3;
        bool wasEnded = false;
        bool newGame = true;
        public void setTime(int _time)
        {
            LiT.time = _time;
        }
        public int getTime()
        {
            return LiT.time;
        }
        public void setLifes(int _lifes)
        {
            LiT.life = _lifes;
        }
        public int getLifes()
        {
            return LiT.life;
        }
        
        public Form1()
        {
            Random random = new Random();

            InitializeComponent();
            for (int i = 0; i < ROW_COUNT * COLUMN_COUNT; i++)
            {
                var btn = new Button();
                btn.Dock = DockStyle.Fill;
                btn.Text = "?";                
                btn.Font = new Font("Microsoft Sans Serif", 16);
                btn.BackColor = Color.RoyalBlue;
                btn.MouseEnter += btnCard_MouseEnter;
                btn.MouseLeave += btnCard_MouseLeave;
                btn.MouseDown += btnCard_Click;
                panel1.Controls.Add(btn);
            }
            
            labelsIC = new int[COLUMN_COUNT];
            labelsIR = new int[ROW_COUNT];
            lifesLabel.Text = "lifes: " + currentLifes;

            setTime(10);
            setLifes(3);

            //ustawienia jak dla trybu gry
            gameToolStripMenuItem.Checked = true;

            saveToolStripMenuItem.Enabled = false;
            openToolStripMenuItem.Enabled = true;
            settingsToolStripMenuItem.Enabled = true;
            newGameToolStripMenuItem.Enabled = true;
        }
        
        private void editButton_Click(object sender, EventArgs e)
        {
            MouseEventArgs ev = (MouseEventArgs)e;
            TableLayoutPanelCellPosition Pos = panel1.GetPositionFromControl((Control)sender);
            var btn = (Button)sender;
            if (ev.Button == MouseButtons.Right)
            {
                if (btn.BackColor == Color.Black)
                {
                    btn.BackColor = Color.White;
                    labelsIC[Pos.Row - 1]--;
                    btn.Tag = "passive";
                    labelsIR[Pos.Column - 1]--;
                }
            }
            if (ev.Button == MouseButtons.Left)
            {
                if (btn.BackColor == Color.White)
                {
                    labelsIC[Pos.Row - 1]++;
                    labelsIR[Pos.Column - 1]++;
                    btn.Tag = "active";
                    btn.BackColor = Color.Black;
                }
            }
        }

        private void btnCard_Click(object sender, EventArgs e)
        {
            if (active)
            {
                MouseEventArgs ev = (MouseEventArgs)e;
                TableLayoutPanelCellPosition Pos = panel1.GetPositionFromControl((Control)sender);
                var btn = (Button)sender;
                if (ev.Button == MouseButtons.Right)
                {
                    if (btn.BackColor != Color.Black)
                    {
                        btn.BackColor = Color.White;
                    }

                }
                if (ev.Button == MouseButtons.Left)
                {
                    if (btn.Tag.ToString() == "active")
                    {
                        bool ifWon = false;
                        if (btn.BackColor != Color.Black)
                        {
                            points += 50;
                            scoreLabel.Text = "score: " + points;
                            if((++selectedCounter)==activeCounter)
                            {
                                GameIsWon();
                                ifWon = true;
                            }
                        }
                        if(!ifWon)
                            btn.BackColor = Color.Black;
                    }
                    else
                    {
                    currentLifes-=1;
                    UpdateStatusInfo();
                        
                    string buttonString = btn.Text;
                    Color color = btn.BackColor;
                        
                    //btn.BackColor = Color.Red;
                    var timer = new Timer();
                    timer.Interval = 500;
                    timer.Tick += buttonTick;
                        if(currentLifes!=0)
                    changeToRed(btn,timer);
                    }

                }
            }
            else
            {
                if(wasEdited)
                {
                    MouseEventArgs ev = (MouseEventArgs)e;
                    TableLayoutPanelCellPosition Pos = panel1.GetPositionFromControl((Control)sender);
                    var btn = (Button)sender;
                    if (ev.Button == MouseButtons.Right)
                    {
                        if (btn.BackColor == Color.Black)
                        {
                            btn.BackColor = Color.White;
                            labelsIC[Pos.Row - 1]--;
                            btn.Tag = "passive";
                            labelsIR[Pos.Column - 1]--;
                        }
                    }
                    if (ev.Button == MouseButtons.Left)
                    {
                        if (btn.BackColor == Color.White)
                        {
                            labelsIC[Pos.Row - 1]++;
                            labelsIR[Pos.Column - 1]++;
                            btn.Tag = "active";
                            btn.BackColor = Color.Black;
                        }
                    }
                }
            }
        }
        private void changeToRed(Control btn, Timer timer)
        {
            btn.BackColor = Color.Red;
            timer.Tag = btn;
            timer.Start();
        }

        private void buttonTick(object sender, EventArgs e)
        {
            Timer timer = (Timer)sender;
            Control btn = (Control)timer.Tag;
            btn.BackColor = Color.RoyalBlue;
            btn.Text = "?";
            timer.Enabled = false;
            timer.Stop();
        }
        private void Gameover()
        {
            timerGame.Stop();
            active = false;
            newGame=true;
            wasEnded = true;
            MessageBox.Show("Your score is: " + points, "Congratulations!", MessageBoxButtons.OK);
        }



        private void GameIsWon()
        {
            points += 500;
            wasEnded = true;
            UpdateStatusInfo();
            newGameToolStripMenuItem_Click();
        }

        private void btnCard_MouseEnter(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            if (btn.BackColor == Color.RoyalBlue)
            {
                    btn.BackColor = Color.Yellow;
                    btn.Text = "";
            }
        }
        private void btnCard_MouseLeave(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            if (btn.BackColor == Color.Yellow)
            {
                btn.BackColor = Color.RoyalBlue;
                btn.Text = "?";
            }
        }

        private void newGameToolStripMenuItem_Click(object sender=null, EventArgs e=null)
        {
            if (newGame || !wasEnded)
            {
                points = 0;
            }

            if (!wasEdited)
            {
                activeCounter = 0;
                for (int i = 0; i < ROW_COUNT; i++)
                {
                    labelsIC[i] = 0;
                    labelsIR[i] = 0;
                }

                Random random = new Random();
                for (int i = 9; i < panel1.Controls.Count; i++)
                {
                    //wybierz aktywne przyciski
                    if (random.Next() % 2 == 0)
                        panel1.Controls[i].Tag = "pasive";
                    else
                    {
                        panel1.Controls[i].Tag = "active";
                        activeCounter++;


                        TableLayoutPanelCellPosition Pos = panel1.GetPositionFromControl((Control)panel1.Controls[i]);
                        labelsIC[Pos.Row - 1]++;
                        labelsIR[Pos.Column - 1]++;
                    }
                }
            }
            else
            {
                for (int i = 9; i < panel1.Controls.Count; i++)
                {
                    panel1.Controls[i].MouseDown -= editButton_Click;
                    if (panel1.Controls[i].Tag.ToString() == "active")
                        activeCounter++;
                }
                wasEdited = false;
                points = 0;
            }

            timeLeft = getTime();
            ProgressBar1.Value = timeLeft;
            selectedCounter = 0;

            for (int i=9;i<panel1.Controls.Count;i++)
            {
                //ustawienia przycisków dla kolejnych nowych gier
                panel1.Controls[i].BackColor = Color.RoyalBlue;
                panel1.Controls[i].Text = "?";
            }

            gameStart = DateTime.Now;

            label1.Text = labelsIC[1].ToString();
            label2.Text = labelsIR[0].ToString();
            label3.Text = labelsIR[1].ToString();
            label4.Text = labelsIR[2].ToString();
            label5.Text = labelsIR[3].ToString();
            label6.Text = labelsIC[3].ToString();
            label7.Text = labelsIC[0].ToString();
            label8.Text = labelsIC[2].ToString();

            //Flaga nowej rozgrywki
            active = true;
            
            currentLifes = getLifes();          

            UpdateStatusInfo();
            timerGame.Start();
            
            newGame = false;
            wasEnded = false;
        }

        private void UpdateProgressBar()
        {
            if (gameStart.HasValue)
            {
                DateTime dt = DateTime.Now;
                if(timeLeft>0)
                    ProgressBar1.Value = --timeLeft;
                if (ProgressBar1.Value == 0)
                {
                    gameStart = null;
                    active = false;
                    Gameover();
                }
            }
        }
        private void timerGame_tick(object sender, EventArgs e)
        {
            if (gameStart.HasValue)
                UpdateProgressBar();
            else
                timerGame.Stop();
        }

        private void Form1_Load(object sender, FormClosingEventArgs e)
        {
            MessageBoxButtons Mb = MessageBoxButtons.YesNo;
            DialogResult dr;
            dr = MessageBox.Show("Are you sure?", "Confirmation", Mb);
            if (dr == DialogResult.No)
            {
                e.Cancel=true;
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timerGame.Stop();
            Settings setForm=new Settings();
            setForm.ShowDialog();
            currentLifes = getLifes();
            timeLeft = getTime();
            ProgressBar1.Maximum = timeLeft;
            ProgressBar1.Value = timeLeft;
            UpdateStatusInfo();
        }
        

        private void UpdateStatusInfo()
        {
            lifesLabel.Text = "lifes: " + currentLifes;

            if (currentLifes == 0)
                Gameover();

            scoreLabel.Text = "score: " + points;
        }
        
        private void gameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!gameActive)
            {
                gameToolStripMenuItem.Checked = true;

                saveToolStripMenuItem.Enabled = false;
                openToolStripMenuItem.Enabled = true;
                settingsToolStripMenuItem.Enabled = true;
                newGameToolStripMenuItem.Enabled = true;
                gameActive = true;
                editToolStripMenuItem.Checked = false;
                menuStrip1.BackColor = Color.Empty;
                newGameToolStripMenuItem_Click(sender, e);
            }
            else
            {
                if (gameToolStripMenuItem.Checked == false)
                    gameToolStripMenuItem.Checked = true;
                else
                    gameToolStripMenuItem.Checked = false;
            }
        }
        
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gameActive)
            {
                gameToolStripMenuItem.Checked = false;
                gameActive = false;
                timerGame.Stop();
                active = false;

                saveToolStripMenuItem.Enabled = true;
                openToolStripMenuItem.Enabled = false;
                settingsToolStripMenuItem.Enabled = false;
                newGameToolStripMenuItem.Enabled = false;

                editToolStripMenuItem.Checked = true;
                wasEdited = true;

                activeCounter = 0;
                for (int i = 0; i < ROW_COUNT; i++)
                {
                    labelsIC[i] = 0;
                    labelsIR[i] = 0;
                }

                menuStrip1.BackColor = Color.Gold;
                for (int i = 9; i < panel1.Controls.Count; i++)
                {
                    panel1.Controls[i].BackColor = Color.White;
                    panel1.Controls[i].Text = " ";
                    panel1.Controls[i].Tag = "passive";
                }

                for(int i=0;i<9;i++)
                {
                    panel1.Controls[i].Text = "";
                }
                
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileStream fs;
            var dlg = new OpenFileDialog();
            dlg.Filter = @"Puzzle Game Files(*.pg)|*.pg|All files (*.*)|*.*";
            dlg.Multiselect = false;

            for(int i=0;i<values.Length;i++)
            {
                values[i] = false;
            }

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < 4; i++)
                {
                    labelsIC[i] = 0;
                    labelsIR[i] = 0;
                }
                selectedCounter = 0;
                activeCounter = 0;

                foreach (var file in dlg.FileNames)
                {

                    fs = new FileStream(file, FileMode.Open);
                    BinaryReader bw = new BinaryReader(fs);
                    for (int i = 9; i < panel1.Controls.Count; ++i)
                    {
                        values[i]=bw.ReadBoolean();
                        if (values[i])
                        {
                            panel1.Controls[i].Tag = "active";
                            TableLayoutPanelCellPosition Pos = panel1.GetPositionFromControl((Control)panel1.Controls[i]);
                            labelsIC[Pos.Row - 1]++;
                            labelsIR[Pos.Column - 1]++;
                        }
                        else
                            panel1.Controls[i].Tag = "passive";
                        
                    }
                    bw.Close();
                    fs.Close();
                }
                wasEdited = true;
                newGameToolStripMenuItem_Click();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileStream fs;
            var dlg = new SaveFileDialog();
            dlg.Filter = @"Puzzle Game Files(*.pg)|*.pg|All files (*.*)|*.*";            
            if(dlg.ShowDialog() == DialogResult.OK)
            {
                foreach (var file in dlg.FileNames)
                {
                    fs = new FileStream(file, FileMode.Create);
                    BinaryWriter bw = new BinaryWriter(fs);
                    for (int i = 9; i < panel1.Controls.Count; ++i)
                    {
                        if (panel1.Controls[i].Tag.ToString() == "active")
                            values[i] = true;
                        bw.Write(values[i]);
                    }
                    bw.Close();
                    fs.Close();
                }
            }
        }
    }
    public class LiT
    {
        public static int life;
        public static int time;
    }
}
