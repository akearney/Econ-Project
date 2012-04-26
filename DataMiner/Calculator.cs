using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataMiner
{
    public static class Calculator
    {

        // CAUTION!!!!!!
        // once loading in data is complete, this will need to be examined to make sure
        //that the order that the information is handled in is correct.
        //these spots are marked with a causion below
        public static Dictionary<Util.TimeType, StockInfo> calculateAllStockInfo(List<double> data)
        {
            if (data.Count < 252)
                return null;
            Dictionary<Util.TimeType, StockInfo> results = new Dictionary<DataMiner.Util.TimeType, StockInfo>();
            data = DailyContGrowthRate(convertToLog(data));

            // CAUTION!!!!!! get range could be at the beginning or end depending on parsing
            results.Add(Util.Domain.THIRTY_DAYS, calcStockInfo(data.GetRange(0,Util.Domain.THIRTY)));
            results.Add(Util.Domain.SIXY_DAYS, calcStockInfo(data.GetRange(0, Util.Domain.SIXTY)));
            results.Add(Util.Domain.ONE_YEAR, calcStockInfo(data.GetRange(0, Util.Domain.YEAR)));
            return results;
        }

        private static StockInfo calcStockInfo(List<double> data)
        {
            StockInfo results = new StockInfo();
            results.NumDays = data.Count;
            results.Average = data.Average();
            results.Stdev = StandardDeviation(data);
            results.Max = data.Max();
            results.Min = data.Min();

            List<double> NormDCGR = NormalizedDailyContGrowthRate(data, results.Average, results.Stdev);
            results.MaxNormDCGR = NormDCGR.Max();
            results.MinNormDCGR = NormDCGR.Min();

            return results;
        }
        /// <summary>
        /// This is code found at 
        /// http://stackoverflow.com/questions/895929/how-do-i-determine-the-standard-deviation-stddev-of-a-set-of-values
        /// and not checked for correctness
        /// </summary>
        /// <param name="valueList"></param>
        /// <returns></returns>
        private static double StandardDeviation(List<double> valueList)
        {
            double M = 0.0;
            double S = 0.0;
            int k = 1;
            foreach (double value in valueList)
            {
                double tmpM = M;
                M += (value - tmpM) / k;
                S += (value - tmpM) * (value - M);
                k++;
            }
            return Math.Sqrt(S / (k - 1));
        }

        private static List<double> convertToLog(List<double> data)
        {
            data.ForEach(i => Math.Log(i, Math.E));
            return data;
        }

        private static List<double> DailyContGrowthRate(List<double> logData)
        {
            List<double> DCGR = new List<double>();
            // CAUTION!!!!!! difference in cont growth rate is either one direction or the other
            for (int i = 1; i < logData.Count; i++)
            {
                DCGR.Add(logData[i] - logData[i - 1]);
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
