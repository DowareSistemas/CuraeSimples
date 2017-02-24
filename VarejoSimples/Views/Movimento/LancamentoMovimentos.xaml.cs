using System;
using System.Collections.Generic;
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
using VarejoSimples.Enums;
using VarejoSimples.Interfaces;
using VarejoSimples.Model;
using VarejoSimples.Views.Cliente;
using VarejoSimples.Views.Fornecedor;
using VarejoSimples.Views.Produto;
using VarejoSimples.Views.Tipo_movimento;
using VarejoSimples.Views.Vendedor;

namespace VarejoSimples.Views.Movimento
{
    /// <summary>
    /// Lógica interna para LancamentoMovimentos.xaml
    /// </summary>
    public partial class LancamentoMovimentos : Window
    {
        private MovimentosController Movimento_Controller { get; set; }
        private Tipos_movimento Tipo_movimento { get; set; }
        private decimal Desconto { get; set; }
        private decimal Acrescimo { get; set; }

        public LancamentoMovimentos()
        {
            InitializeComponent();

            dataGrid.AplicarPadroes();
            BStatus.Attach(2, lbStatus, image);
            Grid_Mov.IsEnabled = false;

            Desconto = 0;
            Acrescimo = 0;
            txValor_final.ToMoney();
            txTotal_prod.ToMoney();
            txValor_unit.ToMoney();
            txDesconto.ToMoney();
            txAcrescimo.ToMoney();
            txTotal.ToMoney();
            lbDescricao_produto.Content = string.Empty;

            lbFatorConversao.Visibility = Visibility.Hidden;

        }

        private void btSelecionarTmv_Click(object sender, RoutedEventArgs e)
        {
            SelecionarTipoMov();
        }

        public bool FechaVenda()
        {
            try
            {


                return true;
            }
            catch (Exception ex)
            {

            }

            return false;
        }

        private void SelecionarTipoMov()
        {
            PesquisarTmv pt = new PesquisarTmv();
            pt.ShowDialog();

            if (pt.Selecionado.Id == 0)
            {
                MessageBox.Show("O tipo de movimento deve ser selecionado antes de iniciar a operação!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                SelecionarTipoMov();
            }
            else
            {
                txCod_tipo.Text = pt.Selecionado.Id.ToString();
                txNome_tipo.Text = pt.Selecionado.Descricao;

                if (pt.Selecionado.Cfop == 0)
                    BStatus.Alert("O tipo de movimento selecionado não possui CFOP. Não será permitido emitir NFC-e.");
                else
                    BStatus.Success("Tipo de movimento OK");

                btNFCe.IsEnabled = (pt.Selecionado.Cfop != 0);
                txCfop.Text = pt.Selecionado.Cfop.ToString();

                lbCliente_fornecedor.Content = (pt.Selecionado.Utiliza_fornecedor
                     ? "Fornecedor"
                     : "Cliente");

                Tipo_movimento = pt.Selecionado;
            }
        }

        private void SelecionarCliente_Fornecedor()
        {
            //FORNECEDOR
            if (Tipo_movimento.Utiliza_fornecedor)
            {
                PesquisarFornecedor pf = new PesquisarFornecedor();
                pf.ShowDialog();

                if (pf.Selecionado.Id == 0)
                {
                    MessageBox.Show("O tipo de movimento informado utiliza fornecedor. \nNeste caso, informar o fornecedor é obrigatório. \nSelecione o fornecedor!", "Fornecedor obrigatório", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    SelecionarCliente_Fornecedor();
                }
                else
                {
                    txCod_cliente_fornecedor.Text = pf.Selecionado.Id.ToString();
                    txNome_cliente_fornecedor.Text = (pf.Selecionado.Id == 0
                        ? "Não selecionado"
                        : pf.Selecionado.Nome);
                }
            }

            //CLIENTE
            if (!Tipo_movimento.Utiliza_fornecedor)
            {
                PesquisarCliente pc = new PesquisarCliente();
                pc.ShowDialog();

                txCod_cliente_fornecedor.Text = pc.Selecionado.Id.ToString();
                txNome_cliente_fornecedor.Text = (pc.Selecionado.Id == 0
                    ? "Não selecionado"
                    : pc.Selecionado.Nome);
            }
        }

        private void SelecionarVendedor()
        {
            PesquisarVendedor pv = new PesquisarVendedor();
            pv.ShowDialog();

            txCod_vendedor.Text = pv.Selecionado.Id.ToString();
            txNome_vendedor.Text = (pv.Selecionado.Id == 0
                ? "Não selecionado"
                : pv.Selecionado.Nome);
        }

        private void btNovo_Click(object sender, RoutedEventArgs e)
        {
            AbreMovimento();
        }

        private void AbreMovimento()
        {
            BStatus.Success("Abrindo movimento...");

            Movimento_Controller = new MovimentosController();
            SelecionarTipoMov();
            SelecionarCliente_Fornecedor();

            if (!Tipo_movimento.Utiliza_fornecedor)
                SelecionarVendedor();

            Movimento_Controller.AbreMovimento(int.Parse(txCod_cliente_fornecedor.Text), int.Parse(txCod_tipo.Text));
            Grid_Mov.IsEnabled = true;

            txProduto.Focus();
            dataGrid.ItemsSource = Movimento_Controller.Itens_movimento;
            BStatus.Success("Movimento iniciado!");
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                AbreMovimento();
        }

        private void txProduto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                BuscarProduto(false);

            if (e.Key == Key.F3)
                BuscarProduto(true);
        }

        private void BuscarProduto(bool abrirPesquisa)
        {
            Produtos produto;
            if (abrirPesquisa)
            {
                PesquisarProduto pp = new PesquisarProduto();
                pp.ShowDialog();

                produto = pp.Selecionado;
                txProduto.Text = produto.Id.ToString();
            }
            else
                produto = new ProdutosController().Get(txProduto.Text).Produtos;

            if (produto == null)
            {
                BStatus.Alert("Produto não encontrado.");
                txProduto.Text = string.Empty;
                return;
            }
            else
                BStatus.Success("Produto encontrado!");

            if (Tipo_movimento.Utiliza_fornecedor)
            {
                if (produto.Produtos_fornecedores.Where(pf => pf.Fornecedor_id == int.Parse(txCod_cliente_fornecedor.Text)).Count() == 0)
                {
                    BStatus.Alert("O produto selecionado não pode ser adicionado a este movimento, pois ele não está relacionado a este fornecedor.");
                    return;
                }

                if (Tipo_movimento.Utiliza_fornecedor && Tipo_movimento.Movimentacao_itens == (int)Tipo_movimentacao.ENTRADA)
                    MostraFatorConv(produto.Produtos_fornecedores.First(pf => pf.Fornecedor_id == int.Parse(txCod_cliente_fornecedor.Text)));
                else
                    lbFatorConversao.Visibility = Visibility.Hidden;
            }

            lbDescricao_produto.Content = produto.Descricao;
            txValor_unit.Text = (Tipo_movimento.Utiliza_fornecedor
                 ? produto.Produtos_fornecedores.First(pf => pf.Fornecedor_id == int.Parse(txCod_cliente_fornecedor.Text)).Preco_custo.ToString("N2")
                 : produto.Valor_unit.ToString("N2"));
            txQuant.Text = "1";
            txValor_final.Text = txValor_unit.Text;
            txQuant.Focus();
            txQuant.SelectAll();
        }

        private void MostraFatorConv(Produtos_fornecedores pf)
        {
            lbFatorConversao.Visibility = Visibility.Visible;
            lbFatorConversao.Content = $"1 {pf.Unidades.Sigla} = {pf.Fator_conversao} {pf.Produtos.Unidades.Sigla}";
        }

        private void txQuant_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CalculaTotalQuant();
            }
        }

        private void txQuant_LostFocus(object sender, RoutedEventArgs e)
        {
            CalculaTotalQuant();
        }

        private void CalculaTotalQuant()
        {
            if (string.IsNullOrWhiteSpace(txProduto.Text))
                return;

            if (txProduto.Text == "0")
                return;

            Produtos prod = new ProdutosController().Get(txProduto.Text).Produtos;
            if (prod == null)
                return;

            decimal valorUnitProd = decimal.Parse(txValor_unit.Text);
            decimal quant = decimal.Parse(txQuant.Text);
            decimal valorTotal = (valorUnitProd * quant);
            txTotal_prod.Text = valorTotal.ToString("N2");
            txValor_final.Text = txTotal_prod.Text;
            txDesconto.Focus();
            txDesconto.SelectAll();
        }

        private void txDesconto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                CalculaTotalDesc_Item();
            if (e.Key == Key.D8 || e.Key == Key.Multiply)
            {
                if (lbDesconto.Content.ToString().Contains("R$"))
                {
                    lbDesconto.Content = "Desconto (%)";
                    lbAltera_desconto.Content = "* - Alterar para R$";
                }
                else
                {
                    lbDesconto.Content = "Desconto (R$)";
                    lbAltera_desconto.Content = "* - Alterar para %";
                }
            }
        }

        private void txDesconto_LostFocus(object sender, RoutedEventArgs e)
        {
            CalculaTotalDesc_Item();
        }

        private void CalculaTotalDesc_Item()
        {
            if (txProduto.Text == "0")
                return;
            if (Desconto > 0) //desconto já foi aplicado para este item
                return;

            decimal valor = decimal.Parse(txTotal_prod.Text);
            decimal desconto = (lbDesconto.Content.ToString().Contains("R$")
                ? decimal.Parse(txDesconto.Text)
                : valor / 100 * decimal.Parse(txDesconto.Text));

            this.Desconto = desconto;

            valor = (valor - desconto);
            txValor_final.Text = valor.ToString("N2");

            if (desconto > 0)
            {
                txValor_final.Focus();
                txValor_final.SelectAll();
            }
            else
            {
                txAcrescimo.Focus();
                txAcrescimo.SelectAll();
            }

            txAcrescimo.IsEnabled = (desconto == 0);
        }

        private void CalculaTotalAcresc_Item()
        {
            if (txProduto.Text == "0")
                return;
            if (Acrescimo > 0) //acrescimo já foi aplicado para este item
                return;

            decimal valor = decimal.Parse(txTotal_prod.Text);
            decimal acrescimo = (lbAcrescimo.Content.ToString().Contains("R$")
                ? decimal.Parse(txAcrescimo.Text)
                : valor / 100 * decimal.Parse(txAcrescimo.Text));

            this.Acrescimo = acrescimo;

            valor = (valor + acrescimo);
            txValor_final.Text = valor.ToString("N2");
            txValor_final.Focus();
            txValor_final.SelectAll();

            txDesconto.IsEnabled = (acrescimo == 0);
        }

        private void AdicionaItem()
        {
            Itens_movimento item = new Itens_movimento();
            Estoque est = new ProdutosController().Get(txProduto.Text);
            Produtos prod = est.Produtos;

            if (Tipo_movimento.Movimentacao_itens == (int)Tipo_movimentacao.SAIDA)
            {
                if (est.Quant <= 0)
                    if (ParametrosController.FindParametro("EST_SAIZERO").Valor.Equals("N"))
                    {
                        MessageBox.Show($"Não é possível retirar o produto '{est.Produtos.Descricao}' do estoque porque o sistema está atualmente configurado para não permitir retiradas de estoque cujo o saldo atual é igual ou inferior a 0.", "EST_SAIZERO", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
            }

            if (prod == null)
                return;

            item.Produto_id = prod.Id;
            item.Produtos = prod;
            item.Lote = est.Lote;
            item.Sublote = est.Sublote;
            item.Aliquota = prod.Aliquota;
            item.Valor_unit = (Tipo_movimento.Utiliza_fornecedor
                 ? prod.Produtos_fornecedores.First(pf => pf.Fornecedor_id == int.Parse(txCod_cliente_fornecedor.Text)).Preco_custo
                 : prod.Valor_unit);
            item.Desconto = this.Desconto;
            item.Acrescimo = this.Acrescimo;
            item.Quant = decimal.Parse(txQuant.Text);
            item.Valor_final = decimal.Parse(txValor_final.Text);
            item.Cfop = Tipo_movimento.Cfop;
            item.Vendedor_id = int.Parse(txCod_vendedor.Text);

            if (Tipo_movimento.Utiliza_fornecedor && Tipo_movimento.Movimentacao_itens == (int)Tipo_movimentacao.ENTRADA)
                item.Unidades = prod.Produtos_fornecedores.First(pf => pf.Fornecedor_id == int.Parse(txCod_cliente_fornecedor.Text)).Unidades;
            else
                item.Unidades = prod.Unidades;

            item.Unidade_id = item.Unidades.Id;

            Movimento_Controller.AdicionaItem(item);
            dataGrid.ItemsSource = Movimento_Controller.Itens_movimento;

            txProduto.Text = "0";
            txValor_unit.Text = "0,00";
            txQuant.Text = "0,00";
            txTotal_prod.Text = "0,00";
            txDesconto.Text = "0,00";
            txAcrescimo.Text = "0,00";
            txValor_final.Text = "0,00";
            lbDescricao_produto.Content = string.Empty;
            lbFatorConversao.Visibility = Visibility.Hidden;
            txProduto.Focus();
            txProduto.Text = string.Empty;
            Desconto = 0;
            Acrescimo = 0;
            txDesconto.IsEnabled = true;
            txAcrescimo.IsEnabled = true;

            RecalculaTotais();
        }

        private void btSelecionarProduto_Click(object sender, RoutedEventArgs e)
        {
            BuscarProduto(true);
        }

        private void txValor_final_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                AdicionaItem();
        }

        private void btInserir_Click(object sender, RoutedEventArgs e)
        {
            AdicionaItem();
        }

        private void txDesconto_GotFocus(object sender, RoutedEventArgs e)
        {
            Desconto = 0;
            Acrescimo = 0;
            txDesconto.Text = "0,00";
            txAcrescimo.Text = "0,00";
        }

        private void txAcrescimo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                CalculaTotalAcresc_Item();
            if (e.Key == Key.D8 || e.Key == Key.Multiply)
            {
                if (lbAcrescimo.Content.ToString().Contains("R$"))
                {
                    lbAcrescimo.Content = "Acréscimo (%)";
                    lbAltera_acrescimo.Content = "* - Alterar para R$";
                }
                else
                {
                    lbAcrescimo.Content = "Acréscimo (R$)";
                    lbAltera_acrescimo.Content = "* - Alterar para %";
                }
            }
        }

        private void txAcrescimo_GotFocus(object sender, RoutedEventArgs e)
        {
            Acrescimo = 0;
        }

        private void txAcrescimo_LostFocus(object sender, RoutedEventArgs e)
        {
            CalculaTotalAcresc_Item();
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            ITelaPagamentoMovimento iPagamento = new PagamentoRetaguarda();
            iPagamento.Exibir(decimal.Parse(txTotal.Text));

            string msg = $@"Confirma fechamento do movimento?";

            if(iPagamento.Pago)
            {
                MessageBoxResult result = MessageBox.Show(msg, "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                    return;

                iPagamento.Itens_pagamento.ForEach(ip => Movimento_Controller.EfetuaPagamento(ip.Forma_pagamento_id, ip.Valor));
                Movimento_Controller.FechaMovimento();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            BStatus.Dettach(2);
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (dataGrid.Items.Count == 0)
                return;

            if (e.Key == Key.F3)
                return;

            Itens_movimento item = (Itens_movimento)dataGrid.SelectedItem;

            if (item == null)
                return;

            if (e.Key == Key.Insert)
                Movimento_Controller.IncrementaItem(item.Id);

            if ((e.Key == Key.Delete) && Keyboard.Modifiers == ModifierKeys.Control)
                Movimento_Controller.RemoveItem(item.Id);
            else if (e.Key == Key.Delete)
            {
                if (Movimento_Controller.Itens_movimento.First(i => i.Id == item.Id).Quant - 1 == 0)
                    Movimento_Controller.RemoveItem(item.Id);
                else
                    Movimento_Controller.DecrementaItem(item.Id);
            }

            dataGrid.ItemsSource = Movimento_Controller.Itens_movimento;
            dataGrid.Focus();
            RecalculaTotais();
        }

        private void Grid_Mov_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F3)
                BuscarProduto(true);
        }

        private void RecalculaTotais()
        {
            decimal valor_total = Movimento_Controller.Itens_movimento.Sum(i => i.Valor_final);
            txTotal.Text = valor_total.ToString("N2");
        }
    }
}
