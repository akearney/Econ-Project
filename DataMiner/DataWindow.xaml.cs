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
            currentTime = Util.Domain.ONE_YEAR;
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
                TabItem newTab = createTab(stockCall);
                //UpdateContent
                this.StockSymbolTabs.Items.Add(newTab);
                this.StockSymbolTabs.SelectedIndex = this.StockSymbolTabs.Items.Count - 1;
            }


        }

        public TabItem createTab(string stockCall)
        {
            TabItem newTab = new TabItem();
            newTab.Header = stockCall;
            updateTab(newTab, currentTime);
            newTab.MouseUp += new MouseButtonEventHandler(ChangeStock);
            newTab.MouseDoubleClick += new MouseButtonEventHandler(newWindow);
            return newTab;

        }
        private void updateTab(TabItem currentTab, Util.TimeType time)
        {
            //Get the corresponding data, which is already stored
            StockInfo stockInfo = stockInformation[currentTab.Header.ToString()][time];

            //The border surrounds the stackpanel
            Border myBorder = new Border();
            //myBorder.Width = 200;
            myBorder.HorizontalAlignment = HorizontalAlignment.Center;
            myBorder.VerticalAlignment = VerticalAlignment.Center;
            myBorder.Background = Brushes.SkyBlue;
            myBorder.BorderBrush = Brushes.Black;
            myBorder.BorderThickness = new Thickness(1);

            //Creating the stackpanel to store the information
            StackPanel info = new StackPanel();

            //Things added to the stack panel
            TextBlock name = new TextBlock();
            name.Text = currentTab.Header.ToString();
            info.Children.Add(name);

            TextBlock days = new TextBlock();
            days.Text = "Days of data: " + stockInfo.NumDays.ToString() ;
            info.Children.Add(days);

            TextBlock alpha = new TextBlock();
            alpha.Text = "Alpha: " + stockInfo.Alpha.ToString(); 
            info.Children.Add(alpha);

            TextBlock beta = new TextBlock();
            beta.Text = "Beta: " + stockInfo.Beta.ToString();
            info.Children.Add(beta);

            TextBlock min = new TextBlock();
            min.Text = "Min: " + stockInfo.Min.ToString();
            info.Children.Add(min);

            TextBlock max = new TextBlock();
            max.Text = "Max: " + stockInfo.Max.ToString();
            info.Children.Add(max);

            TextBlock MinNorm = new TextBlock();
            MinNorm.Text = "Min Norm DCGR: " + stockInfo.MinNormDCGR.ToString();
            info.Children.Add(MinNorm);

            TextBlock MaxNorm = new TextBlock();
            MaxNorm.Text = "Max Norm DCGR: " + stockInfo.MaxNormDCGR.ToString();
            info.Children.Add(MaxNorm);

            myBorder.Child = info;
            currentTab.Content = myBorder;

        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
