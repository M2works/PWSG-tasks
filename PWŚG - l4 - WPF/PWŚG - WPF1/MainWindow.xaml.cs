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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.ComponentModel;
using Microsoft.Win32;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace PWŚG___WPF1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            dataGrid.DataContext = ProductNames;
            c = new Cart(); 
            tbtb.DataContext = c;
            productNames.CollectionChanged += OnCollectionChanged;
        }

        [XmlArray("ArrayOfProductInfo")]
        [XmlArrayItem("productInfo", typeof(productInfo))]
        private ObservableCollection<productInfo> productNames = new ObservableCollection<productInfo>();
        public static ObservableCollection<Cart.productCount> productCountNames = new ObservableCollection<Cart.productCount>();
        private  ObservableCollection<productInfo> selectedProducts = new ObservableCollection<productInfo>();

        bool selectByName = false;
        bool selectByPrice = false;
        bool selectByCategory = false;

        public static Cart c;
        public Cart cart;
        public static Cart.productCount pi;
        public ObservableCollection<productInfo> SelectedProducts
        {
            get {
                return selectedProducts;
            }
        }
        public ObservableCollection<productInfo> ProductNames
        {
            get{ return productNames; }
        }
        public ObservableCollection<Cart.productCount> ProductCountNames
        {
            get
            {
                return productCountNames;
            }
        }
        
        [Serializable]
        public class productInfo 
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public double Price { get; set; }
            public Categories Category { get; set; }
            public productInfo(string name, string description, double price, Categories category)
            {
                Name = name;
                Description = description;
                Price = price;
                Category = category;
            }

            public enum Categories
            {
                [XmlEnum(Name = "Food")]
                Food,
                [XmlEnum(Name = "Electronics")]
                Electronics,
                [XmlEnum(Name = "Clothes")]
                Clothes
            };
        }
        public class Cart:INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string name)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }
            private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            public Cart() { }
            public Cart(productCount p, double s)
            {
                pc = new productCount(p.Name,p.Price,p.Count);
                sum = s;
            }
            public class productCount : INotifyPropertyChanged
            {
                public event PropertyChangedEventHandler PropertyChanged;

                protected void OnPropertyChanged(string name)
                {
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(name));
                    }

                }
                public int count;
                public int Count
                {
                    get { return count; }
                    set
                    {
                        count = value;
                        NotifyPropertyChanged();
                    }
                }
                public double Price { get; set; }
                public string Name { get; set; }
                public productCount(string name, double price, int count)
                {
                    Price = price;
                    Name = name;
                    Count = count;
                }

                private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }
            public productCount pc;
            public productCount PC
            {
                get { return pc; }
                set { pc = value; }
            }
            public double sum=0;
            public double Sum
            {
                get { return sum; }
                set
                {
                    sum = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public class productCountInfo
        {
            public string Name { get; set; }
            public double Price { get; set; }
            public productCountInfo(string name, double price)
            {
                Name = name;
                Price = price;
            }
            public productCountInfo() { }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This is simple shop manager application.", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                productNames.Add(new productInfo("Computer1", "description", 15, productInfo.Categories.Electronics));
                productNames.Add(new productInfo("Computer2", "description", 4.5, productInfo.Categories.Electronics));
                productNames.Add(new productInfo("Computer3", "description", 13.49, productInfo.Categories.Electronics));
                productNames.Add(new productInfo("Computer4", "description", 1, productInfo.Categories.Electronics));
                productNames.Add(new productInfo("Computer5", "description", 2, productInfo.Categories.Electronics));
                productNames.Add(new productInfo("Computer6", "description", 3, productInfo.Categories.Electronics));
            }));
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void ActualiseSelectedProducts()
        {
            selectedProducts.Clear();
            for (int i = 0; i < productNames.Count; i++)
                selectedProducts.Add(productNames[i]);
        }
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ActualiseSelectedProducts();
        }

        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Checkout completed. Total price: ");
            sb.AppendFormat(c.Sum.ToString());
            string napis = sb.ToString();
            MessageBox.Show(napis, "Checkout", MessageBoxButton.OK);
        }

        private void ByNameChecked(object sender, RoutedEventArgs e)
        {
            if (ByNameCheckBox.IsChecked.Value)
            {
                ByNameTextBox.IsEnabled = true;
                selectByName = true;
            }
            else
            {
                ByNameTextBox.IsEnabled = false;
                selectByName = false;
            }

        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            selectedProducts.Clear();
            for (int i = 0; i < productNames.Count; i++)
                selectedProducts.Add(productNames[i]);

            List<productInfo> toRemove = new List<productInfo>();
            if (selectByPrice)
            {
                int first, second;
                if (!int.TryParse(ByPriceTextBox1.Text, out first) ||
                    !int.TryParse(ByPriceTextBox2.Text, out second))
                {
                    MessageBox.Show("Invalid format of price!", "An error occured",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                foreach (productInfo pi in selectedProducts)
                    if (pi.Price < first || pi.Price > second)
                        toRemove.Add(pi);
            }

            if (selectByName)
            {
                foreach (productInfo pi in selectedProducts)
                    if (pi.Name != ByNameTextBox.Text)
                        toRemove.Add(pi);
            }

            if (selectByCategory)
            {
                switch (ByCategoryComboBox.SelectedIndex)
                {
                    case 0:
                        selectProducts(productInfo.Categories.Food, toRemove);
                        break;
                    case 1:
                        selectProducts(productInfo.Categories.Electronics, toRemove);
                        break;
                    case 2:
                        selectProducts(productInfo.Categories.Clothes, toRemove);
                        break;
                }
            }

            foreach (productInfo pi in toRemove)
                selectedProducts.Remove(pi);
        }
        
        private void selectProducts(productInfo.Categories category, List<productInfo> toRemove)
        {
            foreach (productInfo pi in selectedProducts)
                if (pi.Category != category)
                    toRemove.Add(pi);
        }
        private void ShowAll_Click(object sender, RoutedEventArgs e)
        {
            selectedProducts.Clear();
            for (int i = 0; i < productNames.Count; i++)
                selectedProducts.Add(productNames[i]);
        }


        private void ByPriceChecked(object sender, RoutedEventArgs e)
        {
            if (ByPriceCheckBox.IsChecked.Value)
            {
                ByPriceTextBox1.IsEnabled = true;
                ByPriceTextBox2.IsEnabled = true;
                selectByPrice = true;
            }
            else
            {
                ByPriceTextBox1.IsEnabled = false;
                ByPriceTextBox2.IsEnabled = false;
                selectByPrice = false;
            }
        }

        private void ByCategoryChecked(object sender, RoutedEventArgs e)
        {
            if (ByCategoryCheckBox.IsChecked.Value)
            {
                ByCategoryComboBox.IsEnabled = true;
                selectByCategory = true;
            }
            else
            {
                ByCategoryComboBox.IsEnabled = false;
                selectByCategory = false;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML Files (*.xml)|*.xml";
            bool? result = dlg.ShowDialog();
            if (result.HasValue && result.Value)
            {
                string filetype = System.IO.Path.GetExtension(dlg.FileName);
                if (filetype != ".xml")
                {
                    MessageBox.Show("Error!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<productInfo>));
                using (StreamWriter wr = new StreamWriter(dlg.FileName))
                {
                    xs.Serialize(wr, productNames);
                }
            }
        }
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            productNames.Clear();
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML Files (*.xml)|*.xml";
            bool? result = dlg.ShowDialog();
            if (result.HasValue && result.Value)
            {
                string filetype = System.IO.Path.GetExtension(dlg.FileName);
                if (filetype != ".xml")
                {
                    MessageBox.Show("Error!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                XmlSerializer deserializer = new XmlSerializer(typeof(ObservableCollection<productInfo>));
                productNames.Clear();
                TextReader reader = new StreamReader(dlg.FileName);
                object obj = deserializer.Deserialize(reader);
                for (int i = 0; i < ((ObservableCollection<productInfo>)obj).Count; i++)
                    productNames.Add(((ObservableCollection<productInfo>)obj)[i]);
                reader.Close();
            }
        }

        private void ClearProducts_Click(object sender, RoutedEventArgs e)
        {
            productNames.Clear();
            productCountNames.Clear();
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            Window1 win2 = new Window1();
            Button bn = (Button)sender;
            win2.TextBoxToSet.Tag = ((productInfo)((Button)sender).Tag).Price;
            win2.LabelToInsertContent.Content = ((productInfo)((Button)sender).Tag).Name;
            win2.Show();
            
        }
        private void RemoveFromCart_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < productCountNames.Count; i++)
                if ((Cart.productCount)(((Button)sender).Tag) == productCountNames[i])
                    c.Sum -= productCountNames[i].Price* productCountNames[i].Count;

            productCountNames.Remove((Cart.productCount)(((Button)sender).Tag));
        }

        private void AddCount_Click(object sender, RoutedEventArgs e)
        {
            for(int i=0;i<productCountNames.Count;i++)
                if((Cart.productCount)(((Button)sender).Tag)==productCountNames[i])
                {
                    productCountNames[i].Count++;
                    c.Sum += productCountNames[i].Price;
                    return;
                }
        }

        private void MinusCount_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < productCountNames.Count; i++)
                if ((Cart.productCount)(((Button)sender).Tag) == productCountNames[i])
                {
                    if (productCountNames[i].Count != 0)
                    {
                        productCountNames[i].Count--;
                        c.Sum -= productCountNames[i].Price;
                    }
                    return;
                }
        }
    }
}
