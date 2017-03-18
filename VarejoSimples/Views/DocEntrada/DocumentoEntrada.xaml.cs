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
using VarejoSimples.Interfaces;
using VarejoSimples.Model;
using VarejoSimples.Views.Movimento;

namespace VarejoSimples.Views.DocEntrada
{
    /// <summary>
    /// Lógica interna para DocumentoEntrada.xaml
    /// </summary>
    public partial class DocumentoEntrada : Window
    {
        private NFe.Model.NFe NFe { get; set; }
        
        private bool CadastrarFornecedor { get; set; }
        private bool AtualizarCustoProdutos { get; set; }
        private bool AdicionarAmarracaoPF { get; set; }
        

        public DocumentoEntrada()
        {
            InitializeComponent();
            
            NFeLoader nfeLoader = new NFeLoader(@"C:\Temp\13140311707347000195650030000004591064552496-nfe.xml");
            NFe = nfeLoader.Load();

            if (NFe.dest.CNPJ != UsuariosController.LojaAtual.Cnpj)
            {
                MessageBoxResult mResult = MessageBox.Show("Esta nota não é destinada a sua empresa. \nSua importação poderá gerar inconsistências na parte financeira do sistema e após executado, esta operação não poderá ser revertida. \n\nTem certeza que deseja continuar?", "AVISO", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (mResult == MessageBoxResult.No)
                    Close();
            }

            txCod_nf.Text = NFe.ide.cNF;
            txTipo_documento.Text = (NFe.ide.mod == "65"
                ? "NF-e"
                : "NFC-e");
            txNumero_nf.Text = NFe.ide.nNF;
            txSerie_nf.Text = NFe.ide.serie;
            txOperacao.Text = NFe.ide.natOp;
            txData_emissao.Text = Convert.ToDateTime(NFe.ide.dhEmi).ToString("dd/MM/yyyy");
            txNome_emit.Text = NFe.emit.xNome;
            txLogradouro.Text = NFe.emit.xLogr;
            txNumero_emit.Text = NFe.emit.nro;
            txUF.Text = NFe.emit.UF;
            txBairro.Text = NFe.emit.xBairro;
            txMunicipio.Text = NFe.emit.xMun;
            txCNPJ.Text = NFe.emit.CNPJ;
            txIE.Text = NFe.emit.IE;

            if (!string.IsNullOrEmpty(NFe.total.vBC))
                txBC_icms.Text = NFe.total.vBC;

            if (!string.IsNullOrEmpty(NFe.total.vICMS))
                txIcms.Text = NFe.total.vICMS;

            if (!string.IsNullOrEmpty(NFe.total.vBCST))
                txBC_icms_st.Text = NFe.total.vBCST;

            if (!string.IsNullOrEmpty(NFe.total.vST))
                txIcms_st.Text = NFe.total.vST;

            if (!string.IsNullOrEmpty(NFe.total.vProd))
                txTotalProd.Text = NFe.total.vProd;

            if (!string.IsNullOrEmpty(NFe.total.vFrete))
                txFrete.Text = NFe.total.vFrete;

            if (!string.IsNullOrEmpty(NFe.total.vDesc))
                txDesconto.Text = NFe.total.vDesc;

            if (!string.IsNullOrEmpty(NFe.total.vIPI))
                txIpi.Text = NFe.total.vIPI;

            if (!string.IsNullOrEmpty(NFe.total.vPIS))
                txPis.Text = NFe.total.vPIS;

            if (!string.IsNullOrEmpty(NFe.total.vCOFINS))
                txCofins.Text = NFe.total.vCOFINS;

            if (!string.IsNullOrEmpty(NFe.total.vOutro))
                txOutros.Text = NFe.total.vOutro;

            if (!string.IsNullOrEmpty(NFe.total.vNF))
                txTotal_nf.Text = NFe.total.vNF;

            List<DetAdapter> listAdp = new List<DetAdapter>();
            NFe.dets.ForEach(e => listAdp.Add(new DetAdapter(e)));

            dataGrid.AplicarPadroes();
            dataGrid.FontSize = 14;
            dataGrid.MinRowHeight = 14;
            dataGrid.ItemsSource = listAdp;
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            ITelaPagamentoMovimento pagamento = new PagamentoRetaguarda();
            pagamento.Exibir(decimal.Parse(NFe.total.vNF.Replace(".", ",")));

            FornecedoresController fController = new FornecedoresController();
            if(!fController.Existe(NFe.emit.CNPJ))
            {
                //TODO fazer o cadastro
            }
            
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
