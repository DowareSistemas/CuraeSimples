using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace NFe.Model
{
    [Serializable]
    public class ide
    {
        [XmlElement(ElementName = "cUF")]
        public string cUF { get; set; }
        public string cNF { get; set; }
        /// <summary>
        /// Natureza da operacao
        /// </summary>
        public string natOp { get; set; }
        /// <summary>
        /// Indicador da forma de pagto
        /// </summary>
        public string indPag { get; set; }
        /// <summary>
        /// Modelo NFE (65 ou 55)
        /// </summary>
        public string mod { get; set; }
        public string serie { get; set; }
        public string nNF { get; set; }
        /// <summary>
        /// Data/Hora Emissao
        /// </summary>
        public string dhEmi { get; set; }
        /// <summary>
        /// Tipo NF
        /// </summary>
        public string tpNF { get; set; }
        public string idDest { get; set; }
        public string cMunFG { get; set; }
        public string tpImp { get; set; }
        public string tpEmis { get; set; }
        public string cDV { get; set; }
        public string tpAmb { get; set; }
        public string finNFe { get; set; }
        public string indFinal { get; set; }
        public string indPres { get; set; }
        public string procEmi { get; set; }
        public string verProc { get; set; }
    }
}
