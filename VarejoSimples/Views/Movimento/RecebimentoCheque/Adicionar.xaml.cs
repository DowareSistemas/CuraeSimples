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

namespace VarejoSimples.Views.Movimento.RecebimentoCheque
{
    /// <summary>
    /// Lógica interna para Adicionar.xaml
    /// </summary>
    public partial class Adicionar : Window
    {
        public List<Cheque> Cheques = new List<Cheque>();

        public Adicionar()
        {
            InitializeComponent();

            txNumero_cheque.ToNumeric();
            txAgencia.ToNumeric(true);
            txDias_compens.ToNumeric();
            txValor.ToMoney();
            txNumero_cheque.Focus();
        }

        private void Salvar(bool close)
        {
            if (string.IsNullOrWhiteSpace(txDias_compens.Text))
                txDias_compens.Text = "0";

            if(string.IsNullOrWhiteSpace(txNumero_cheque.Text))
            {
                MessageBox.Show("Informe o número do cheque", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if(txData_deposito.SelectedDate == null)
            {
                MessageBox.Show("Informe a data do depósito", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if(int.Parse(txNumero_cheque.Text) == 0)
            {
                MessageBox.Show("Informe o número do cheque", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if(string.IsNullOrWhiteSpace(txBanco.Text))
            {
                MessageBox.Show("Informe o nome do banco", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if(string.IsNullOrWhiteSpace(txAgencia.Text))
            {
                MessageBox.Show("Informe a agência", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if(string.IsNullOrWhiteSpace(txValor.Text))
            {
                MessageBox.Show("Informe o valor do cheque", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if(decimal.Parse(txValor.Text) == 0)
            {
                MessageBox.Show("Informe o valor do cheque", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            Cheques.Add(new Cheque()
            {
                Numero_cheque = txNumero_cheque.Text,
                Banco = txBanco.Text,
                Agencia = txAgencia.Text,
                Conta = txConta.Text,
                Data_deposito = (DateTime)txData_deposito.SelectedDate,
                Dias_compensacao = int.Parse(txDias_compens.Text),
                Valor = decimal.Parse(txValor.Text)
            });

            if (close)
                Fechar();
            else
                LimparCampos();
        }

        private void LimparCampos()
        {
            txNumero_cheque.Text = "0";
            txDias_compens.Text = "0";
            txValor.Text = "0,00";
            txNumero_cheque.Focus();
        }

        private void Fechar()
        {
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.F1:
                    Salvar(true);
                    break;
                case Key.F2:
                    Salvar(false);
                    break;
                case Key.Escape:
                    Fechar();
                    break;
            }
        }

        public void SetConta(Contas conta)
        {
            txBanco.Text = conta.Nome_banco;
            txAgencia.Text = conta.Agencia;
            txConta.Text = conta.Conta;

            txBanco.IsEnabled = false;
            txAgencia.IsEnabled = false;
            txConta.IsEnabled = false;

            txDias_compens.IsEnabled = false;
        }

        private void btSalvarEContinuar_Click(object sender, RoutedEventArgs e)
        {
            Salvar(false);
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            Salvar(true);
        }

        private void btFechar_Click(object sender, RoutedEventArgs e)
        {
            Fechar();
        }
    }
}
