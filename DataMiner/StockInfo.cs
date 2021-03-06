﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Controls;

namespace DataMiner
{
    public class StockInfo
    {
        public Image Graph {get; set;}
        public int NumDays { get; set; }
        public double Alpha { get; set; }
        public double Beta { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double MinNormDCGR { get; set; }
        public double MaxNormDCGR { get; set; }
        public double SpotPrice { get; set; }
        //OtherStuff
    }
}
