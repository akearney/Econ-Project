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
        //This dictionary is basically ALL of the data loaded in
        // It maps from stock symbol to a dictionary of
        // The time periods to the stock information
        private Dictionary<string, Dictionary<Util.TimeType, StockInfo>> stockInformation;
        private Util.TimeType currentTime;


        public DataWindow()
        {
            stockInformation = new Dictionary<string, Dictionary<DataMiner.Util.TimeType, StockInfo>>();
            currentTime = Util.Domain.THIRTY_DAYS;
            InitializeComponent();
        }

        #region Events
        private void WindowClosing(object sender, RoutedEventArgs e)
        {
            if (this.Owner.OwnedWindows.Count == 1)
                this.Owner.Close();
        }
        private void TimePeriodClick(object sender, RoutedEventArgs e)
        {
            TabItem clicked = sender as TabItem;
            Util.TimeType time = Util.Domain.getTime(clicked);
            if (currentTime != time)
            {
                currentTime = time;
                updateTab(this.StockSymbolTabs.SelectedItem as TabItem, currentTime);
            }
            
        }
        private void ChangeStock(object sender, RoutedEventArgs e)
        {
            updateTab(sender as TabItem, currentTime);
        }
        private void newWindow(object sender, RoutedEventArgs e)
        {
            TabItem currentClick = sender as TabItem;
            currentClick.MouseUp -= ChangeStock;
            currentClick.MouseDoubleClick -= newWindow;
            this.StockSymbolTabs.Items.Remove(currentClick);
            Dictionary<Util.TimeType, StockInfo> storedInfo = stockInformation[currentClick.Header.ToString()];
            DataWindow newWin = new DataWindow();
            newWin.dealWithResults(currentClick.Header.ToString(), storedInfo);
            newWin.Owner = this.Owner;
            newWin.Show();
        }

        private void newSearch(object sender, RoutedEventArgs e)
        {
            SeachBox search = new SeachBox();
            search.Owner = this;
            search.Show();
            
        }
        public void Search(string stockSymbol)
        {
            ((Window1)this.Owner).newSearch(stockSymbol, this);
        }
        #endregion

        public void dealWithResults(string stockCall, Dictionary<Util.TimeType, StockInfo> results)
        {
            //Could throw an error if adding the same stock again, just adds another tab using the same info
            try
            {
                stockInformation.Add(stockCall, results);
            }
            catch
            {
            }
            finally
            {
                StockInfo currentInfo;
                results.TryGetValue(currentTime, out currentInfo);
                TabItem newTab = createTab(stockCall, currentInfo);
                //UpdateContent
                this.StockSymbolTabs.Items.Add(newTab);
                this.StockSymbolTabs.SelectedIndex = this.StockSymbolTabs.Items.Count - 1;
            }


        }

        public TabItem createTab(string stockCall, StockInfo stockInfo)
        {
            TabItem newTab = new TabItem();
            StackPanel info = new StackPanel();
            TextBlock name = new TextBlock();
            name.Text = stockCall + " 1";
            info.Children.Add(name);
            newTab.Header = stockCall;
            newTab.Content = info;
            newTab.MouseUp += new MouseButtonEventHandler(ChangeStock);
            newTab.MouseDoubleClick += new MouseButtonEventHandler(newWindow);
            return newTab;

        }
        private void updateTab(TabItem currentTab, Util.TimeType time)
        {
            StackPanel info = new StackPanel();
            TextBlock name = new TextBlock();
            name.Text = currentTab.Name + " " + time.MyType;
            info.Children.Add(name);
            currentTab.Content = info;

        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
