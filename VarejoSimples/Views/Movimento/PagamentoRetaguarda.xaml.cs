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
using VarejoSimples.Interfaces;
using VarejoSimples.Model;
using VarejoSimples.Views.Forma_pagto;

namespace VarejoSimples.Views.Movimento
{
    /// <summary>
    /// Lógica interna para PagamentoRetaguarda.xaml
    /// </summary>
    public partial class PagamentoRetaguarda : Window, ITelaPagamentoMovimento
    {
        public PagamentoRetaguarda()
        {
            InitializeComponent();

            dataGrid.AplicarPadroes(false);
            Itens_pagamento = new List<Model.Itens_pagamento>();
            Pago = false;

            txCod_fpg.ToNumeric();
            txValor.ToMoney();
            txValorMovimento.ToMoney();
            txValorPago.ToMoney();
            txValorFaltando.ToMoney();
            txTroco.ToMoney();

            dataGrid.ItemsSource = Itens_pagamento;
            dataGrid.Columns[0].IsReadOnly = true;
            dataGrid.CanUserAddRows = false;
        }

        public List<Itens_pagamento> Itens_pagamento { get; set; }

        public bool Pago { get; set; }

        public decimal Troco { get; set; }

        public void Exibir(decimal valor_movimento)
        {
            txValorMovimento.Text = valor_movimento.ToString("N2");
            txValorFaltando.Text = valor_movimento.ToString("N2");

            this.ShowDialog();
        }

        private void btSelecionarForma_pag_Click(object sender, RoutedEventArgs e)
        {
            BuscarFormaPag();
        }

        private void BuscarFormaPag()
        {
            txValor.IsEnabled = false;
            txCod_fpg.Focus();

            PesquisarFormas_pag pg = new PesquisarFormas_pag();
            pg.ShowDialog();

            txCod_fpg.Text = pg.Selecionado.Id.ToString();
            txForma_pag.Text = (pg.Selecionado.Id == 0
                ? "Não selecionado"
                : pg.Selecionado.Descricao);

            if (pg.Selecionado.Id > 0)
            {
                txValor.IsEnabled = true;
                txValor.Focus();
            }

            if (Itens_pagamento.FirstOrDefault(i => i.Forma_pagamento_id == pg.Selecionado.Id) != null)
            {
                txForma_pag.Text = "Condição já informada.";
                txValor.IsEnabled = false;
            }
        }

        private void txValor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.Parse(txCod_fpg.Text) == 0)
                    return;

                Itens_pagamento.Add(new Model.Itens_pagamento()
                {
                    Forma_pagamento_id = int.Parse(txCod_fpg.Text),
                    Formas_pagamento = new Formas_pagamentoController().Find(int.Parse(txCod_fpg.Text)),
                    Valor = decimal.Parse(txValor.Text)
                });

                txCod_fpg.Text = "0";
                txForma_pag.Text = string.Empty;
                txValor.Text = "0,00";
                dataGrid.Items.Refresh();
                RecalculaTotais();
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F3)
                BuscarFormaPag();

            if (e.Key == Key.Escape)
                Close();

            if (e.Key == Key.F5)
                Confirmar();
        }

        private void Confirmar()
        {
            if (decimal.Parse(txValorFaltando.Text) > 0)
                return;

            Pago = true;
            Close();
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }

        private void dataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (this.dataGrid.SelectedItem != null)
            {
                (sender as DataGrid).RowEditEnding -= dataGrid_RowEditEnding;
                (sender as DataGrid).CommitEdit();
                (sender as DataGrid).Items.Refresh();
                (sender as DataGrid).RowEditEnding += dataGrid_RowEditEnding;

                RecalculaTotais();
            }
            else return;
        }

        private void RecalculaTotais()
        {
            txValorPago.Text = "0,00";
            txTroco.Text = "0,00";
            txValorFaltando.Text = "0,00";

            decimal valor_movimento = decimal.Parse(txValorMovimento.Text);
            decimal valor_pago = Itens_pagamento.Sum(e => e.Valor);
            decimal falta_pagar = (valor_movimento - valor_pago);

            if (valor_pago > valor_movimento)
                txTroco.Text = (falta_pagar * (-1)).ToString("N2");

            txValorPago.Text = valor_pago.ToString("N2");

            if (falta_pagar > 0)
                txValorFaltando.Text = falta_pagar.ToString("N2");
            else
                txValorFaltando.Text = "0,00";
        }

        private void txValorFaltando_TextChanged(object sender, TextChangedEventArgs e)
        {
            btConfirmar.IsEnabled = (decimal.Parse(txValorFaltando.Text) == 0);
        }
    }
}
