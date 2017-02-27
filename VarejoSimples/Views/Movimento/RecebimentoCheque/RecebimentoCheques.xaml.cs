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
using VarejoSimples.Views.Movimento.RecebimentoCheque;

namespace VarejoSimples.Views.Movimento.RecebimentoCheques
{
    /// <summary>
    /// Lógica interna para RecebimentoCheques.xaml
    /// </summary>
    public partial class RecebimentoCheques : Window, IRegistroCheques
    {
        private decimal Valor_pagamento { get; set; }
        public RecebimentoCheques()
        {
            InitializeComponent();

            Cheques = new List<Cheque>();
            dataGrid.AplicarPadroes();
            txValor_faltando.ToMoney();
            txValor_pago.ToMoney();
            dataGrid.ItemsSource = Cheques;
        }

        public void Exibir(decimal valor_pagamento)
        {
            Valor_pagamento = valor_pagamento;
            txValor_faltando.Text = valor_pagamento.ToString("N2");
            this.ShowDialog();
        }

        public List<Cheque> Cheques { get; set; }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (decimal.Parse(txValor_faltando.Text) > 0)
                e.Cancel = true;
        }

        private void btInserir_Click(object sender, RoutedEventArgs e)
        {
            AdicionarCheque();
        }

        private void AdicionarCheque()
        {
            Adicionar ad = new Adicionar();
            ad.ShowDialog();

            ad.Cheques.ForEach(c => Cheques.Add(c));
            dataGrid.Items.Refresh();

            RecalculaTotal();
            dataGrid.Focus();
            if (dataGrid.Items.Count > 0)
                dataGrid.SelectedIndex = 0;
        }

        private void RecalculaTotal()
        {
            txValor_pago.Text = Cheques.Sum(c => c.Valor).ToString("N2");
            decimal valor_faltando = (Valor_pagamento - Cheques.Sum(c => c.Valor));
            txValor_faltando.Text = valor_faltando.ToString("N2");
        }

        private void txValor_faltando_TextChanged(object sender, TextChangedEventArgs e)
        {
            btConfirmar.IsEnabled = (decimal.Parse(txValor_faltando.Text) == 0);
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            RemoverCheque();
        }

        private void RemoverCheque()
        {
            Cheque cheque = (Cheque)dataGrid.SelectedItem;
            if (cheque == null)
                return;

            Cheques.Remove(cheque);
            dataGrid.ItemsSource = Cheques;
            dataGrid.Items.Refresh();

            RecalculaTotal();
            dataGrid.Focus();

            if (dataGrid.Items.Count > 0)
                dataGrid.SelectedIndex = 0;
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Insert:
                    AdicionarCheque();
                    break;

                case Key.Delete:
                    RemoverCheque();
                    break;
            }
        }
    }
}
