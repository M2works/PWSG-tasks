using System;
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
using System.Windows.Shapes;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace PWŚG___WPF2
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }
        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bi = (BitmapImage)imToIns.Tag;
            imToIns.Source = bi;
            imToIns.UpdateLayout();
        }
        private void save_click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.Filter = "Image Files (*.jpg, *.bmp, *.png)|*.jpg; *.bmp; *.png";
            bool? result = dlg.ShowDialog();
            if (result.HasValue && result.Value)
            {
                string ext = System.IO.Path.GetExtension(dlg.FileName);
                if (ext == ".bmp")
                    SaveToBmp(imToIns, dlg.FileName);
                if (ext == ".png")
                    SaveToPng(imToIns, dlg.FileName);
                if (ext == ".jpg")
                    SaveToJpg(imToIns, dlg.FileName);
            }
        }
        void SaveToBmp(FrameworkElement visual, string fileName)
        {
            var encoder = new BmpBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder);
        }

        void SaveToPng(FrameworkElement visual, string fileName)
        {
            var encoder = new PngBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder);
        }
        void SaveToJpg(FrameworkElement visual, string fileName)
        {
            var encoder = new JpegBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder);
        }


        void SaveUsingEncoder(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            BitmapFrame frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);

            using (var stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }
        }
    }
}
