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
        #region Constants
        private const int SIGFIG = 2;
        #endregion

        //This dictionary is basically ALL of the data loaded in
        // It maps from stock symbol to a dictionary of
        // The time periods to the stock information
        private Dictionary<string, Dictionary<Util.TimeType, StockInfo>> stockInformation;
        private Util.TimeType currentTime;

        private List<UIElement> hasEvents;
        private SeachBox searchBoxOpen;


        public DataWindow()
        {
            stockInformation = new Dictionary<string, Dictionary<DataMiner.Util.TimeType, StockInfo>>();
            currentTime = Util.Domain.ONE_YEAR;
            hasEvents = new List<UIElement>();
            InitializeComponent();
        }

        #region Events
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            ((Window1)this.Owner).numWindows--;
            if (((Window1)this.Owner).numWindows == 0)
            {
                ((Window1)this.Owner).end();
            }
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
        //Warning event fires wherever you click on the page of a tab
        private void newWindow(object sender, MouseButtonEventArgs e)
        {
            TabItem currentClick = sender as TabItem;

            //This point stuff is so that the new window is only triggered near the ACTUAL tab
            GeneralTransform transform = currentClick.TransformToAncestor(this);
            Point rootPoint = transform.Transform(new Point(0, 0));
            Point tabClick = e.GetPosition(this);

            if (Math.Abs(rootPoint.X - tabClick.X) > 20)
                return;
            //Clean up dynamically assigned events, so they are not triggered for this window
            currentClick.MouseUp -= ChangeStock;
            currentClick.MouseDoubleClick -= newWindow;
            foreach (UIElement elem in hasEvents)
            {
                if ( (string)((TextBox)elem).Tag == currentClick.Header.ToString().ToUpper())
                {
                    ((TextBox)elem).MouseDown -= price_MouseDown;
                }
            }
            this.StockSymbolTabs.Items.Remove(currentClick);
            Dictionary<Util.TimeType, StockInfo> storedInfo = stockInformation[currentClick.Header.ToString()];
            DataWindow newWin = new DataWindow();
            newWin.dealWithResults(currentClick.Header.ToString(), storedInfo);
            newWin.Owner = this.Owner;
            newWin.Show();
        }

        private void newSearch(object sender, RoutedEventArgs e)
        {
            if (searchBoxOpen != null)
            {
                searchBoxOpen.Close();
            }
            SeachBox search = new SeachBox();
            search.Owner = this;
            searchBoxOpen = search;
            search.Show();

            
        }
        public void Search(string stockSymbol)
        {
            ((Window1)this.Owner).newSearch(stockSymbol, this);
        }
        private void price_MouseDown(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).Text = "";
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
            StackPanel content = new StackPanel();
            StackPanel info = createStockInfo(currentTab, stockInfo);
            DockPanel option = createOptionInfo(currentTab);
            content.Children.Add(info);
            content.Children.Add(option);

            myBorder.Child = content;
            currentTab.Content = myBorder;


        }
        private DockPanel createOptionInfo(TabItem currentTab)
        {
            DockPanel info = new DockPanel();
            info.Name = "Options";
            info.Margin = new Thickness(10);
           
            TextBlock name = new TextBlock();
            name.Text = "Option";
            name.HorizontalAlignment = HorizontalAlignment.Center;
            DockPanel.SetDock(name, Dock.Top);
            name.FontWeight = FontWeights.Bold;            
            info.Children.Add(name);

            #region text for options

            StackPanel text = new StackPanel();
            DockPanel.SetDock(text, Dock.Left);
            TextBlock description = new TextBlock();
            description.Text = "Call Option Strike Price";
            description.HorizontalAlignment = HorizontalAlignment.Right;
            description.Margin = new Thickness(2);

            TextBlock price = new TextBlock();
            price.Text = "Call Option Price";
            price.HorizontalAlignment = HorizontalAlignment.Right;
            price.Margin = new Thickness(2);

            TextBlock days = new TextBlock();
            days.Text = "Days To Expiration";
            days.HorizontalAlignment = HorizontalAlignment.Right;
            days.Margin = new Thickness(2);

            text.Children.Add(description);
            text.Children.Add(price);
            text.Children.Add(days);
            #endregion

            #region input boxes

            StackPanel boxes = new StackPanel();

            DockPanel.SetDock(boxes, Dock.Right);
            TextBox strikebox = new TextBox();
            strikebox.Text = "Enter Price";
            strikebox.Tag = currentTab.Header.ToString().ToUpper();
            hasEvents.Add(strikebox);
            strikebox.MouseUp += new MouseButtonEventHandler(price_MouseDown);

            TextBox pricebox = new TextBox();
            pricebox.Text = "Enter Price";
            pricebox.Tag = currentTab.Header.ToString().ToUpper();
            hasEvents.Add(pricebox);
            pricebox.MouseEnter += new MouseEventHandler(price_MouseDown);

            TextBox daysbox = new TextBox();
            daysbox.Text = "Enter Days";
            daysbox.Tag = currentTab.Header.ToString().ToUpper();
            hasEvents.Add(daysbox);
            daysbox.MouseUp += new MouseButtonEventHandler(price_MouseDown);

            boxes.Children.Add(strikebox);
            boxes.Children.Add(pricebox);
            boxes.Children.Add(daysbox);

            #endregion


            info.Children.Add(text);
            info.Children.Add(boxes);

            return info;
            
        }
        private StackPanel createStockInfo(TabItem currentTab, StockInfo stockInfo)
        {
            //Creating the stackpanel to store the information
            StackPanel info = new StackPanel();

            //Things added to the stack panel
            TextBlock name = new TextBlock();
            name.Text = currentTab.Header.ToString().ToUpper();
            name.FontWeight = FontWeights.Bold;
            info.Children.Add(name);

            TextBlock days = new TextBlock();
            days.Text = "Days of data: " + stockInfo.NumDays.ToString();
            info.Children.Add(days);

            TextBlock alpha = new TextBlock();
            alpha.Text = "Alpha: " + Math.Round(stockInfo.Alpha, SIGFIG).ToString();
            info.Children.Add(alpha);

            TextBlock beta = new TextBlock();
            beta.Text = "Beta: " + Math.Round(stockInfo.Beta, SIGFIG).ToString();
            info.Children.Add(beta);

            TextBlock min = new TextBlock();
            min.Text = "Min: " + Math.Round(stockInfo.Min, SIGFIG).ToString();
            info.Children.Add(min);

            TextBlock max = new TextBlock();
            max.Text = "Max: " + Math.Round(stockInfo.Max, SIGFIG).ToString();
            info.Children.Add(max);

            TextBlock MinNorm = new TextBlock();
            MinNorm.Text = "Min Norm DCGR: " + Math.Round(stockInfo.MinNormDCGR, SIGFIG).ToString();
            info.Children.Add(MinNorm);

            TextBlock MaxNorm = new TextBlock();
            MaxNorm.Text = "Max Norm DCGR: " + Math.Round(stockInfo.MaxNormDCGR, SIGFIG).ToString();
            info.Children.Add(MaxNorm);

            return info;
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
