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
            if (data.Count < 252)
            {
                Console.Write("Error: insufficient data\n");
                return null;
            }
            Dictionary<Util.TimeType, StockInfo> results = new Dictionary<DataMiner.Util.TimeType, StockInfo>();
            data = DailyContGrowthRate(convertToLog(data));

            //Data is arranged newest to oldest
            results.Add(Util.Domain.THIRTY_DAYS, calcStockInfo(data.GetRange(0, Util.Domain.THIRTY - 1)));
            results.Add(Util.Domain.SIXY_DAYS, calcStockInfo(data.GetRange(0, Util.Domain.SIXTY - 1)));
            results.Add(Util.Domain.ONE_YEAR, calcStockInfo(data.GetRange(0, Util.Domain.YEAR - 1)));
            return results;
        }

        private static StockInfo calcStockInfo(List<double> data)
        {
            StockInfo results = new StockInfo();
            //Calcuating DCGR yields 1 less observation
            results.NumDays = data.Count + 1;
            results.Alpha = data.Average();
            results.Beta = StandardDeviation(data, results.Alpha);
            results.Max = data.Max();
            results.Min = data.Min();

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
            data.ForEach(i => Math.Log(i));
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
    }
}
