using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataMiner
{
    public static class Calculator
    {
        public static Dictionary<Util.TimeType, StockInfo> calculateAllStockInfo(List<double> data)
        {
            double spot = data[0];
            if (data.Count < 252)
            {
                Console.Write("Error: insufficient data\n");
                return null;
            }
            Dictionary<Util.TimeType, StockInfo> results = new Dictionary<DataMiner.Util.TimeType, StockInfo>();
            data = DailyContGrowthRate(convertToLog(data));

            //Data is arranged newest to oldest
            results.Add(Util.Domain.THIRTY_DAYS, calcStockInfo(data.GetRange(0, Util.Domain.THIRTY - 1), spot));
            results.Add(Util.Domain.SIXY_DAYS, calcStockInfo(data.GetRange(0, Util.Domain.SIXTY - 1), spot));
            results.Add(Util.Domain.ONE_YEAR, calcStockInfo(data.GetRange(0, Util.Domain.YEAR - 1), spot));
            return results;
        }

        private static StockInfo calcStockInfo(List<double> data, double spotPrice)
        {
            StockInfo results = new StockInfo();
            //Calcuating DCGR yields 1 less observation
            results.NumDays = data.Count + 1;
            results.Alpha = data.Average();
            results.Beta = StandardDeviation(data, results.Alpha);
            results.Max = data.Max();
            results.Min = data.Min();
            results.SpotPrice = spotPrice;

            List<double> NormDCGR = NormalizedDailyContGrowthRate(data, results.Alpha, results.Beta);
            results.MaxNormDCGR = NormDCGR.Max();
            results.MinNormDCGR = NormDCGR.Min();

            return results;
        }

        private static double StandardDeviation(List<double> valueList, double average)
        {
            double sum = 0;
            int i = 1;
            foreach (double value in valueList)
            {
                double diff = value - average;
                sum += Math.Pow(diff, 2);
                i++;
            }
            return Math.Sqrt(sum / (i - 1));
        }

        private static List<double> convertToLog(List<double> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                data[i] = Math.Log(data[i]);
            }
            //Debugging
            //for (int i = 0; i < data.Count; i++)
            //{
            //    Console.Write(data[i].ToString() + "\n");
            //}
            return data;
        }

        private static List<double> DailyContGrowthRate(List<double> logData)
        {
            List<double> DCGR = new List<double>();
            // Data stores the most recent first
            for (int i = 0; i < logData.Count - 1; i++)
            {
                DCGR.Add(logData[i] - logData[i + 1]);
            }
            return DCGR;

        }
        private static List<double> NormalizedDailyContGrowthRate(List<double> DCGR, double avg, double stdev)
        {
            List<double> NormDCGR = new List<double>();
            for (int i = 0; i < DCGR.Count; i++)
            {
                NormDCGR.Add((DCGR[i] - avg)/stdev);
            }
            return NormDCGR;
        }

        public static double probabilityCalculator(double spotPrice, double strikePrice, double alpha, double beta, int dtm)
        {
            //Debugging
            //Console.Write("spot, strike, alpha, beta, dtm: " + spotPrice.ToString() + " " + strikePrice.ToString() + " " + alpha.ToString() + " " + beta.ToString() + " " + dtm.ToString());
            double growthRateToStrike = Math.Log(spotPrice / strikePrice);
            double timeAdjBeta = beta * Math.Sqrt((double) dtm);

            return cumNormDist(growthRateToStrike, alpha, timeAdjBeta);
        }

        private static double cumNormDist(double value, double mean, double stdDev)
        {
            double zValue = (value - mean) / stdDev;
            return stdCumNormDist(zValue);
        }

        private static double stdCumNormDist(double value)
        {
            // taken from: http://www.johndcook.com/csharp_phi.html
            // completely and utterly untested

            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;

            int sign = 1;
            if (value < 0)
                sign = -1;
            value = Math.Abs(value) / Math.Sqrt(2.0);

            double t = 1.0 / (1.0 + p * value);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-value * value);

            return 0.5 * (1.0 + sign * y);
        }
    }
}
