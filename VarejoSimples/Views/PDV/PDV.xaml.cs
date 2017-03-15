using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VarejoSimples.Controller;
using VarejoSimples.Enums;
using VarejoSimples.Model;
using VarejoSimples.Views.Produto;
using VarejoSimples.Views.VendaRapida;

namespace VarejoSimples.Views.PDV
{
    /// <summary>
    /// Lógica interna para PDV.xaml
    /// </summary>
    public partial class PDV : Window
    {
        private bool VendaAberta { get; set; }
        private PainelItensVenda PainelVenda = null;

        private Tipos_movimento Tipo_movimento_id_venda { get; set; }
        private Tipos_movimento Tipo_movimento_id_devolucao { get; set; }
        Tipo_operacao_atual Operacao_atual { get; set; }

        List<KeyValuePair<int, Formas_pagamento>> Atalhos_pagamentos { get; set; }

        public PDV()
        {
            InitializeComponent();

            VendaAberta = false;
            GridContainer.Children.Add(new LogoEmpresa());
            Atalhos_pagamentos = new List<KeyValuePair<int, Formas_pagamento>>();
            Operacao_atual = Tipo_operacao_atual.VENDA;
            Setup();
        }

        private void Setup()
        {
            string parametroAtual = "";

            try
            {
                ParametrosController parametros = new ParametrosController();
                Tipos_movimentoController tmvController = new Tipos_movimentoController();
                Formas_pagamentoController fpgController = new Formas_pagamentoController();

                #region PAGAMENTOS
                List<Parametros> parametrosPagamentos = parametros.ParametrosPagamentoPDV();
               
                foreach(Parametros param in parametrosPagamentos)
                {
                    if (param.Valor == null || param.Valor == "0" || param.Valor == "" || param.Valor == "NA")
                        continue;

                    Formas_pagamento fpg = fpgController.Find(int.Parse(param.Valor));
                    if (fpg == null)
                        continue;

                    Atalhos_pagamentos.Add(new KeyValuePair<int, Formas_pagamento>(int.Parse(param.Nome.Replace("PDV_F", string.Empty)), fpg));
                }
                #endregion

                #region TMV VENDA
                Parametros pTmvVenda = parametros.FindParametroLojaAtual("TMV_VNDPDV", true);
                parametroAtual = "TMV_VNDPDV";

                if (pTmvVenda == null)
                {
                    pTmvVenda = parametros.FindParametroLojaAtual("TMV_VNDPDV", false);
                    if (pTmvVenda == null)
                    {
                        MessageBox.Show("Não é possível iniciar o PDV porque o parâmetro de sistema 'TMV_VNDPDV' não foi atribuido para este computador. \nAcione o suporte Doware.", "Erro de configuração", MessageBoxButton.OK, MessageBoxImage.Error);
                        Close();
                    }
                }

                if (pTmvVenda.Valor == null || pTmvVenda.Valor == "0" || pTmvVenda.Valor == "")
                {
                    MessageBox.Show("O parâmetro de sistema 'TMV_VNDPDV' não foi informado corretamente ou não foi reconhecido. \nAcione o suporte Doware.", "Erro de configuração", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close(); ;
                }

                Tipos_movimento tmvVenda = tmvController.Find(int.Parse(pTmvVenda.Valor));
                if (tmvVenda == null)
                {
                    MessageBox.Show("O tipo de movimento para VENDA informado no parâmetro 'TMV_VNDPDV' não existe. \nAcione o suporte Doware.", "Erro de configuração", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }

                if (tmvVenda.Movimentacao_itens != (int)Tipo_movimentacao.SAIDA)
                {
                    MessageBox.Show(@"Não é possível iniciar o PDV por que o Tipo de Movimento designado para operações de 
de VENDA no parâmetro 'TMV_VNDPDV' não é compatível. A operação de VENDA requer um Tipo de Movimento cujo
a movimentação de produtos é SAIDA.
Acione o suporte Doware.", "Erro de configuração", MessageBoxButton.OK, MessageBoxImage.Error);

                    Close();
                }

                Tipo_movimento_id_venda = tmvVenda;
                #endregion

                #region TMV DEVOLUCAO
                Parametros pDevolucao = parametros.FindParametroLojaAtual("TMV_DEVPDV", true);
                parametroAtual = "TMV_DEVPDV";
                if (pDevolucao == null)
                {
                    pDevolucao = parametros.FindParametroLojaAtual("TMV_DEVPDV", false);
                    if (pDevolucao == null)
                    {
                        MessageBox.Show("Não é possivel iniciar o PDV porque o parâmetro de sistema 'TMV_DEVPDV' não foi atribuido para este computador. \nAcione o suporte Doware.", "Erro de configuração", MessageBoxButton.OK, MessageBoxImage.Error);
                        Close();
                    }
                }

                if (pDevolucao.Valor == null || pDevolucao.Valor == "0" || pDevolucao.Valor == "")
                {
                    MessageBox.Show("Não é possível iniciar o PDV porque o parâmetro de sistema 'TMV_DEVPDV' não foi atribuido ou seu valor não foi reconhecido. \nAcione o suporte Doware.", "Erro de configuração", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }

                Tipos_movimento tmvDevolucao = tmvController.Find(int.Parse(pDevolucao.Valor));
                if (tmvDevolucao == null)
                {
                    MessageBox.Show("Não é possível iniciar o PDV porque o Tipo de Movimento informado no parâmetro de sistema 'TMV_DEVPDV' não existe. \nAcione o suporte Doware.", "Erro de configuração", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }

                if (tmvDevolucao.Movimentacao_itens != (int)Tipo_movimentacao.ENTRADA)
                {
                    MessageBox.Show(@"Não é possível iniciar o PDV por que o Tipo de Movimento designado para operações de 
de DEVOLUÇÃO no parâmetro 'TMV_DEVPDV' não é compatível. A operação de DEVOLUÇÃO requer um Tipo de Movimento cujo
a movimentação de produtos é ENTRADA.
Acione o suporte Doware.", "Erro de configuração", MessageBoxButton.OK, MessageBoxImage.Error);

                    Close();
                }

                Tipo_movimento_id_devolucao = tmvDevolucao;
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não é possível iniciar o PDV porque o parâmetro de sistema '" + parametroAtual + "' não foi informado corretamente ou seu valor não pôde ser reconhecido. \nAcione o suporte Doware.", "Erro de configuração", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void txProduto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F3)
                ShowPesquisaProduto();

            if (e.Key == Key.Enter)
                VendeItem();
        }

        private void VendeItem()
        {
            if (!VendaAberta)
            {
                VendaAberta = true;
                PainelVenda = new PainelItensVenda();
                PainelVenda.AbreVenda(Tipo_movimento_id_venda.Id);
                GridContainer.Children.Clear();
                GridContainer.Children.Add(PainelVenda);
            }

            Estoque estoque = new ProdutosController().Get(txProduto.Text);

            if (estoque == null)
                return;
            if (string.IsNullOrEmpty(txQuant.Text))
                txQuant.Text = "1";

            decimal quant = decimal.Parse(txQuant.Text);

            Itens_movimento itemMov = new Itens_movimento();
            itemMov.Produtos = estoque.Produtos;
            itemMov.Produto_id = itemMov.Produtos.Id;
            itemMov.Valor_unit = estoque.Produtos.Valor_unit;
            itemMov.Unidades = estoque.Produtos.Unidades;
            itemMov.Unidade_id = estoque.Produtos.Unidade_id;
            itemMov.Quant = quant;
            itemMov.Valor_final = (estoque.Produtos.Valor_unit * quant);
            itemMov.Cfop = (Operacao_atual == Tipo_operacao_atual.VENDA
                ? Tipo_movimento_id_venda.Cfop
                : Tipo_movimento_id_devolucao.Cfop);

            PainelVenda.VendeItem(itemMov);
        }

        private void ShowPesquisaProduto()
        {
            PesquisarProduto pp = new PesquisarProduto();
            pp.ShowDialog();

            if (pp.Selecionado.Id > 0)
                txProduto.Text = pp.Selecionado.Id.ToString();
        }

        public enum Tipo_operacao_atual
        {
            VENDA = 0,
            DEVOLUCAO = 1
        }

        private void btPagamento_Click(object sender, RoutedEventArgs e)
        {
            PagamentosPDV pagamentos = new PagamentosPDV(Atalhos_pagamentos, PainelVenda.GetValorParcial());
            pagamentos.ShowDialog();

            if(pagamentos.Pago)
            {
                pagamentos.Itens_pagamento.ForEach(i => PainelVenda.EfetuaPagamento(i.Forma_pagamento_id, i.Valor));
                PainelVenda.Encerrar();
            }
        }

        private void btNFCe_Click(object sender, RoutedEventArgs e)
        {
            PainelVenda.NFCe();
        }
    }
}
