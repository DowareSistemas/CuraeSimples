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
using VarejoSimples.Model;

namespace VarejoSimples.Views.Conta
{
    /// <summary>
    /// Lógica interna para CadContas.xaml
    /// </summary>
    public partial class CadContas : Window
    {
        private ContasController controller;
        public CadContas()
        {
            InitializeComponent();

            controller = new ContasController();

            List<KeyValuePair<Tipo_conta, string>> tipos = new List<KeyValuePair<Tipo_conta, string>>();
            tipos.Add(new KeyValuePair<Tipo_conta, string>(Tipo_conta.CONTA_CAIXA, "CONTA CAIXA"));
            tipos.Add(new KeyValuePair<Tipo_conta, string>(Tipo_conta.CONTA_BANCARIA, "CONTA BANCÁRIA"));

            cbTipo.ItemsSource = tipos;
            cbTipo.DisplayMemberPath = "Value";
            cbTipo.SelectedValuePath = "Key";
            cbTipo.SelectedIndex = 0;

            txCod.ToNumeric();
            txNumero_banco.ToNumeric();
            txCod_conta.ToNumeric(true);
            txAgencia.ToNumeric();
            txConvenio.ToNumeric();
            txCarteira.ToNumeric();
            txNosso_numero.ToNumeric();
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCod.Text);
            Contas conta = controller.Next(id);
            FillConta(conta);
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCod.Text);
            if((id - 1) <= 0)
            {
                LimparCampos();
                return;
            }

            Contas conta = controller.Prev(id);
            FillConta(conta);
        }

        private void btNovo_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            Salvar();
        }

        private void Salvar()
        {
            int id = int.Parse(txCod.Text);

            Contas conta = (id == 0
                ? new Contas()
                : controller.Find(id));

            conta.Nome = txNome.Text;
            conta.Tipo = ((int)(Tipo_conta)cbTipo.SelectedValue);
            conta.Inativa = ckInativo.IsChecked.Value;
            conta.Banco_numero = int.Parse(txNumero_banco.Text);
            conta.Nome_banco = txNome_banco.Text;
            conta.Conta = txCod_conta.Text;
            conta.Convenio = txConvenio.Text;
            conta.Carteira = txCarteira.Text;
            conta.Agencia = txAgencia.Text;
            conta.Titular = txTitular.Text;
            conta.Nosso_numero = txNosso_numero.Text;

            if (controller.Save(conta))
                LimparCampos();
        }

        private void LimparCampos()
        {
            txCod.Text = "0";
            txNome.Text = string.Empty;
            ckInativo.IsChecked = false;
            cbTipo.SelectedIndex = 0;
            txCod_conta.Text = "0";
            txNome_banco.Text = string.Empty;
            txTitular.Text = string.Empty;
            txAgencia.Text = "0";
            txConvenio.Text = "0";
            txCarteira.Text = "0";
            txNosso_numero.Text = "0";
            txNumero_banco.Text = "0";
            txNome.Focus();
        }

        private void FillConta(Contas conta)
        {
            if (conta == null)
                return;

            txCod.Text = conta.Id.ToString();
            txNome.Text = conta.Nome;
            cbTipo.SelectedValue = ((Tipo_conta)conta.Tipo);
            ckInativo.IsChecked = conta.Inativa;
            txNumero_banco.Text = conta.Banco_numero.ToString();
            txNome_banco.Text = conta.Nome_banco;
            txCod_conta.Text = conta.Conta;
            txTitular.Text = conta.Titular;
            txAgencia.Text = conta.Agencia;
            txConvenio.Text = conta.Convenio;
            txCarteira.Text = conta.Carteira;
            txNosso_numero.Text = conta.Nosso_numero;
            groupBox.IsEnabled = (conta.Tipo == (int)Tipo_conta.CONTA_BANCARIA);
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCod.Text);
            if (id == 0)
                return;

            if (controller.Remove(id))
                LimparCampos();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void cbTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTipo == null)
                return;

            Tipo_conta tipo = (Tipo_conta)cbTipo.SelectedValue;
            groupBox.IsEnabled = (tipo == Tipo_conta.CONTA_BANCARIA);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F3)
            {
                PesquisarConta pc = new PesquisarConta(true);
                pc.ShowDialog();

                if (pc.Selecionado.Id == 0)
                    return;

                FillConta(pc.Selecionado);
            }
        }
    }
}
