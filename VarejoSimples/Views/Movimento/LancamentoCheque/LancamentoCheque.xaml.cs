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
using VarejoSimples.Views.Movimento.RecebimentoCheque;

namespace VarejoSimples.Views.Movimento.LancamentoCheque
{
    /// <summary>
    /// Lógica interna para LancamentoCheque.xaml
    /// </summary>
    public partial class LancamentoCheque : Window, IRegistroCheques
    {
        private Contas Conta { get; set; }
        private decimal Valor_pagamento { get; set; }
        public LancamentoCheque()
        {
            InitializeComponent();

            Cheques = new List<Cheque>();
            dataGrid.AplicarPadroes();
            txValor_faltando.ToMoney();
            txValor_pago.ToMoney();
        }

        public List<Cheque> Cheques { get; set; }

        public void Exibir(decimal valor_pagamento)
        {
            Valor_pagamento = valor_pagamento;
            txValor_faltando.Text = valor_pagamento.ToString("N2");

            dataGrid.ItemsSource = Cheques;
            this.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (decimal.Parse(txValor_faltando.Text) > 0)
                e.Cancel = true;
        }

        private void btInserir_Click(object sender, RoutedEventArgs e)
        {
            InserirCheque();
        }

        private void InserirCheque()
        {
            Adicionar ad = new Adicionar();
            ad.SetConta(Conta);
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

        private void btRemover_Click(object sender, RoutedEventArgs e)
        {
            RemoverCheque();
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txValor_faltando_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (btConfirmar != null)
                btConfirmar.IsEnabled = (decimal.Parse(txValor_faltando.Text) == 0);
        }

        public void SetConta(Contas conta)
        {
            Conta = conta;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Insert:
                    InserirCheque();
                    break;

                case Key.Delete:
                    RemoverCheque();
                    break;
            }
        }
    }
}
