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

namespace VarejoSimples.Views.Operadora_cartao
{
    /// <summary>
    /// Lógica interna para CadOperadora.xaml
    /// </summary>
    public partial class CadOperadora : Window
    {
        private Operadoras_cartaoController controller;
        public CadOperadora()
        {
            InitializeComponent();

            controller = new Operadoras_cartaoController();
            List<KeyValuePair<int, string>> tipos_receb = new List<KeyValuePair<int, string>>();
            tipos_receb.Add(new KeyValuePair<int, string>((int)Tipo_recebimento.DIAS, "Dias"));
            tipos_receb.Add(new KeyValuePair<int, string>((int)Tipo_recebimento.HORAS, "Horas"));

            cbTipo_receb.ItemsSource = tipos_receb;
            cbTipo_receb.DisplayMemberPath = "Value";
            cbTipo_receb.SelectedValuePath = "Key";

            cbTipo_receb.SelectedIndex = 0;

            txPrazo_rec.ToNumeric();
            txCod.ToNumeric();
            txTaxa.ToMoney();

            txNome.Focus();
        }

        private void Save()
        {
            Operadoras_cartao op = (int.Parse(txCod.Text) == 0
                            ? new Operadoras_cartao()
                            : controller.Find(int.Parse(txCod.Text)));

            op.Id = int.Parse(txCod.Text);
            op.Nome = txNome.Text;
            op.Tipo_recebimento =  (int)cbTipo_receb.SelectedValue;
            op.Prazo_recebimento = int.Parse(txPrazo_rec.Text);
            op.Taxa = decimal.Parse(txTaxa.Text);
            op.Inativo = ckInativo.IsChecked.Value;

            if (controller.Save(op))
                LimparCampos();
        }

        private void LimparCampos()
        {
            txCod.Text = "0";
            txNome.Text = string.Empty;
            cbTipo_receb.SelectedIndex = 0;
            txPrazo_rec.Text = "0";
            txTaxa.Text ="0";
            ckInativo.IsChecked = false;

            txNome.Focus();
        }

        private void FillOp(Operadoras_cartao op)
        {
            if (op == null)
                return;

            txCod.Text = op.Id.ToString();
            txNome.Text = op.Nome;
            cbTipo_receb.SelectedIndex = op.Tipo_recebimento;
            txPrazo_rec.Text = op.Prazo_recebimento.ToString();
            txTaxa.Text = op.Taxa.ToString("N2");
            ckInativo.IsChecked = op.Inativo;

            txNome.Focus();
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            Operadoras_cartao op = controller.Next(int.Parse(txCod.Text));
            FillOp(op);
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            if((int.Parse(txCod.Text) - 1) <= 0)
            {
                LimparCampos();
                return;
            }

            Operadoras_cartao op = controller.Prev(int.Parse(txCod.Text));
            FillOp(op);
        }

        private void btNovo_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            Save();
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

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F3)
            {
                PesquisarOperadora po = new PesquisarOperadora(true);
                po.ShowDialog();

                FillOp(po.Selecionado);
            }
        }
    }
}
