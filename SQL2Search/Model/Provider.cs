using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQL2Search.Model
{
    [Serializable]
    public class Provider
    {
        public string Name { get; set; }
        public string Website { get; set; }
        public string Signature { get; set; }
    }
}
