using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataMiner.Util
{
    public class TimeType
    {
        private string _type;
        public TimeType(string type)
        {
            _type = type;
        }
        public override bool Equals (object other)
        {
            return _type.Equals((other as TimeType).MyType);
        }
        public string MyType
        {
            get
            {
                return _type;
            }
        }
    }
}
