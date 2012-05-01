using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Text;
using System.Drawing;
using System.Media;

using System.Net;
using System.IO;
namespace DataMiner
{
    public class WebInteractor
    {
        //This region will help in seaching things from google
        #region References
        //http://www.codeproject.com/Articles/37550/Stock-quote-and-chart-from-Yahoo-in-C
        #endregion

        #region Locals
        //private byte[] downloadedData;
        private string currentSearch;
        private Dictionary<Util.TimeType, string> timeFilter = new Dictionary<DataMiner.Util.TimeType, string>();
        #endregion

        #region Constants
        private const string URL_BASE = "http://ichart.yahoo.com/table.csv?s=";
        private const string URL_FROM_DATE = "&a={0}&b={1}&c={2}";
        private const string URL_TO_DATE = "&d={0}&e={1}&f={2}";
        private const string URL_END = "&g=d&ignore=.csv";
        private const int CLOSE_COL = 4;

        private const string CHART_BASE = "http://ichart.yahoo.com/z?s=";
        private const string CHART_TIME = "&t=";
        #endregion


        public WebInteractor()
        {
            timeFilter.Add(Util.Domain.ONE_YEAR, "1y");
            timeFilter.Add(Util.Domain.SIXY_DAYS, "2m");
            timeFilter.Add(Util.Domain.THIRTY_DAYS, "1m");
        }

        public List<double> Search(string search)
        {
            try
            {
                currentSearch = search;

                //Get the date
                int todayDay = System.DateTime.Now.Day;
                int todayMonth = System.DateTime.Now.Month;
                int todayYear = System.DateTime.Now.Year;

                //Create query; note that yahoo indexes months from 0 (why???)
                string query = currentSearch;
                query += String.Format(URL_FROM_DATE, todayMonth - 1, todayDay.ToString(), todayYear - 1);
                query += String.Format(URL_TO_DATE, todayMonth - 1, todayDay.ToString(), todayYear);

                //Create the url for the historical prices
                Uri url = new Uri(URL_BASE + query + URL_END);
                WebRequest webRequest = WebRequest.Create(url);

                //Debugging
                //Console.Write(URL_BASE + query + URL_END + "\n");

                //Craete stream
                StreamReader webStream = new StreamReader(webRequest.GetResponse().GetResponseStream());

                List<double> data = new List<double>();
                string line;

                //Clear header
                if ((line = webStream.ReadLine()) == null)
                {
                    Console.Write("Error: file improperly formatted\n");
                }

                while ((line = webStream.ReadLine()) != null)
                {
                    string[] entries = line.Split(',');
                    double close = double.Parse(entries[CLOSE_COL]);
                    data.Add(close);
                }

                //Debugging
                //for (int i = 0; i < data.Count; i++)
                //{
                //    Console.Write(data[i].ToString() + "\n");
                //}

                webStream.Close();
                return data;
            }
            catch (Exception)
            {
                MessageBox.Show("Error downloading the data.");
                return null;
            }
        }

        public System.Windows.Controls.Image getGraph(Util.TimeType duration)
        {
            if (currentSearch == null)
                return null;
            try
            {
                string query = currentSearch + CHART_TIME;
                query += timeFilter[duration];

                Uri url = new Uri(CHART_BASE + query);
                //Debugging
                //Console.Write(CHART_BASE + query);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                //See the response that yahoo creates
                WebResponse response = request.GetResponse();

                Stream sr = response.GetResponseStream();
                System.Drawing.Image chart = System.Drawing.Image.FromStream(sr);

                //Debugging
                //chart.Save("h:\\chartTest" + query + ".bmp");
             
                sr.Close();
                return convertDrawingImgToWindowImg(chart);

            }
            catch (Exception)
            {
                MessageBox.Show("Error downloading the graph.");
                return null;
            }
        }

        private System.Windows.Controls.Image convertDrawingImgToWindowImg(System.Drawing.Image source)
        {
            // referenced from: http://rohitagarwal24.blogspot.com/2011/04/convert-from-systemdrawingimage-to.html
            // haven't tested
            System.Windows.Controls.Image dst = new System.Windows.Controls.Image();
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(source);

            IntPtr hBitMap = bmp.GetHbitmap();
            System.Windows.Media.ImageSource windowBMP =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitMap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            dst.Source = windowBMP;
            dst.Height = 287;
            dst.Width = 510; // Yay magic numbers!

            return dst;
        }

    }
}
