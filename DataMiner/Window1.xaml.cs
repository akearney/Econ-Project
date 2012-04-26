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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace DataMiner
{
    /// <summary>
    /// Interaction logic for Window1.xaml. Hi ANDY!
    /// </summary>
    public partial class Window1 : Window
    {
        #region Locals
        //Handles all of the we requesting
        private WebInteractor webInteractor;
        private DataWindow dataWindow;
        #endregion

        #region Constants

        #endregion
        public Window1()
        {
            webInteractor = new WebInteractor();
            InitializeComponent();
        }
        private void searchBoxMouseDown(object sender, RoutedEventArgs e)
        {
            this.SearchBox.Text = "";
        }
        private void searchButtonClick(object sender, RoutedEventArgs e)
        {
            string searchItem = this.SearchBox.Text;
            dataWindow = new DataWindow();
            dataWindow.Owner = this;
            StockInfo info = new StockInfo();

            newSearch(searchItem, this.dataWindow);
            this.dataWindow.Show();
            this.Hide();
        }

        public void newSearch(string searchItem, DataWindow window)
        {
            List<double> data = webInteractor.Search(searchItem);
            Image graph = webInteractor.getYearGraph();
            StockInfo info = new StockInfo();
            Dictionary<Util.TimeType, StockInfo> newStockInformation = Calculator.calculateAllStockInfo(data);
            //Dictionary<Util.TimeType, StockInfo> newStockInformation = new Dictionary<DataMiner.Util.TimeType, StockInfo>();
            window.dealWithResults(searchItem, newStockInformation);
        }
    }
}
