using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;
using System.Windows.Controls;
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
        private byte[] downloadedData;
        private string currentSearch;
        #endregion

        #region Constants
        private const string URL_BASE = "http://ichart.yahoo.com/table.csv?s=";
        private const string URL_FROM_DATE = "&a={0}&b={1}&c={2}";
        private const string URL_TO_DATE = "&d={0}&e={1}&f={2}";
        private const string URL_END = "&g=d&ignore=.csv";
        private const int CLOSE_COL = 4;

        private const string CHART_BASE = "http://chart.finance.yahoo.com/c/1y/";
        #endregion

        public WebInteractor()
        {
        }

        public List<double> Search(string search)
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
            //for (int i = 0; i < result.Count; i++)
            //{
            //    Console.Write(data[i].ToString() + "\n");
            //}

            webStream.Close();
            return data;
        }

        public Image getYearGraph()
        {
            if (currentSearch == null)
                return null;
            try
            {
                Uri url = new Uri(CHART_BASE + currentSearch);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                //See the response that yahoo creates
                WebResponse response = request.GetResponse();

                Stream sr = response.GetResponseStream();
                //Download in chuncks
                byte[] buffer = new byte[1024];

                //Get Total Size
                int dataLength = (int)response.ContentLength;

                MemoryStream stream = new MemoryStream();

                while (true)
                {
                    //Try to read the data
                    int bytesRead = sr.Read(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                    {
                        break;
                    }
                    else
                    {
                        //Write the downloaded data
                        stream.Write(buffer, 0, bytesRead);
                    }
                }
                //Convert the downloaded stream to a byte array
                downloadedData = stream.ToArray();
                System.Windows.Media.Imaging.PngBitmapDecoder decoder =
                    new System.Windows.Media.Imaging.PngBitmapDecoder(stream, System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat, System.Windows.Media.Imaging.BitmapCacheOption.Default);
                System.Windows.Media.Imaging.BitmapSource bitmapSource = decoder.Frames[0];
                stream.Close();
                                // Draw the Image
                Image myImage = new Image();
                myImage.Source = bitmapSource;
                myImage.Margin = new Thickness(20);
      
                return myImage;
            }
            catch (Exception)
            {
                MessageBox.Show("Error downloading the graph");
                return null;
            }
        }

    }
}
