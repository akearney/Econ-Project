using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace DataMiner.Util
{
    public static class Domain
    {
        private const string _thirtydays = "ThirtyDays";
        private const string _sixtydays = "SixtyDays";
        private const string _oneyear = "OneYear";

        public const int THIRTY = 30;
        public const int SIXTY = 60;
        public const int YEAR = 252;
        public const string NEW_SEARCH = "NewSearch";

        public static TimeType THIRTY_DAYS = new TimeType(_thirtydays);
        public static TimeType SIXY_DAYS = new TimeType(_sixtydays);
        public static TimeType ONE_YEAR = new TimeType(_oneyear);

        public static List<TimeType> TIMES = new List<TimeType>() { THIRTY_DAYS, SIXY_DAYS, ONE_YEAR };

        public static TimeType getTime(TabItem clicked)
        {
            switch (clicked.Name)
            {
                case (_thirtydays): return THIRTY_DAYS;
                case (_sixtydays): return SIXY_DAYS;
                case (_oneyear): return ONE_YEAR;
                default: return new TimeType(clicked.Name);
            }
        }
    }
}
