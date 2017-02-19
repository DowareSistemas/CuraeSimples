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

        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btNovo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FillPf(Produtos_fornecedores pf)
        {

        }

        private void Salvar()
        {

        }

        private void LimparCampos()
        {

        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btSelecionarProduto_Click(object sender, RoutedEventArgs e)
        {
            PesquisarProduto pp = new PesquisarProduto();
            pp.ShowDialog();

            txCod_prod.Text = pp.Selecionado.Id.ToString();
            txProduto.Text = (pp.Selecionado.Id == 0
                ? "Não selecionado"
                : pp.Selecionado.Descricao);
        }

        private void btSelecionarForn_Click(object sender, RoutedEventArgs e)
        {
            PesquisarFornecedor pf = new PesquisarFornecedor();
            pf.ShowDialog();

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
            txCod_movEntrada.Text = pt.Selecionado.Id.ToString();
            txMovimentoEntrada.Text = (pt.Selecionado.Id == 0
                ? "Não selecionado"
                : pt.Selecionado.Descricao);
        }

        private void btSelecionarMovDevolucao_Click(object sender, RoutedEventArgs e)
        {
            PesquisarTmv pt = new PesquisarTmv();
            txCod_movDevol.Text = pt.Selecionado.Id.ToString();
            txMovimentoDevol.Text = (pt.Selecionado.Id == 0
                ? "Não selecionado"
                : pt.Selecionado.Descricao);
        }
    }
}
