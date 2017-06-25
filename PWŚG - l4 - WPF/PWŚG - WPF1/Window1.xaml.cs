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

namespace PWŚG___WPF1
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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ATC_Click(object sender, RoutedEventArgs e)
        {
            int value;
            if (!int.TryParse(TextBoxToSet.Text, out value))
            {
                MessageBox.Show("Wrong input format!", "An error occured", MessageBoxButton.OK);
                Application.Current.Shutdown();
            }
            MainWindow.pi = new MainWindow.Cart.productCount(
                (string)LabelToInsertContent.Content,
                double.Parse(TextBoxToSet.Tag.ToString()),
                int.Parse(TextBoxToSet.Text.ToString()));

            MainWindow.c.PC=
               new MainWindow.Cart.productCount(
                (string)LabelToInsertContent.Content,
                double.Parse(TextBoxToSet.Tag.ToString()),
                int.Parse(TextBoxToSet.Text.ToString())
                );

            bool exists = false;

            for(int i=0;i<MainWindow.productCountNames.Count;i++)
            {
                if (MainWindow.productCountNames[i].Name == MainWindow.pi.Name && MainWindow.productCountNames[i].Price == MainWindow.pi.Price)
                {
                    MainWindow.productCountNames[i].Count += MainWindow.pi.Count;
                    MainWindow.c.Sum += MainWindow.pi.Count * MainWindow.pi.Price;
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                MainWindow.productCountNames.Add(MainWindow.pi);
                MainWindow.c.Sum += MainWindow.pi.Count * MainWindow.pi.Price;
            }
                this.Close();
        }
    }
}
