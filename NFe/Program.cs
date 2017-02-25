using NFe.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFe
{
    class Program
    {
        public static string digito(string chave)
        {
            int digitoRetorno;
            int soma = 0;
            int resto = 0;
            int[] peso = { 4, 3, 2, 9, 8, 7, 6, 5 };

            for (int i = 0; i < chave.Length; i++)
            {
                soma += peso[i % 8] * (int.Parse(chave.Substring(i, 1)));
            }

            resto = soma % 11;
            if (resto == 0 || resto == 1)
            {
                digitoRetorno = 0;
            }
            else
            {
                digitoRetorno = 11 - resto;
            }

            return  digitoRetorno.ToString();
        }

        static void Main(string[] args)
        {
            string uf = "33";
            string aamm = DateTime.Now.ToString("yyMM");
            string cnpj = "24203304000114";
            string modelo = "55";
            string serie = "001";
            string numeroNf = "000000098";
            string emissao = "1";
            string codigo = "00000008";

            string chave = uf + aamm + cnpj + modelo + serie + numeroNf + emissao + codigo;
            string dv = digito(chave);
            chave += dv;

            NFe.Model.NFe nfe = new Model.NFe();
            nfe.Id = chave;
            nfe.ide = new Model.ide()
            {
                cUF = "33",
                cNF = "00000008",
                natOp = "Venda",
                indPag = "0",
                mod = "55",
                serie = "1",
                nNF = "98",
                dhEmi = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss-02:00"),
                tpNF = "1",
                idDest = "1",
                cMunFG = "3306305",
                tpImp = "0",
                tpEmis = "1",
                cDV = dv,
                tpAmb = "2",
                finNFe = "1",
                indFinal = "1",
                indPres = "1",
                procEmi = "0",
                verProc = "1.0"
            };

            nfe.emit = new Model.emit()
            {
                CNPJ = "24203304000114",
                xNome = "EMERSON TINOCO DE ALMEIDA 15982881724",
                xLogr = "RUA AIMORES",
                nro = "15",
                xBairro = "Retiro",
                cMun = "3306305",
                xMun = "VOLTA REDONDA",
                UF = "RJ",
                CEP = "27275350",
                IE = "ISENTO",
                CRT = "3"
            };

            nfe.dest = new Model.dest()
            {
                CPF = "17132107704",
                xNome = "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                xLogr = "Rua Vogue",
                nro = "106",
                xBairro = "Retiro",
                cMun = "3306305",
                xMun = "Volta Redonda",
                UF = "RJ",
                CEP = "27281440",
                indIEDest = "2",
            };

            nfe.dets = new List<Model.det>();
            nfe.dets.Add(new Model.det()
            {
                nItem = "1",
                cProd = "1",
                cEAN = "7891000315507",
                xProd = "NOTA FISCAL EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                NCM = "13021190",
                CFOP = "5102",
                uCom = "L",
                qCom = "1.0000",
                vUnCom = "10.00",
                vProd = "10.00",
                cEANTrib = "7891000315507",
                uTrib = "L",
                qTrib = "1.0000",
                pICMS = "17",
                vUnTrib = "10.00",
                indTot = "1",
                CST = "00",
                orig = "0",
                modBC = "3",
                vBC = "10.00",
                vICMS = "1.70"
            });

            nfe.total = new Model.total()
            {
                vBC = "10.00",
                vICMS = "1.70",
                vICMSDeson = "0",
                vBCST = "0.00",
                vST = "0.00",
                vProd = "10.00",
                vFrete = "0.00",
                vSeg = "0.00",
                vDesc = "0.00",
                vII = "0.00",
                vIPI = "0.00",
                vPIS = "0.00",
                vCOFINS = "0.00",
                vOutro = "0.00",
                vNF = "10.00",
            };

            nfe.transp = new Model.transp()
            {
                modFrete = "9"
            };

            nfe.pag = new Model.pag()
            {
                tPag = "01",
                vPag = "10.00"
            };

            nfe.infAdic = new Model.infAdic()
            {
                infCpl = "Powered by Doware - Curae ERP"
            };

            GeradorNFe gerador = new GeradorNFe();
            gerador.GeraXML(nfe);
            Console.ReadKey();
        }
    }
}
