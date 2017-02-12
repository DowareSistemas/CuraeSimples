using NFe.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace NFe.Controller
{
    public class GeradorNFe
    {
        public void GeraXML(NFe.Model.NFe nfe)
        {
            try
            {
                UTF8Encoding encoding = new UTF8Encoding();
                XmlTextWriter xml = new XmlTextWriter($@"C:\xml\{nfe.Id}-nfe.xml", encoding);
                xml.Formatting = Formatting.Indented;

                xml.WriteStartDocument(); //inicio do documento
                xml.WriteStartElement("NFe", "http://www.portalfiscal.inf.br/nfe");
                xml.WriteStartElement("infNFe");

                xml.WriteStartAttribute("Id");
                xml.WriteString($"NFe{nfe.Id}");
                xml.WriteEndAttribute();

                xml.WriteStartAttribute("versao");//Atributos do Nó
                xml.WriteString("3.10");
                xml.WriteEndAttribute();


                xml.WriteStartElement("ide");//ide


                xml.WriteStartElement("cUF");//Tag codigo Uf
                xml.WriteString(nfe.ide.cUF);// Código da Uf
                xml.WriteEndElement();

                xml.WriteStartElement("cNF");//Tag codigo Uf
                xml.WriteString(nfe.ide.cNF);// Código da Nf
                xml.WriteEndElement();

                xml.WriteStartElement("natOp");//Tag codigo Uf
                xml.WriteString("VENDAS DE PRODUCAO DO ESTABELECIMENTO");// Natureza da Operação
                xml.WriteEndElement();

                xml.WriteStartElement("indPag");//Tag codigo Uf
                xml.WriteString("1");// indicacao de pagamento
                xml.WriteEndElement();

                xml.WriteStartElement("mod");
                xml.WriteString(nfe.ide.mod);
                xml.WriteEndElement();

                xml.WriteStartElement("serie");//Tag codigo Uf
                xml.WriteString("1");// Série da Nota
                xml.WriteEndElement();

                xml.WriteStartElement("nNF");//Tag codigo Uf
                xml.WriteString(nfe.ide.nNF);// Numero da NF
                xml.WriteEndElement();

                xml.WriteStartElement("dhEmi");//Tag codigo Uf
                xml.WriteString(nfe.ide.dhEmi);// Data da Emissão
                xml.WriteEndElement();

                xml.WriteStartElement("tpNF");//Tag codigo Uf
                xml.WriteString(nfe.ide.tpNF);// Tipo da Nf
                xml.WriteEndElement();

                xml.WriteStartElement("idDest");
                xml.WriteString(nfe.ide.idDest);
                xml.WriteEndElement();

                xml.WriteStartElement("cMunFG");//Tag codigo Uf
                xml.WriteString(nfe.ide.cMunFG);// Código do municipio fator Gerador
                xml.WriteEndElement();

                xml.WriteStartElement("tpImp");//Tag codigo Uf
                xml.WriteString(nfe.ide.tpImp);// Tipo de impressao
                xml.WriteEndElement();

                xml.WriteStartElement("tpEmis");//Tag codigo Uf
                xml.WriteString(nfe.ide.tpEmis);// Tipo de Emissao
                xml.WriteEndElement();
           
                xml.WriteStartElement("cDV");//Tag codigo Uf
                xml.WriteString(nfe.ide.cDV);// Código verificador
                xml.WriteEndElement();

                xml.WriteStartElement("tpAmb");//Tag codigo Uf
                xml.WriteString(nfe.ide.tpAmb);// Tipo de ambiente
                xml.WriteEndElement();

                xml.WriteStartElement("finNFe");//Tag codigo Uf
                xml.WriteString(nfe.ide.finNFe);// Finalidade da Nfe
                xml.WriteEndElement();

                xml.WriteStartElement("indFinal");
                xml.WriteString(nfe.ide.indFinal);
                xml.WriteEndElement();

                xml.WriteStartElement("indPres");
                xml.WriteString(nfe.ide.indPres);
                xml.WriteEndElement();

                xml.WriteStartElement("procEmi");//Tag codigo Uf
                xml.WriteString(nfe.ide.procEmi);// Proc Nfe
                xml.WriteEndElement();

                xml.WriteStartElement("verProc");//Tag codigo Uf
                xml.WriteString(nfe.ide.verProc);// Versao da Proc Nfe
                xml.WriteEndElement();
                xml.WriteEndElement();//ide

                xml.WriteStartElement("emit");//ide

                xml.WriteStartElement("CNPJ");//Tag codigo Uf
                xml.WriteString(nfe.emit.CNPJ);// CNPJ Emitente
                xml.WriteEndElement();

                xml.WriteStartElement("xNome");//Tag codigo Uf
                xml.WriteString(nfe.emit.xNome);// Versao da Proc Nfe
                xml.WriteEndElement();

                /*
                xml.WriteStartElement("xFant");//Tag codigo Uf
                xml.WriteString(nfe.emit.xfa);// Versao da Proc Nfe
                xml.WriteEndElement();
                */

                xml.WriteStartElement("enderEmit");//Tag codigo Uf

                xml.WriteStartElement("xLgr");//Tag codigo Uf
                xml.WriteString(nfe.emit.xLogr);// Versao da Proc Nfe
                xml.WriteEndElement();

                xml.WriteStartElement("nro");//Tag codigo Uf
                xml.WriteString(nfe.emit.nro);// Versao da Proc Nfe
                xml.WriteEndElement();

                xml.WriteStartElement("xBairro");//Tag codigo Uf
                xml.WriteString(nfe.emit.xBairro);// Versao da Proc Nfe
                xml.WriteEndElement();

                xml.WriteStartElement("cMun");//Tag codigo Uf
                xml.WriteString(nfe.emit.cMun);// Versao da Proc Nfe
                xml.WriteEndElement();

                xml.WriteStartElement("xMun");//Tag codigo Uf
                xml.WriteString(nfe.emit.xMun);// Versao da Proc Nfe
                xml.WriteEndElement();

                xml.WriteStartElement("UF");//Tag codigo Uf
                xml.WriteString(nfe.emit.UF);// Versao da Proc Nfe
                xml.WriteEndElement();

                xml.WriteStartElement("CEP");//Tag codigo Uf
                xml.WriteString(nfe.emit.CEP);// Versao da Proc Nfe
                xml.WriteEndElement();


                xml.WriteEndElement();//enderEmit

                xml.WriteStartElement("IE");//Tag codigo Uf
                xml.WriteString(nfe.emit.IE);// Versao da Proc Nfe
                xml.WriteEndElement();

                xml.WriteStartElement("CRT");
                xml.WriteString(nfe.emit.CRT);
                xml.WriteEndElement();

                xml.WriteEndElement();//emit

                xml.WriteStartElement("dest");//dest

                xml.WriteStartElement(string.IsNullOrEmpty(nfe.dest.CPF) ? "CNPJ" : "CPF");//Tag codigo Uf
                xml.WriteString(string.IsNullOrEmpty(nfe.dest.CPF) ? nfe.dest.CNPJ : nfe.dest.CPF);// Versao da Proc Nfe
                xml.WriteEndElement();

                xml.WriteStartElement("xNome");//Tag codigo Uf
                xml.WriteString(nfe.dest.xNome);// Versao da Proc Nfe
                xml.WriteEndElement();

                xml.WriteStartElement("enderDest");//enderDest

                xml.WriteStartElement("xLgr");//Tag codigo Uf
                xml.WriteString(nfe.dest.xLogr);// Versao da Proc Nfe
                xml.WriteEndElement();

                xml.WriteStartElement("nro");//Tag codigo Uf
                xml.WriteString(nfe.dest.nro);// Versao da Proc Nfe
                xml.WriteEndElement();

                xml.WriteStartElement("xBairro");//Tag codigo Uf
                xml.WriteString(nfe.dest.xBairro);// Versao da Proc Nfe
                xml.WriteEndElement();

                xml.WriteStartElement("cMun");//Tag codigo Uf
                xml.WriteString(nfe.dest.cMun);// Versao da Proc Nfe
                xml.WriteEndElement();

                xml.WriteStartElement("xMun");//Tag codigo Uf
                xml.WriteString(nfe.dest.xMun);// Versao da Proc Nfe
                xml.WriteEndElement();

                xml.WriteStartElement("UF");//Tag codigo Uf
                xml.WriteString(nfe.dest.UF);// Versao da Proc Nfe
                xml.WriteEndElement();

                xml.WriteStartElement("CEP");//Tag codigo Uf
                xml.WriteString(nfe.dest.CEP);// Versao da Proc Nfe
                xml.WriteEndElement();

                xml.WriteEndElement();//enderDest

                xml.WriteStartElement("indIEDest");//Tag codigo Uf
                xml.WriteString(nfe.dest.indIEDest);// Versao da Proc Nfe
                xml.WriteEndElement();

                xml.WriteEndElement();//dest
                
                foreach(det det in nfe.dets)//Itens da Nota
                {
                    xml.WriteStartElement("det");//Tag codigo Uf
                    xml.WriteStartAttribute("nItem");//Atributos do Nó
                    xml.WriteString(det.nItem);
                    xml.WriteEndAttribute();// finalizando o atributo

                    xml.WriteStartElement("prod");//Tag codigo Uf

                    xml.WriteStartElement("cProd");//Tag codigo Uf
                    xml.WriteString(det.cProd);// Versao da Proc Nfe
                    xml.WriteEndElement();

                    xml.WriteStartElement("cEAN");//Tag codigo Uf
                    xml.WriteString(det.cEAN);
                    xml.WriteEndElement();

                    xml.WriteStartElement("xProd");//Tag codigo Uf
                    xml.WriteString(det.xProd);// Versao da Proc Nfe
                    xml.WriteEndElement();

                    xml.WriteStartElement("NCM");//Tag codigo Uf
                    xml.WriteString(det.NCM);// Versao da Proc Nfe
                    xml.WriteEndElement();

                    xml.WriteStartElement("CFOP");//Tag codigo Uf
                    xml.WriteString(det.CFOP);// Versao da Proc Nfe
                    xml.WriteEndElement();

                    xml.WriteStartElement("uCom");//Tag codigo Uf
                    xml.WriteString(det.uCom);// Versao da Proc Nfe
                    xml.WriteEndElement();

                    xml.WriteStartElement("qCom");//Tag codigo Uf
                    xml.WriteString(det.qCom);// Versao da Proc Nfe
                    xml.WriteEndElement();

                    xml.WriteStartElement("vUnCom");
                    xml.WriteString(det.vUnCom);
                    xml.WriteEndElement();

                    xml.WriteStartElement("vProd");//Tag codigo Uf
                    xml.WriteString(det.vProd);// Versao da Proc Nfe
                    xml.WriteEndElement();

                    xml.WriteStartElement("cEANTrib");//Tag codigo Uf
                    xml.WriteString(det.cEANTrib);
                    xml.WriteEndElement();

                    xml.WriteStartElement("uTrib");//Tag codigo Uf
                    xml.WriteString(det.uTrib);// Versao da Proc Nfe
                    xml.WriteEndElement();

                    xml.WriteStartElement("qTrib");//Tag codigo Uf
                    xml.WriteString(det.qTrib);// Versao da Proc Nfe
                    xml.WriteEndElement();

                    xml.WriteStartElement("vUnTrib");//Tag codigo Uf
                    xml.WriteString(det.vUnTrib);// Versao da Proc Nfe
                    xml.WriteEndElement();

                    xml.WriteStartElement("indTot");
                    xml.WriteString(det.indTot);
                    xml.WriteEndElement();

                    xml.WriteEndElement();//fim roduto

                    xml.WriteStartElement("imposto"); // Imposto
                    xml.WriteStartElement("ICMS");  //Icms
                    xml.WriteStartElement($"ICMS{det.CST}"); //ICMS00

                    xml.WriteStartElement("orig");
                    xml.WriteString(det.orig);
                    xml.WriteEndElement();

                    xml.WriteStartElement("CST");
                    xml.WriteString(det.CST);
                    xml.WriteEndElement();

                    xml.WriteStartElement("modBC");
                    xml.WriteString(det.modBC);
                    xml.WriteEndElement();

                    xml.WriteStartElement("vBC");
                    xml.WriteString(det.vBC);
                    xml.WriteEndElement();

                    xml.WriteStartElement("pICMS");
                    xml.WriteString(det.pICMS);
                    xml.WriteEndElement();

                    xml.WriteStartElement("vICMS");
                    xml.WriteString(det.vICMS);
                    xml.WriteEndElement();


                    xml.WriteEndElement();//fim ICMS00
                    xml.WriteEndElement(); // Fim Icms

                    xml.WriteStartElement("PIS");
                    xml.WriteStartElement("PISNT");
                    xml.WriteStartElement("CST");
                    xml.WriteString("06");
                    xml.WriteEndElement();
                    xml.WriteEndElement();
                    xml.WriteEndElement();

                    xml.WriteStartElement("COFINS");
                    xml.WriteStartElement("COFINSNT");
                    xml.WriteStartElement("CST");
                    xml.WriteString("06");
                    xml.WriteEndElement();
                    xml.WriteEndElement();
                    xml.WriteEndElement();

                    xml.WriteEndElement(); // Fim imposto
                    
                    xml.WriteEndElement(); //fim det
                }

                xml.WriteStartElement("total");
                xml.WriteStartElement("ICMSTot");

                xml.WriteStartElement("vBC");
                xml.WriteString(nfe.total.vBC);
                xml.WriteEndElement();

                xml.WriteStartElement("vICMS");
                xml.WriteString(nfe.total.vICMS);
                xml.WriteEndElement();

                xml.WriteStartElement("vICMSDeson");
                xml.WriteString(nfe.total.vICMSDeson);
                xml.WriteEndElement();

                xml.WriteStartElement("vBCST");
                xml.WriteString(nfe.total.vBCST);
                xml.WriteEndElement();

                xml.WriteStartElement("vST");
                xml.WriteString(nfe.total.vST);
                xml.WriteEndElement();

                xml.WriteStartElement("vProd");
                xml.WriteString(nfe.total.vProd);
                xml.WriteEndElement();

                xml.WriteStartElement("vFrete");
                xml.WriteString(nfe.total.vFrete);
                xml.WriteEndElement();

                xml.WriteStartElement("vSeg");
                xml.WriteString(nfe.total.vSeg);
                xml.WriteEndElement();

                xml.WriteStartElement("vDesc");
                xml.WriteString(nfe.total.vDesc);
                xml.WriteEndElement();

                xml.WriteStartElement("vII");
                xml.WriteString(nfe.total.vII);
                xml.WriteEndElement();

                xml.WriteStartElement("vIPI");
                xml.WriteString(nfe.total.vIPI);
                xml.WriteEndElement();

                xml.WriteStartElement("vPIS");
                xml.WriteString(nfe.total.vPIS);
                xml.WriteEndElement();

                xml.WriteStartElement("vCOFINS");
                xml.WriteString(nfe.total.vCOFINS);
                xml.WriteEndElement();

                xml.WriteStartElement("vOutro");
                xml.WriteString(nfe.total.vOutro);
                xml.WriteEndElement();

                xml.WriteStartElement("vNF");
                xml.WriteString(nfe.total.vNF);
                xml.WriteEndElement();
                
                xml.WriteEndElement();
                xml.WriteEndElement();

                xml.WriteStartElement("transp");
                xml.WriteStartElement("modFrete");
                xml.WriteString(nfe.transp.modFrete);
                xml.WriteEndElement();
                xml.WriteEndElement();

                /*
                xml.WriteStartElement("pag"); //`PAG

                xml.WriteStartElement("tPag");
                xml.WriteString(nfe.pag.tPag);
                xml.WriteEndElement();

                xml.WriteStartElement("vPag");
                xml.WriteString(nfe.pag.vPag);
                xml.WriteEndElement();

                xml.WriteEndElement();//fim PAG
                */

                xml.WriteEndElement();//InfNFE
                xml.WriteEndElement();//Nfe

                xml.Close();
                Console.WriteLine("Arquivo gerado");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
