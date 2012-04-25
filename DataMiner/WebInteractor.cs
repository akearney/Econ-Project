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
        private const string URL_BASE = "http://finance.yahoo.com/q/hp?s=";
        private const string URL_END = "+Historical+Prices";

        private const string CHART_BASE = "http://chart.finance.yahoo.com/c/1y/";
        #endregion

        public WebInteractor()
        {
        }

        public string Search(string search)
        {
            currentSearch = search;
            //Create the url for the historical prices
            Uri url = new Uri(URL_BASE + search + URL_END);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            //See the response that yahoo creates
            WebResponse response = request.GetResponse();

            StreamReader sr = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);

            string result = sr.ReadToEnd();
            sr.Close();
            response.Close();
            return result;

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
