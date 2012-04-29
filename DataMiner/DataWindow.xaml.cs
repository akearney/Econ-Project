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
        private const int SIGFIG = 4;
        #endregion

        //This dictionary is basically ALL of the data loaded in
        // It maps from stock symbol to a dictionary of
        // The time periods to the stock information
        private Dictionary<string, Dictionary<Util.TimeType, StockInfo>> stockInformation;
        private Util.TimeType currentTime;
        private string currentStock;

        private List<UIElement> hasEvents;
        private SeachBox searchBoxOpen;

        //Information for calculating the probability stuff
        private double strikePrice;
        private double optionPrice;
        private int daysToExperation;
        TextBlock probdata;
        private const int NOTSET = -1;



        public DataWindow()
        {
            stockInformation = new Dictionary<string, Dictionary<DataMiner.Util.TimeType, StockInfo>>();
            currentTime = Util.Domain.ONE_YEAR;
            strikePrice = NOTSET;
            optionPrice = NOTSET;
            daysToExperation = NOTSET;
            hasEvents = new List<UIElement>();
            InitializeComponent();
        }

        #region Events
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Window1 window = ((Window1)this.Owner);
            window.numWindows--;
            if (window.numWindows == 0)
            {
                this.Owner = null;
                window.end();
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
        private void setStrikePrice(object sender, RoutedEventArgs e)
        {
            TextBox box = sender as TextBox;
            if (box.Text == "")
                return;
            double data;
            if (double.TryParse(box.Text, out data))
                strikePrice = data;
            else
            {
                MessageBox.Show("You can only enter numbers here");
                box.Text = box.Text.Substring(0,box.Text.Length - 1);
                box.Select(box.Text.Length, 0);
            }
            updateProbability();

        }
        private void setOptionPrice(object sender, RoutedEventArgs e)
        {
            TextBox box = sender as TextBox;
            double data;
            if (double.TryParse(box.Text, out data))
                optionPrice = data;
            else
            {
                MessageBox.Show("You can only enter numbers here");
                box.Text = box.Text.Substring(0,box.Text.Length - 1);
                box.Select(box.Text.Length, 0);
            }
            updateProbability();
        }
        private void setDaysLeft(object sender, RoutedEventArgs e)
        {
            TextBox box = sender as TextBox;
            int data;
            if (int.TryParse(box.Text, out data))
                daysToExperation = data;
            else
            {
                MessageBox.Show("You can only enter integers here");
                box.Text = box.Text.Substring(0,box.Text.Length - 1);
                box.Select(box.Text.Length, 0);
            }
            updateProbability();

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

        private void updateProbability()
        {
            if (daysToExperation == NOTSET || strikePrice == NOTSET || optionPrice == NOTSET)
                return;
            else
            {
                StockInfo info = stockInformation[currentStock][currentTime];
                double prob = Calculator.probabilityCalculator(info.SpotPrice, strikePrice, info.Alpha, info.Beta, daysToExperation);
                probdata.Text = Math.Round(prob, SIGFIG).ToString();
            }
        }

        public TabItem createTab(string stockCall)
        {
            TabItem newTab = new TabItem();
            newTab.Header = stockCall;
            updateTab(newTab, currentTime);
            newTab.MouseDoubleClick += new MouseButtonEventHandler(newWindow);
            return newTab;

        }

        private void removeGraphAsChild()
        {
            try
            {
                ((DockPanel)stockInformation[currentStock][currentTime].Graph.Parent).Children.Remove(stockInformation[currentStock][currentTime].Graph);

            }
            catch
            { }
        }

        private void updateTab(TabItem currentTab, Util.TimeType time)
        {
            DockPanel tab = new DockPanel();
            //Get the corresponding data, which is already stored
            removeGraphAsChild();

            #region stockinfo
            StockInfo stockInfo = stockInformation[currentTab.Header.ToString()][time];
            currentStock = currentTab.Header.ToString();
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
            DockPanel info = createStockInfo(currentTab, stockInfo);
            DockPanel option = createOptionInfo(currentTab);
            content.Children.Add(info);
            content.Children.Add(option);

            myBorder.Child = content;
            #endregion

            DockPanel.SetDock(stockInfo.Graph, Dock.Top);
            DockPanel.SetDock(myBorder, Dock.Bottom);
            tab.Children.Add(stockInfo.Graph);
            tab.Children.Add(myBorder);
            currentTab.Content = tab;


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

            TextBlock prob = new TextBlock();
            prob.Text = "Probability: ";
            prob.HorizontalAlignment = HorizontalAlignment.Right;
            prob.Margin = new Thickness(2);

            text.Children.Add(description);
            text.Children.Add(price);
            text.Children.Add(days);
            text.Children.Add(prob);
            #endregion

            #region input boxes

            StackPanel boxes = new StackPanel();

            DockPanel.SetDock(boxes, Dock.Right);
            TextBox strikebox = new TextBox();
            strikebox.MinWidth = 80;
            //strikebox.Text = "Enter Price";
            strikebox.Tag = currentTab.Header.ToString().ToUpper();
            hasEvents.Add(strikebox);
            strikebox.KeyUp += new KeyEventHandler(setStrikePrice);

            TextBox pricebox = new TextBox();
            //pricebox.Text = "Enter Price";
            pricebox.Tag = currentTab.Header.ToString().ToUpper();
            hasEvents.Add(pricebox);
            pricebox.KeyUp += new KeyEventHandler(setOptionPrice);
            //pricebox.PreviewMouseUp += new MouseButtonEventHandler(price_MouseDown);

            TextBox daysbox = new TextBox();
            //daysbox.Text = "Enter Days";
            daysbox.Tag = currentTab.Header.ToString().ToUpper();
            hasEvents.Add(daysbox);
            daysbox.KeyUp += new KeyEventHandler(setDaysLeft);
            //daysbox.MouseUp += new MouseButtonEventHandler(price_MouseDown);

            probdata = new TextBlock();
            probdata.Text = "Enter Option Info to Calculate";
            probdata.Margin = new Thickness(2);

            boxes.Children.Add(strikebox);
            boxes.Children.Add(pricebox);
            boxes.Children.Add(daysbox);
            boxes.Children.Add(probdata);

            #endregion


            info.Children.Add(text);
            info.Children.Add(boxes);

            return info;
            
        }
        private DockPanel createStockInfo(TabItem currentTab, StockInfo stockInfo)
        {
            DockPanel stockinfo = new DockPanel();
            //Creating the stackpanel to store the information
            StackPanel info = new StackPanel();
            DockPanel.SetDock(info, Dock.Right);
            StackPanel data = new StackPanel();
            DockPanel.SetDock(info, Dock.Left);

            //Things added to the stack panel
            TextBlock name = new TextBlock();
            name.HorizontalAlignment = HorizontalAlignment.Center;
            name.Text = currentTab.Header.ToString().ToUpper();
            name.FontWeight = FontWeights.Bold;
            DockPanel.SetDock(name, Dock.Top);
            stockinfo.Children.Add(name);

            TextBlock days = new TextBlock();
            days.Text = "Days of data: ";
            days.HorizontalAlignment = HorizontalAlignment.Right;
            TextBlock daysdata = new TextBlock();
            daysdata.Text = stockInfo.NumDays.ToString();
            daysdata.HorizontalAlignment = HorizontalAlignment.Left;
            info.Children.Add(days);
            data.Children.Add(daysdata);

            TextBlock spot = new TextBlock();
            spot.Text = "Last Price: ";
            spot.HorizontalAlignment = HorizontalAlignment.Right;
            TextBlock spotdata = new TextBlock();
            spotdata.Text = stockInfo.SpotPrice.ToString();
            spotdata.HorizontalAlignment = HorizontalAlignment.Left;
            info.Children.Add(spot);
            data.Children.Add(spotdata);

            TextBlock alpha = new TextBlock();
            alpha.Text = "Alpha: ";
            alpha.HorizontalAlignment = HorizontalAlignment.Right;
            TextBlock alphadata = new TextBlock();
            alphadata.Text = Math.Round(stockInfo.Alpha, SIGFIG).ToString();
            alphadata.HorizontalAlignment = HorizontalAlignment.Left;
            info.Children.Add(alpha);
            data.Children.Add(alphadata);

            TextBlock beta = new TextBlock();
            beta.Text = "Beta: ";
            beta.HorizontalAlignment = HorizontalAlignment.Right;
            TextBlock betadata = new TextBlock();
            betadata.Text = Math.Round(stockInfo.Beta, SIGFIG).ToString();
            betadata.HorizontalAlignment = HorizontalAlignment.Left;
            info.Children.Add(beta);
            data.Children.Add(betadata);

            TextBlock min = new TextBlock();
            min.Text = "Min: ";
            min.HorizontalAlignment = HorizontalAlignment.Right;
            TextBlock mindata = new TextBlock();
            mindata.Text = Math.Round(stockInfo.Min, SIGFIG).ToString();
            mindata.HorizontalAlignment = HorizontalAlignment.Left;
            info.Children.Add(min);
            data.Children.Add(mindata);

            TextBlock max = new TextBlock();
            max.Text = "Max: ";
            max.HorizontalAlignment = HorizontalAlignment.Right;
            TextBlock maxdata = new TextBlock();
            maxdata.Text = Math.Round(stockInfo.Max, SIGFIG).ToString();
            maxdata.HorizontalAlignment = HorizontalAlignment.Left;
            info.Children.Add(max);
            data.Children.Add(maxdata);

            TextBlock MinNorm = new TextBlock();
            MinNorm.Text = "Min Norm DCGR: ";
            MinNorm.HorizontalAlignment = HorizontalAlignment.Right;
            TextBlock MinNormdata = new TextBlock();
            MinNormdata.Text = Math.Round(stockInfo.MinNormDCGR, SIGFIG).ToString();
            MinNormdata.HorizontalAlignment = HorizontalAlignment.Left;
            info.Children.Add(MinNorm);
            data.Children.Add(MinNormdata);

            TextBlock MaxNorm = new TextBlock();
            MaxNorm.Text = "Max Norm DCGR: ";
            MaxNorm.HorizontalAlignment = HorizontalAlignment.Right;
            TextBlock MaxNormdata = new TextBlock();
            MaxNormdata.Text = Math.Round(stockInfo.MaxNormDCGR, SIGFIG).ToString();
            MaxNormdata.HorizontalAlignment = HorizontalAlignment.Left;
            info.Children.Add(MaxNorm);
            data.Children.Add(MaxNormdata);

            stockinfo.Children.Add(info);
            stockinfo.Children.Add(data);

            return stockinfo;
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
