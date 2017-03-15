using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFe.Model
{
    public class det
    {
        public string nItem { get; set; }
        /// <summary>
        /// Código do produto
        /// </summary>
        public string cProd { get; set; }
        public string cEAN { get; set; }
        /// <summary>
        /// EAN
        /// </summary>
        public string xProd { get; set; }
        public string NCM { get; set; } 
        public string CFOP { get; set; }

        /// <summary>
        /// Unidade comercializada
        /// </summary>
        public string uCom { get; set; }
        public string qCom { get; set; }
        public string vUnCom { get; set; }
        public string vProd { get; set; }
        public string cEANTrib { get; set; }
        public string uTrib { get; set; }
        public string qTrib { get; set; }
        public string vUnTrib { get; set; }
        public string indTot { get; set; }
        public string orig { get; set; }
        public string CST { get; set; }
        public string modBC { get; set; }
        public string vBC { get; set; }
        public string pICMS { get; set; }
        public string vICMS { get; set; }
        public string vFrete { get; set; }
        public string vSeg { get; set; }
        public string vDesc { get; set; }
        public string vOutro { get; set; }
        public string PIS_CST { get; internal set; }
        public string COFINS_CST { get; internal set; }
    }
}
