using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace NFe.Model
{
    [Serializable]
    [XmlRootAttribute("NFe", Namespace = "http://www.portalfiscal.inf.br/nfe", IsNullable = false)]
    public class NFe
    {
        public string Id { get; set; }
        public ide ide { get; set; }
        public emit emit { get; set; }
        public dest dest { get; set; }
        public List<det> dets { get; set; }
        public total total { get; set; }
        public transp transp { get; set; }
        public pag pag { get; set; }
        public infAdic infAdic { get; set; }
    }
}
