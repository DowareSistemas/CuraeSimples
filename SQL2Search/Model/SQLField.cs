using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQL2Search.Model
{
    [Serializable]
    public class SQLField
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
        public object Value { get; set; }
        public MatchMode MachMode { get; set; }

        public enum MatchMode
        {
            EXACT = 0,
            START = 1,
            END = 2,
            ANYWHERE = 3
        }
    }
}
