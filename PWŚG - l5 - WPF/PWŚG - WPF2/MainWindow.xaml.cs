using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Windows.Media.Animation;

namespace PWŚG___WPF2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isChecked = true;


        private object dummyNode = null;
        public string SelectedImagePath { get; set; }
        public class DirectoryInfo : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string name)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }

            }
            public string ImagePath { get; set; }
            public string Name { get; set; }
            public DirectoryInfo() { }
            public DirectoryInfo(string ip, string n)
            {
                ImagePath = ip;
                Name = n;
            }
            private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
    }
        public ObservableCollection<Image> images=new ObservableCollection<Image>();
        public ObservableCollection<Image> Images
        {
            get
            {
                return images;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = images;
            foldersItem.SelectedItemChanged += treeItem_Selected;
        }

        private void Exit_click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void About_click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Simple image browser", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_mouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            System.Windows.Controls.Button b = (System.Windows.Controls.Button)sender;
            b.Background = Brushes.Yellow;
        }

        private void Button_mouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            System.Windows.Controls.Button b = (System.Windows.Controls.Button)sender;
            b.Background = Brushes.Yellow;
        }

        private void Button_mouseClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button b = (System.Windows.Controls.Button)sender;
            b.Background = Brushes.Yellow;
        }

        private void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Filter = "Image Files (*.jpg, *.jpeg, *.bmp, *.png)|*.jpg; *.jpeg; *.bmp; *.png";
            bool? result = dlg.ShowDialog();
            if (result.HasValue && result.Value)
            {
                Window1 win2 = new Window1();
                BitmapImage bi = new BitmapImage(new Uri(dlg.FileName));
                ImageBrush ib = new ImageBrush(bi);
                win2.Height = bi.PixelHeight + 43;
                win2.Width = bi.PixelWidth + 275;
                double proportion = bi.Height / bi.Width;
                //if (win2.Height + 100 > SystemParameters.PrimaryScreenHeight)
                //{
                //    win2.Height = SystemParameters.PrimaryScreenHeight - 100;
                //    win2.Width = win2.Width - 100 * proportion;
                //}
                //if (win2.Width + 100 > SystemParameters.PrimaryScreenWidth)
                //{
                //    win2.Height = SystemParameters.PrimaryScreenHeight - 100;
                //    win2.Width = SystemParameters.PrimaryScreenWidth - 100 * proportion;
                //}
                win2.imToIns.Source = bi;
                win2.imToIns.Tag = bi;
                win2.imageBorder.Background = ib;
                ib.Stretch = Stretch.Fill;
                win2.Title = System.IO.Path.GetFileName(dlg.FileName);
                FileInfo fi = new FileInfo(dlg.FileName);
                win2.l1.Content = win2.Title;
                StringBuilder sb = new StringBuilder();
                sb.Append(bi.PixelWidth);
                sb.Append("px");
                win2.l2.Content = sb.ToString();
                sb.Clear();
                sb.Append(bi.PixelHeight);
                sb.Append("px");
                win2.l3.Content = sb.ToString();
                win2.l4.Content = fi.CreationTime;
                win2.Show();
            }
        }
        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            DialogResult result = dlg.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FilesFromFolderToMiniatures(dlg.SelectedPath);
            }
        }


        private void FilesFromFolderToMiniatures(string fullPath)
        {
            List<DirectoryInfo> infos = new List<DirectoryInfo>();
            IEnumerable<string> files = Directory.GetFiles(fullPath, "*.jpg")
                                        .Concat(Directory.GetFiles(fullPath, "*.jpeg"))
                                        .Concat(Directory.GetFiles(fullPath, "*.bmp"))
                                        .Concat(Directory.GetFiles(fullPath, "*.png"));
            foreach (var file in files)
            {
                string name = System.IO.Path.GetFileName(file);
                infos.Add(new DirectoryInfo() { ImagePath = file, Name = name });
            }
            this.DataContext = infos;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string s in Directory.GetLogicalDrives())
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = s;
                item.Tag = s;
                item.FontWeight = FontWeights.Normal;
                item.Items.Add(dummyNode);
                item.Expanded += new RoutedEventHandler(folder_Expanded);
                foldersItem.Items.Add(item);
            }
        }
        void treeItem_Selected(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TreeView temp = (System.Windows.Controls.TreeView)sender;
            TreeViewItem tvi = (TreeViewItem)temp.SelectedItem;
            FilesFromFolderToMiniatures((string)tvi.Tag);
        }
        private void foldersItem_SelectedItemChanged(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TreeView tree = (System.Windows.Controls.TreeView)sender;
            TreeViewItem temp = ((TreeViewItem)tree.SelectedItem);

            if (temp == null)
                return;
            SelectedImagePath = "";
            string temp1 = "";
            string temp2 = "";
            while (true)
            {
                temp1 = temp.Header.ToString();
                if (temp1.Contains(@"\"))
                {
                    temp2 = "";
                }
                SelectedImagePath = temp1 + temp2 + SelectedImagePath;
                if (temp.Parent.GetType().Equals(typeof(System.Windows.Controls.TreeView)))
                {
                    break;
                }
                temp = ((TreeViewItem)temp.Parent);
                temp2 = @"\";
            }
            //show user selected path
            System.Windows.MessageBox.Show(SelectedImagePath);
        }

        void folder_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == dummyNode)
            {
                item.Items.Clear();
                try
                {
                    foreach (string s in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
                        subitem.Tag = s;
                        subitem.FontWeight = FontWeights.Normal;
                        subitem.Items.Add(dummyNode);
                        subitem.Expanded += new RoutedEventHandler(folder_Expanded);
                        item.Items.Add(subitem);
                    }
                }
                catch (Exception) { }
            }
        }

        private void cb_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation da = new DoubleAnimation();
            if (isChecked)
            {
                da.To = 0;
                isChecked = false;
            }
            else
            {
                da.To = 1;
                isChecked = true;
            }
            da.Duration = new Duration(TimeSpan.FromSeconds(2));
            dp.BeginAnimation(OpacityProperty, da);
        }

        private void foldersItem_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void listViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.ListViewItem item = sender as System.Windows.Controls.ListViewItem;
            DirectoryInfo im = (DirectoryInfo)item.Content;
            Window1 win2 = new Window1();
            BitmapImage bi = new BitmapImage(new Uri(im.ImagePath));
            ImageBrush ib = new ImageBrush(bi);
            win2.Height = bi.PixelHeight + 43;
            win2.Width = bi.PixelWidth + 275;
            double proportion = bi.Height / bi.Width;
            //if (win2.Height + 100 > SystemParameters.PrimaryScreenHeight)
            //{
            //    win2.Height = SystemParameters.PrimaryScreenHeight - 100;
            //    win2.Width = win2.Width - 100 * proportion;
            //}
            //if (win2.Width + 100 > SystemParameters.PrimaryScreenWidth)
            //{
            //    win2.Height = SystemParameters.PrimaryScreenHeight - 100;
            //    win2.Width = SystemParameters.PrimaryScreenWidth - 100 * proportion;
            //}
            win2.imToIns.Source = bi;
            win2.imToIns.Tag = bi;
            win2.imageBorder.Background = ib;
            win2.imageBorder.Height = bi.PixelHeight;
            win2.imageBorder.Width = bi.PixelWidth;
            win2.Height = bi.PixelHeight + 43;
            win2.Width = bi.PixelWidth + 275;
            ib.Stretch = Stretch.Fill;
            win2.Title = System.IO.Path.GetFileName(im.ImagePath);
            FileInfo fi = new FileInfo(im.ImagePath);
            win2.l1.Content = win2.Title;
            StringBuilder sb = new StringBuilder();
            sb.Append(bi.PixelWidth);
            sb.Append("px");
            win2.l2.Content = sb.ToString();
            sb.Clear();
            sb.Append(bi.PixelHeight);
            sb.Append("px");
            win2.l3.Content = sb.ToString();
            win2.l4.Content = fi.CreationTime;
            win2.Show();
        }
    }
}
