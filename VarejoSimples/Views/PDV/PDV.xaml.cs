﻿using System;
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
using VarejoSimples.Interfaces;
using VarejoSimples.Model;
using VarejoSimples.Views.PDV.EventMonitors;
using VarejoSimples.Views.Produto;
using VarejoSimples.Views.VendaRapida;

namespace VarejoSimples.Views.PDV
{
    /// <summary>
    /// Lógica interna para PDV.xaml
    /// </summary>
    public partial class PDV : Window, IPDV
    {
        private bool VendaAberta { get; set; }
        public IPainelVenda PainelVenda { get; set; }
        private bool Pago { get; set; }
        private Tipos_movimento Tipo_movimento_id_venda { get; set; }
        private Tipos_movimento Tipo_movimento_id_devolucao { get; set; }
        Tipo_operacao_atual Operacao_atual { get; set; }
        Tipo_PDV Tipo_pdv { get; set; }

        List<KeyValuePair<int, Formas_pagamento>> Atalhos_pagamentos { get; set; }

        public PDV()
        {
            InitializeComponent();

            VendaAberta = false;

            Atalhos_pagamentos = new List<KeyValuePair<int, Formas_pagamento>>();
            Operacao_atual = Tipo_operacao_atual.VENDA;
            Setup();
            ResetUI();
            MonitorInsereRemove.Instance.ItemInserido += Instance_ItemInserido;
            MonitorInsereRemove.Instance.ItemRemovido += Instance_ItemRemovido;
            MonitorSelecaoPedido.Instance.PedidoSelecionado += Instance_PedidoSelecionado;
            txQuant.Text = "1,00";
        }

        private void ResetUI()
        {
            VendaAberta = false;
            btPagamento.IsEnabled = false;
            btCliente.IsEnabled = false;
            btSalvarPedido.IsEnabled = false;
            lbCpf.Content = string.Empty;
            lbNomeCliente.Content = "NÃO INFORMADO";
            lbCreditoCliente.Content = "0,00";

            if (Tipo_pdv == Tipo_PDV.PDV)
            {
                GridContainer.Children.Clear();
                GridContainer.Children.Add(new LogoEmpresa());
                HabilitarPaineis(true);
            }

            if (Tipo_pdv == Tipo_PDV.PDP)
            {
                GridContainer.Children.Clear();
                GridContainer.Children.Add(new PainelPedidos());
                HabilitarPaineis(false);
            }
        }

        private void Instance_PedidoSelecionado(Pedidos_venda pedido)
        {
            HabilitarPaineis(true);
            VendaAberta = true;
            PainelVenda = new PainelItensVenda();
            GridContainer.Children.Clear();
            GridContainer.Children.Add(PainelVenda.CurrentUserControl);

            PainelVenda.TransformarEmMovimento(pedido, Tipo_movimento_id_venda.Id);
            btPagamento.IsEnabled = true;
            btSalvarPedido.IsEnabled = false;

            if (PainelVenda.ClienteInformado)
            {
                Clientes cliente = PainelVenda.GetCliente();
                lbNomeCliente.Content = cliente.Nome;
                lbCpf.Content = cliente.Cpf;
            }

            txProduto.Focus();
        }

        private void HabilitarPaineis(bool habilitado)
        {
            painelPesquisaProduto.IsEnabled = habilitado;
            painelInfoCliente.IsEnabled = habilitado;
            painelAcoes.IsEnabled = habilitado;
        }

        private void Setup()
        {
            Movimentos_caixasController mc_controller = new Movimentos_caixasController();
            if (!mc_controller.CaixaAberto(UsuariosController.UsuarioAtual.Id))
            {
                AberturaCaixa ac = new AberturaCaixa();
                ac.ShowDialog();

                if (!ac.CaixaAberto)
                    Close();
            }

            txProduto.Focus();
            Pago = false;
            txQuant.ToMoney();
            txQuant.IsEnabled = false;

            btPagamento.IsEnabled = false;
            btCliente.IsEnabled = false;
            string parametroAtual = "";

            try
            {
                ParametrosController parametros = new ParametrosController();
                Tipos_movimentoController tmvController = new Tipos_movimentoController();
                Formas_pagamentoController fpgController = new Formas_pagamentoController();

                #region Parametro TP_PDV
                Parametros paramTipoPdv = parametros.FindParametroLojaAtual("TP_PDV", true);
                if (paramTipoPdv == null)
                    paramTipoPdv = parametros.FindParametroLojaAtual("TP_PDV", false);

                if (paramTipoPdv == null)
                {
                    MessageBox.Show("Não é possível iniciar o PDV por que o parâmetro de sistema 'TP_PDV' não foi atribuido.", "TP_PDV", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Close();
                }

                if (string.IsNullOrEmpty(paramTipoPdv.Valor))
                {
                    MessageBox.Show("Não é possível iniciar o PDV por que o parâmetro de sistema 'TP_PDV' não foi atribuido.", "TP_PDV", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Close();
                }

                if (paramTipoPdv.Valor != "PDV" && paramTipoPdv.Valor != "PDP")
                {
                    MessageBox.Show("Não é possível iniciar o PDV por que o valor do parâmetro de sistema 'TP_PDV' não pôde ser reconhecido.", "TP_PDV", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Close();
                }

                Tipo_pdv = (paramTipoPdv.Valor == "PDV"
                    ? Tipo_PDV.PDV
                    : Tipo_PDV.PDP);
                #endregion

                #region PAGAMENTOS
                List<Parametros> parametrosPagamentos = parametros.ParametrosPagamentoPDV();

                foreach (Parametros param in parametrosPagamentos)
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

        public void CancelarVenda()
        {
            ResetUI();
        }

        public void VendeItem(Estoque estoqueParam = null)
        {
            Estoque estoque = estoqueParam;
            if (estoque == null)
                estoque = new ProdutosController().Get(txProduto.Text);
            if (estoque == null)
                return;
            if (string.IsNullOrEmpty(txQuant.Text))
                txQuant.Text = "1";

            if (estoqueParam == null)
                if (estoque.Produtos.Controla_grade && estoque.Grade_id != txProduto.Text)
                {
                    SelecionarGrade sg = new SelecionarGrade(estoque.Produtos);
                    sg.ShowDialog();

                    if (sg.Selecionado.Id == 0)
                        return;

                    estoque = sg.Selecionado;
                }

            if (!VendaAberta)
            {
                PainelVenda = new PainelItensVenda();
                VendaAberta = true;
                PainelVenda = new PainelItensVenda();
                PainelVenda.AbreVenda(Tipo_movimento_id_venda.Id);
                GridContainer.Children.Clear();
                GridContainer.Children.Add(PainelVenda.CurrentUserControl);
                btPagamento.IsEnabled = true;
                btCliente.IsEnabled = true;
                btSalvarPedido.IsEnabled = true;
            }

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
            itemMov.Lote = estoque.Lote;
            itemMov.Sublote = estoque.Sublote;
            itemMov.Grade_id = estoque.Grade_id;

            PainelVenda.VendeItem(itemMov);
            txProduto.Text = string.Empty;
            txQuant.Text = "1";
            txProduto.Focus();
            txQuant.IsEnabled = false;
            lbQuant.Content = "Quantidade";
        }

        private void ShowPesquisaProduto()
        {
            BuscaProdutoPDV pp = new BuscaProdutoPDV();
            pp.ShowDialog();
        }

        private void Instance_ItemInserido(Estoque estoque)
        {
            VendeItem(estoque);
        }

        private void Instance_ItemRemovido(Estoque estoque)
        {
            if (VendaAberta)
                PainelVenda.DecrementaItem(estoque);
        }

        public enum Tipo_operacao_atual
        {
            VENDA = 0,
            DEVOLUCAO = 1
        }

        public enum Tipo_PDV
        {
            PDV = 0,
            PDP = 1
        }

        private void btPagamento_Click(object sender, RoutedEventArgs e)
        {
            ShowPagamento();
        }

        private void ShowPagamento()
        {
            if (!btPagamento.IsEnabled)
                return;

            PagamentosPDV pagamentos = new PagamentosPDV(Atalhos_pagamentos, PainelVenda.GetValorParcial(), this);
            pagamentos.ShowDialog();

            if (pagamentos.Pago)
            {
                VendaAberta = false;
                ResetUI();
                PainelVenda = null;
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                ShowCancelarItem();

            if (e.Key == Key.F4)
                ShowPesquisaCliente();

            if (e.Key == Key.F6)
                ShowPagamento();

            if (e.Key == Key.F8)
                SalvarPedido();

            if (e.Key == Key.F12)
                ShowMenu();
        }

        private void ShowMenu()
        {
            MenuPDV.Menu menu = new MenuPDV.Menu();
            menu.ShowDialog();

            if (menu.CaixaFechado)
                Close();
        }

        private void txQuant_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                VendeItem();
        }

        private void txProduto_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txProduto.Text.Contains("*"))
            {
                txProduto.Text = string.Empty;
                lbQuant.Content = "Quantidade *";
                txQuant.IsEnabled = true;
                txQuant.SelectAll();
            }
        }

        private void txProduto_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F3)
                ShowPesquisaProduto();

            if (e.Key == Key.Enter)
            {
                if (lbQuant.Content.ToString().Contains("*"))
                {
                    txQuant.Focus();
                    txQuant.SelectAll();
                    return;
                }
                else
                    VendeItem();
            }
        }

        private void btCliente_Click(object sender, RoutedEventArgs e)
        {
            ShowPesquisaCliente();
        }

        private void ShowPesquisaCliente()
        {
            if (!VendaAberta)
                return;
            if (!btCliente.IsEnabled)
                return;

            BuscaClientePdv bc = new BuscaClientePdv();
            bc.ShowDialog();

            if (bc.Selecionado.Id == 0)
                return;

            lbNomeCliente.Content = bc.Selecionado.Nome;
            lbCpf.Content = bc.Selecionado.Cpf;

            PainelVenda.InformaCliente(bc.Selecionado.Id);
        }

        private void btSalvarPedido_Click(object sender, RoutedEventArgs e)
        {
            SalvarPedido();
        }

        private void SalvarPedido()
        {
            if (!VendaAberta)
                return;
            if (!btCliente.IsEnabled)
                return;
            if (!btSalvarPedido.IsEnabled)
                return;

            if (!PainelVenda.ClienteInformado)
            {
                ShowPesquisaCliente();
                if (!PainelVenda.ClienteInformado)
                    return;
            }

            int pedido = PainelVenda.TransformarEmPedido();
            ResetUI();
        }

        private void btMenu_Click(object sender, RoutedEventArgs e)
        {
            ShowMenu();
        }

        private void btCancelarItem_Click(object sender, RoutedEventArgs e)
        {
            ShowCancelarItem();
        }

        private void ShowCancelarItem()
        {
            CancelarItem ci = new CancelarItem(PainelVenda);
            ci.ShowDialog();
        }
    }
}