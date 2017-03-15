using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace NFe.Controller
{
    public class NFeLoader
    {
        private NFe.Model.NFe NFe = null;
        private string XmlFile { get; set; }
        public NFeLoader(string xmlFile)
        {
            XmlFile = xmlFile;
        }

        public NFe.Model.NFe Load()
        {
            string dir = XmlFile;
            NFe = new Model.NFe();

            XmlDocument oXml = new XmlDocument();
            oXml.Load(dir);

            XmlNode infNfe = oXml.ChildNodes[1].ChildNodes[0];
            NFe.Id = infNfe.Attributes[0].InnerText;

            List<XmlNode> dets = new List<XmlNode>();

            foreach (XmlNode node in infNfe.ChildNodes)
            {
                if (node.Name.Equals("ide"))
                    Fill_ide(node);

                if (node.Name.Equals("emit"))
                    Fill_emit(node);

                if (node.Name.Equals("dest"))
                    Fill_dest(node);

                if (node.Name.Equals("det"))
                    dets.Add(node);

                if (node.Name.Equals("total"))
                    Fill_total(node);

                if (node.Name.Equals("transp"))
                    Fill_transp(node);

                if (node.Name.Equals("pag"))
                    Fill_pag(node);

                if (node.Name.Equals("infAdic"))
                    Fill_infAdic(node);
            }

            Fill_dets(dets);
            return NFe;
        }

        private void Fill_ide(XmlNode node_ide)
        {
            Model.ide ide = new Model.ide();

            foreach (XmlNode node in node_ide.ChildNodes)
            {
                if (node.Name.Equals("cUF"))
                    ide.cUF = node.InnerText;

                if (node.Name.Equals("cNF"))
                    ide.cNF = node.InnerText;

                if (node.Name.Equals("natOp"))
                    ide.natOp = node.InnerText;

                if (node.Name.Equals("indPag"))
                    ide.indPag = node.InnerText;

                if (node.Name.Equals("mod"))
                    ide.mod = node.InnerText;

                if (node.Name.Equals("serie"))
                    ide.serie = node.InnerText;

                if (node.Name.Equals("nNF"))
                    ide.nNF = node.InnerText;

                if (node.Name.Equals("dhEmi"))
                    ide.dhEmi = node.InnerText;

                if (node.Name.Equals("tpNF"))
                    ide.tpNF = node.InnerText;

                if (node.Name.Equals("idDest"))
                    ide.idDest = node.InnerText;

                if (node.Name.Equals("cMunFG"))
                    ide.cMunFG = node.InnerText;

                if (node.Name.Equals("tpImp"))
                    ide.tpImp = node.InnerText;

                if (node.Name.Equals("tpEmis"))
                    ide.tpEmis = node.InnerText;

                if (node.Name.Equals("cDV"))
                    ide.cDV = node.InnerText;

                if (node.Name.Equals("tpAmb"))
                    ide.tpAmb = node.InnerText;

                if (node.Name.Equals("finNFe"))
                    ide.finNFe = node.InnerText;

                if (node.Name.Equals("indFinal"))
                    ide.indFinal = node.InnerText;

                if (node.Name.Equals("indPres"))
                    ide.indPres = node.InnerText;

                if (node.Name.Equals("procEmi"))
                    ide.procEmi = node.InnerText;

                if (node.Name.Equals("verProc"))
                    ide.verProc = node.InnerText;
            }

            NFe.ide = ide;
        }

        private void Fill_emit(XmlNode node_emit)
        {
            Model.emit emit = new Model.emit();

            foreach (XmlNode node in node_emit.ChildNodes)
            {
                if (node.Name.Equals("CNPJ"))
                    emit.CNPJ = node.InnerText;

                if (node.Name.Equals("xNome"))
                    emit.xNome = node.InnerText;

                if (node.Name.Equals("enderEmit"))
                {
                    foreach (XmlNode endNode in node.ChildNodes)
                    {
                        if (endNode.Name.Equals("xLogr"))
                            emit.xLogr = endNode.InnerText;

                        if (endNode.Name.Equals("nro"))
                            emit.nro = endNode.InnerText;

                        if (endNode.Name.Equals("xBairro"))
                            emit.xBairro = endNode.InnerText;

                        if (endNode.Name.Equals("cMun"))
                            emit.cMun = endNode.InnerText;

                        if (endNode.Name.Equals("UF"))
                            emit.UF = endNode.InnerText;

                        if (endNode.Name.Equals("CEP"))
                            emit.CEP = endNode.InnerText;
                    }
                }

                if (node.Name.Equals("IE"))
                    emit.IE = node.InnerText;

                if (node.Name.Equals("CRT"))
                    emit.CRT = node.InnerText;

                NFe.emit = emit;
            }
        }

        private void Fill_dest(XmlNode node_dest)
        {
            Model.dest dest = new Model.dest();

            foreach (XmlNode node in node_dest.ChildNodes)
            {
                if (node.Name.Equals("CNPJ"))
                    dest.CNPJ = node.InnerText;

                if (node.Name.Equals("xName"))
                    dest.xNome = node.InnerText;

                if (node.Name.Equals("enderDest"))
                {
                    foreach (XmlNode enderNode in node.ChildNodes)
                    {
                        if (enderNode.Name.Equals("xLogr"))
                            dest.xLogr = enderNode.InnerText;

                        if (enderNode.Name.Equals("nro"))
                            dest.nro = enderNode.InnerText;

                        if (enderNode.Name.Equals("xBairro"))
                            dest.xBairro = enderNode.InnerText;

                        if (enderNode.Name.Equals("cMun"))
                            dest.cMun = enderNode.InnerText;

                        if (enderNode.Name.Equals("UF"))
                            dest.UF = enderNode.InnerText;
                    }

                    if (node.Name.Equals("indIEDest"))
                        dest.indIEDest = node.InnerText;
                }

                NFe.dest = dest;
            }
        }

        //são vaaaaaaarios dets que voce precisa popular nesta caralha
        private void Fill_dets(List<XmlNode> dets)
        {
            NFe.dets = new List<Model.det>();

            foreach (XmlNode det in dets)
            {
                Model.det nfeDet = new Model.det();
                foreach (XmlNode detNode in det.ChildNodes)
                {
                    #region NO PROD
                    //NÓ PROD
                    if (detNode.Name.Equals("prod"))
                    {
                        foreach (XmlNode pNode in detNode.ChildNodes)
                        {
                            if (pNode.Name.Equals("cProd"))
                                nfeDet.cProd = pNode.InnerText;

                            if (pNode.Name.Equals("cEAN"))
                                nfeDet.cEAN = pNode.InnerText;

                            if (pNode.Name.Equals("xProd"))
                                nfeDet.xProd = pNode.InnerText;

                            if (pNode.Name.Equals("NCM"))
                                nfeDet.NCM = pNode.InnerText;

                            if (pNode.Name.Equals("CFOP"))
                                nfeDet.CFOP = pNode.InnerText;

                            if (pNode.Name.Equals("uCom"))
                                nfeDet.uCom = pNode.InnerText;

                            if (pNode.Name.Equals("qCom"))
                                nfeDet.qCom = pNode.InnerText;

                            if (pNode.Name.Equals("vUnCom"))
                                nfeDet.vUnCom = pNode.InnerText;

                            if (pNode.Name.Equals("vProd"))
                                nfeDet.vProd = pNode.InnerText;

                            if (pNode.Name.Equals("cEANTrib"))
                                nfeDet.cEANTrib = pNode.InnerText;

                            if (pNode.Name.Equals("uTrib"))
                                nfeDet.uTrib = pNode.InnerText;

                            if (pNode.Name.Equals("qTrib"))
                                nfeDet.qTrib = pNode.InnerText;

                            if (pNode.Name.Equals("vUnTrib"))
                                nfeDet.vUnTrib = pNode.InnerText;

                            if (pNode.Name.Equals("indTot"))
                                nfeDet.indTot = pNode.InnerText;
                        }
                    }
                    #endregion

                    #region NO IMPOSTO
                    if (detNode.Name.Equals("imposto"))
                    {
                        foreach (XmlNode iNode in detNode)
                        {
                            #region ICMS
                            if (iNode.Name.Equals("ICMS"))
                            {
                                /*
                                   Normalmente, a tag <ICMS> contém um filho unico,
                                   podendo vir no XML como ICMS00 ou semelhante.
                                   Neste caso, pegamos os elementos filhos, apartir do primeiro
                                   filho do <ICMS> (tags "netas" do <ICMS>) atravez
                                   do iNode.FirstChild.ChildNodes
                                 */
                                foreach (XmlNode icmsNode in iNode.FirstChild.ChildNodes)
                                {
                                    if (icmsNode.Name.Equals("orig"))
                                        nfeDet.orig = icmsNode.InnerText;

                                    if (icmsNode.Name.Equals("CST"))
                                        nfeDet.CST = icmsNode.InnerText;

                                    if (icmsNode.Name.Equals("modBC"))
                                        nfeDet.modBC = icmsNode.InnerText;

                                    if (icmsNode.Name.Equals("vBC"))
                                        nfeDet.vBC = icmsNode.InnerText;

                                    if (icmsNode.Name.Equals("pICMS"))
                                        nfeDet.pICMS = icmsNode.InnerText;

                                    if (icmsNode.Name.Equals("vICMS"))
                                        nfeDet.vICMS = icmsNode.InnerText;
                                }
                            }
                            #endregion

                            #region PIS
                            if (iNode.Name.Equals("PIS"))
                            {
                                foreach(XmlNode pisNode in iNode.FirstChild.ChildNodes)
                                {
                                    if (pisNode.Name.Equals("PISNT"))
                                        nfeDet.PIS_CST = pisNode.InnerText;
                                }
                            }
                            #endregion

                            #region COFINS
                            if (iNode.Name.Equals("COFINS"))
                            {
                                foreach (XmlNode pisNode in iNode.FirstChild.ChildNodes)
                                {
                                    if (pisNode.Name.Equals("COFINSNT"))
                                        nfeDet.COFINS_CST = pisNode.InnerText;
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion
                }
                NFe.dets.Add(nfeDet);
            }
        }

        private void Fill_total(XmlNode node_total)
        {
            Model.total total = new Model.total();

            foreach(XmlNode node in node_total.FirstChild.ChildNodes)
            {
                if (node.Name.Equals("vBC"))
                    total.vBC = node.InnerText;

                if (node.Name.Equals("vICMS"))
                    total.vICMS = node.InnerText;

                if (node.Name.Equals("vICMSDeson"))
                    total.vICMSDeson = node.InnerText;

                if (node.Name.Equals("vBCST"))
                    total.vBCST = node.InnerText;

                if (node.Name.Equals("vST"))
                    total.vST = node.InnerText;

                if (node.Name.Equals("vProd"))
                    total.vProd = node.InnerText;

                if (node.Name.Equals("vFrete"))
                    total.vFrete = node.InnerText;

                if (node.Name.Equals("vSeg"))
                    total.vSeg = node.InnerText;

                if (node.Name.Equals("vDesc"))
                    total.vDesc = node.InnerText;

                if (node.Name.Equals("vII"))
                    total.vII = node.InnerText;

                if (node.Name.Equals("vIPI"))
                    total.vIPI = node.InnerText;

                if (node.Name.Equals("vPIS"))
                    total.vPIS = node.InnerText;

                if (node.Name.Equals("vCOFINS"))
                    total.vCOFINS = node.InnerText;

                if (node.Name.Equals("vOutro"))
                    total.vOutro = node.InnerText;

                if (node.Name.Equals("vNF"))
                    total.vNF = node.InnerText;
            }

            NFe.total = total;
        }

        private void Fill_transp(XmlNode node_transp)
        {
            Model.transp transp = new Model.transp();

            foreach(XmlNode node in node_transp.ChildNodes)
            {
                if (node.Name.Equals("modFrete"))
                    transp.modFrete = node.InnerText;
            }

            NFe.transp = transp;
        }

        private void Fill_pag(XmlNode pag)
        {

        }

        private void Fill_infAdic(XmlNode infAdic)
        {

        }
    }
}