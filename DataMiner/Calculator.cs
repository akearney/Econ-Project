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
                return null;
            Dictionary<Util.TimeType, StockInfo> results = new Dictionary<DataMiner.Util.TimeType, StockInfo>();
            results.Add(Util.Domain.THIRTY_DAYS, calcStockInfo(data.GetRange(0,Util.Domain.THIRTY)));
            results.Add(Util.Domain.SIXY_DAYS, calcStockInfo(data.GetRange(0, Util.Domain.SIXTY)));
            results.Add(Util.Domain.ONE_YEAR, calcStockInfo(data.GetRange(0, Util.Domain.YEAR)));
            return results;
        }

        private static StockInfo calcStockInfo(List<double> data)
        {
            StockInfo results = new StockInfo();
            return results;
        }
    }
}
