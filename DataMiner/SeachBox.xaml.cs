using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DataMiner
{
    /// <summary>
    /// Interaction logic for SeachBox.xaml
    /// </summary>
    public partial class SeachBox : Window
    {
        public SeachBox()
        {
            InitializeComponent();
        }

        private void searchButtonClick (object sender, RoutedEventArgs e)
        {
            ((DataWindow)this.Owner).Search(this.SearchBox.Text);
            this.Close();
        }
        private void searchBoxMouseDown(object sender, RoutedEventArgs e)
        {
            this.SearchBox.Text = "";
        }
    }
}
