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
    /// Interaction logic for DataWindow.xaml
    /// </summary>
    public partial class DataWindow : Window
    {
        public DataWindow()
        {
            InitializeComponent();
        }

        private void newSearch(object sender, RoutedEventArgs e)
        {
            SeachBox search = new SeachBox();
            search.Owner = this;
            search.Show();
            
        }
        public void Search(string stockSymbol)
        {
            ((Window1)this.Owner).newSearch(stockSymbol);
        }

        public void dealWithResults(string stockSymbol)//otherstuff
        {
            TabItem newTab = new TabItem();
            newTab.Header = stockSymbol;
            //UpdateContent
            this.StockSymbolTabs.Items.Add(newTab);
            this.StockSymbolTabs.SelectedIndex = this.StockSymbolTabs.Items.Count - 1;


        }
    }
}
