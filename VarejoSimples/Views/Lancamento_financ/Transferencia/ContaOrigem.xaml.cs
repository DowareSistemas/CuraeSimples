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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VarejoSimples.Controller;
using VarejoSimples.Enums;
using VarejoSimples.Interfaces;
using VarejoSimples.Views.Conta;
using VarejoSimples.Views.Plano_conta;

namespace VarejoSimples.Views.Lancamento_financ.Transferencia
{
    /// <summary>
    /// Interação lógica para ContaOrigem.xam
    /// </summary>
    public partial class ContaOrigem : UserControl, ITelaTransferenciaConta
    {
        public ContaOrigem()
        {
            InitializeComponent();

            List<KeyValuePair<Tipo_lancamento, string>> tipos = new List<KeyValuePair<Tipo_lancamento, string>>();
            tipos.Add(new KeyValuePair<Tipo_lancamento, string>(Tipo_lancamento.ENTRADA, "ENTRADA"));
            tipos.Add(new KeyValuePair<Tipo_lancamento, string>(Tipo_lancamento.SAIDA, "SAIDA"));

            cbTipo.ItemsSource = tipos;
            cbTipo.DisplayMemberPath = "Value";
            cbTipo.SelectedValuePath = "Key";
            cbTipo.SelectedIndex = 0;

            txValor.ToMoney();
        }

        public int Conta_id
        {
            get
            {
                return int.Parse(txCod_conta.Text);
            }
        }

        public UserControl CurrentView
        {
            get
            {
                return this;
            } 
        }

        public int Plano_conta_id
        {
            get
            {
                return int.Parse(txCod_plano_conta.Text);
            }
        }

        public Tipo_lancamento Tipo_lancamento
        {
            get
            {
                return (Tipo_lancamento)cbTipo.SelectedValue;
            }
        }

        public decimal Valor
        {
            get
            {
                if (string.IsNullOrEmpty(txValor.Text))
                    txValor.Text = "0,00";

                return decimal.Parse(txValor.Text);
            }
            set
            {
                txValor.Text = value.ToString("N2");
            }
        }

        private void btSelecionarConta_Click(object sender, RoutedEventArgs e)
        {
            PesquisarConta pc = new PesquisarConta(false);
            pc.ShowDialog();

            txCod_conta.Text = pc.Selecionado.Id.ToString();
            txNome_conta.Text = (pc.Selecionado.Id == 0
                ? "Não selecionado"
                : pc.Selecionado.Nome);
        }

        private void btSelecionarPlanoConta_Click(object sender, RoutedEventArgs e)
        {
            SelecionarPlanoConta spc = new SelecionarPlanoConta();
            spc.ShowDialog();

            txCod_plano_conta.Text = spc.Selecionado.Id.ToString();
            txNome_plano_conta.Text = (spc.Selecionado.Id == 0
                ? "Não selecionado"
                : spc.Selecionado.Descricao);
        }

        private void txValor_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txValor.Text))
                txValor.Text = "0,00";
        }
    }
}
