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
            string result = webInteractor.Search(searchItem);
            Image graph = webInteractor.getYearGraph();
            if (graph != null)
            {
                //this.Title.Margin = new System.Windows.Thickness(10);
                //this.Title.Text = searchItem + " Year";
                
                this.Grid.Children.Remove(SearchChildren);
                StackPanel picturePanel = new StackPanel();


                this.Grid.Children.Add(graph);
                dataWindow = new DataWindow();
                dataWindow.Owner = this;
                dataWindow.dealWithResults(searchItem);
                this.dataWindow.Show();
                this.Hide();
            }
        }

        public void newSearch(string searchItem)
        {
            string result = webInteractor.Search(searchItem);
            Image graph = webInteractor.getYearGraph();
            this.dataWindow.dealWithResults(searchItem);
        }
    }
}
