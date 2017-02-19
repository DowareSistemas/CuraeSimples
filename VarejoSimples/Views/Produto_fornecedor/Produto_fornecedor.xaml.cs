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
using VarejoSimples.Model;
using VarejoSimples.Views.Fornecedor;
using VarejoSimples.Views.Produto;
using VarejoSimples.Views.Tipo_movimento;
using VarejoSimples.Views.Unidade;

namespace VarejoSimples.Views.Produto_fornecedor
{
    /// <summary>
    /// Lógica interna para Produto_fornecedor.xaml
    /// </summary>
    public partial class Produto_fornecedor : Window
    {
        Produtos_fornecedoresController controller;
        public Produto_fornecedor()
        {
            controller = new Produtos_fornecedoresController();

            InitializeComponent();

            txCod.ToNumeric();
            txCod_forn.ToNumeric();
            txCod_prod.ToNumeric();
            txCod_un.ToNumeric();
            txComissao.ToMoney();
            txCusto.ToMoney();
            txCod_movDevol.ToNumeric();
            txCod_movEntrada.ToNumeric();
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            Produtos_fornecedores pf = controller.Next(int.Parse(txCod.Text));
            FillPf(pf);
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCod.Text);
            if((id - 1) <= 0)
            {
                LimparCampos();
                return;
            }

            Produtos_fornecedores pf = controller.Prev(id);
            FillPf(pf);
        }

        private void btNovo_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            Salvar();
        }

        private void FillPf(Produtos_fornecedores pf)
        {
            if (pf == null)
                return;

            txCod.Text = pf.Id.ToString();
            
            //Produto
            txCod_prod.Text = pf.Produtos.Id.ToString();
            txProduto.Text = pf.Produtos.Descricao;

            //Fornecedor
            txCod_forn.Text = pf.Fornecedores.Id.ToString();
            txFornecedor.Text = pf.Fornecedores.Nome;

            //Unidade
            txCod_un.Text = pf.Unidades.Id.ToString();
            txUnidade.Text = pf.Unidades.Nome;

            txFator_conv.Text = pf.Fator_conversao.ToString();
            txCusto.Text = pf.Preco_custo.ToString("N2");

            ckConsignado.IsChecked = pf.Consignado;
            if(pf.Consignado)
            {
                txComissao.Text = pf.Comissao.ToString("N2");

                //Mov entrada
                txCod_movEntrada.Text = pf.Movimento_entrada.ToString();
                txMovimentoEntrada.Text = new Tipos_movimentoController().Find(pf.Movimento_entrada).Descricao;

                //Mov saida
                txCod_movDevol.Text = pf.Movimento_devolucao.ToString();
                txMovimentoDevol.Text = new Tipos_movimentoController().Find(pf.Movimento_devolucao).Descricao;
            }
        }

        private void Salvar()
        {
            Produtos_fornecedores pf = (int.Parse(txCod.Text) == 0
                ? new Produtos_fornecedores()
                : controller.Find(int.Parse(txCod.Text)));

            pf.Id = int.Parse(txCod.Text);
            pf.Produto_id = int.Parse(txCod_prod.Text);
            pf.Fornecedor_id = int.Parse(txCod_forn.Text);
            pf.Unidade_id = int.Parse(txCod_un.Text);
            pf.Fator_conversao = int.Parse(txFator_conv.Text);
            pf.Preco_custo = decimal.Parse(txCusto.Text);
            pf.Consignado = ckConsignado.IsChecked.Value;
            pf.Comissao = decimal.Parse(txComissao.Text);
            pf.Movimento_entrada = int.Parse(txCod_movEntrada.Text);
            pf.Movimento_devolucao = int.Parse(txCod_movDevol.Text);

            if (controller.Save(pf))
                LimparCampos();
        }

        private void LimparCampos()
        {
            txCod.Text = "0";
            txCod_prod.Text = "0";
            txProduto.Text = string.Empty;
            txCod_forn.Text = "0";
            txFornecedor.Text = string.Empty;
            txCod_un.Text = "0";
            txUnidade.Text = string.Empty;
            txFator_conv.Text = "1";
            txCusto.Text = "0,00";
            ckConsignado.IsChecked = false;
            GridConsignacao.IsEnabled = false;
            txComissao.Text = "0,00";
            txCod_movEntrada.Text = "0";
            txMovimentoEntrada.Text = string.Empty;
            txCod_movDevol.Text = "0";
            txMovimentoDevol.Text = string.Empty;
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCod.Text);
            if (controller.Remove(id))
                LimparCampos();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btSelecionarProduto_Click(object sender, RoutedEventArgs e)
        {
            PesquisarProduto pp = new PesquisarProduto();
            pp.ShowDialog();

            if (pp.Selecionado.Id == 0)
                return;

            txCod_prod.Text = pp.Selecionado.Id.ToString();
            txProduto.Text = (pp.Selecionado.Id == 0
                ? "Não selecionado"
                : pp.Selecionado.Descricao);
        }

        private void btSelecionarForn_Click(object sender, RoutedEventArgs e)
        {
            PesquisarFornecedor pf = new PesquisarFornecedor();
            pf.ShowDialog();

            if (pf.Selecionado.Id == 0)
                return;

            txCod_forn.Text = pf.Selecionado.Id.ToString();
            txFornecedor.Text = (pf.Selecionado.Id == 0
                ? "Não selecionado"
                : pf.Selecionado.Nome);
        }

        private void btSelecionarUnidade_Click(object sender, RoutedEventArgs e)
        {
            PesquisarUnidade pu = new PesquisarUnidade();
            pu.ShowDialog();

            txCod_un.Text = pu.Selecionado.Id.ToString();
            txUnidade.Text = (pu.Selecionado.Id == 0
                ? "Não selecionado"
                : pu.Selecionado.Nome);
        }

        private void btSelecionarMovEntrada_Click(object sender, RoutedEventArgs e)
        {
            PesquisarTmv pt = new PesquisarTmv();
            pt.ShowDialog();

            txCod_movEntrada.Text = pt.Selecionado.Id.ToString();
            txMovimentoEntrada.Text = (pt.Selecionado.Id == 0
                ? "Não selecionado"
                : pt.Selecionado.Descricao);
        }

        private void btSelecionarMovDevolucao_Click(object sender, RoutedEventArgs e)
        {
            PesquisarTmv pt = new PesquisarTmv();
            pt.ShowDialog();

            txCod_movDevol.Text = pt.Selecionado.Id.ToString();
            txMovimentoDevol.Text = (pt.Selecionado.Id == 0
                ? "Não selecionado"
                : pt.Selecionado.Descricao);
        }

        private void ckConsignado_Checked(object sender, RoutedEventArgs e)
        {
            GridConsignacao.IsEnabled = true;
        }

        private void ckConsignado_Unchecked(object sender, RoutedEventArgs e)
        {
            GridConsignacao.IsEnabled = false;
        }

        private void btRelatorio_Click(object sender, RoutedEventArgs e)
        {
            PFRN parametros = new PFRN();
            parametros.ShowDialog();
        }
    }
}
