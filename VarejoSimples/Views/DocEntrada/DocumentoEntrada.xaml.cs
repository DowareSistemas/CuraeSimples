using NFe.Controller;
using NFe.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VarejoSimples.Controller;

namespace VarejoSimples.Views.DocEntrada
{
    /// <summary>
    /// Lógica interna para DocumentoEntrada.xaml
    /// </summary>
    public partial class DocumentoEntrada : Window
    {
        public DocumentoEntrada()
        {
            InitializeComponent();

            NFeLoader nfeLoader = new NFeLoader(@"C:\Temp\13140311707347000195650030000004591064552496-nfe.xml");
            NFe.Model.NFe nfe = nfeLoader.Load();

            if (nfe.dest.CNPJ != UsuariosController.LojaAtual.Cnpj)
            {
                MessageBoxResult mResult = MessageBox.Show("Esta nota não é destinada a sua empresa. \nSua importação poderá gerar inconsistências na parte financeira do sistema e após executado, esta operação não poderá ser revertida. \n\nTem certeza que deseja continuar?", "AVISO", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (mResult == MessageBoxResult.No)
                    Close();
            }

            txCod_nf.Text = nfe.ide.cNF;
            txTipo_documento.Text = (nfe.ide.mod == "65"
                ? "NF-e"
                : "NFC-e");
            txNumero_nf.Text = nfe.ide.nNF;
            txSerie_nf.Text = nfe.ide.serie;
            txOperacao.Text = nfe.ide.natOp;
            txData_emissao.Text = Convert.ToDateTime(nfe.ide.dhEmi).ToString("dd/MM/yyyy");
            txNome_emit.Text = nfe.emit.xNome;
            txLogradouro.Text = nfe.emit.xLogr;
            txNumero_emit.Text = nfe.emit.nro;
            txUF.Text = nfe.emit.UF;
            txBairro.Text = nfe.emit.xBairro;
            txMunicipio.Text = nfe.emit.xMun;
            txCNPJ.Text = nfe.emit.CNPJ;
            txIE.Text = nfe.emit.IE;

            if (!string.IsNullOrEmpty(nfe.total.vBC))
                txBC_icms.Text = nfe.total.vBC;

            if (!string.IsNullOrEmpty(nfe.total.vICMS))
                txIcms.Text = nfe.total.vICMS;

            if (!string.IsNullOrEmpty(nfe.total.vBCST))
                txBC_icms_st.Text = nfe.total.vBCST;

            if (!string.IsNullOrEmpty(nfe.total.vST))
                txIcms_st.Text = nfe.total.vST;

            if (!string.IsNullOrEmpty(nfe.total.vProd))
                txTotalProd.Text = nfe.total.vProd;

            if (!string.IsNullOrEmpty(nfe.total.vFrete))
                txFrete.Text = nfe.total.vFrete;

            if (!string.IsNullOrEmpty(nfe.total.vDesc))
                txDesconto.Text = nfe.total.vDesc;

            if (!string.IsNullOrEmpty(nfe.total.vIPI))
                txIpi.Text = nfe.total.vIPI;

            if (!string.IsNullOrEmpty(nfe.total.vPIS))
                txPis.Text = nfe.total.vPIS;

            if (!string.IsNullOrEmpty(nfe.total.vCOFINS))
                txCofins.Text = nfe.total.vCOFINS;

            if (!string.IsNullOrEmpty(nfe.total.vOutro))
                txOutros.Text = nfe.total.vOutro;

            if (!string.IsNullOrEmpty(nfe.total.vNF))
                txTotal_nf.Text = nfe.total.vNF;

            List<DetAdapter> listAdp = new List<DetAdapter>();
            nfe.dets.ForEach(e => listAdp.Add(new DetAdapter(e)));

            dataGrid.AplicarPadroes();
            dataGrid.FontSize = 14;
            dataGrid.MinRowHeight = 14;
            dataGrid.ItemsSource = listAdp;
        }

        private void txNumero_emit_Copy1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }

    public class DetAdapter
    {
        public string cProd { get; set; }
        public string cEAN { get; set; }
        public string xProd { get; set; }
        public string NCM { get; set; }
        public int CFOP { get; set; }
        public string uCom { get; set; }
        public decimal vUnCom { get; set; }
        public decimal qCom { get; set; }
        public decimal vProd { get; set; }
        public decimal vBC { get; set; }
        public decimal vICMS { get; set; }

        public DetAdapter(NFe.Model.det det)
        {
            cProd = det.cProd;
            cEAN = det.cEAN;
            xProd = det.xProd;
            NCM = det.NCM;
            CFOP = int.Parse(det.CFOP);
            uCom = det.uCom.ToUpper();
            qCom = decimal.Parse(det.qCom.Replace(".", ","));
            vProd = decimal.Parse(det.vProd.Replace(".", ","));
            vUnCom = decimal.Parse(det.vUnCom.Replace(".", ","));
            vICMS = decimal.Parse(det.vICMS.Replace(".", ","));
            vBC = decimal.Parse(det.vBC.Replace(".", ","));
        }
    }
}
