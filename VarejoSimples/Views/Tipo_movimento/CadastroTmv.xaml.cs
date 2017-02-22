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
using VarejoSimples.Views.Plano_conta;

namespace VarejoSimples.Views.Tipo_movimento
{
    /// <summary>
    /// Lógica interna para CadastroTmv.xaml
    /// </summary>
    public partial class CadastroTmv : Window
    {
        private Tipos_movimentoController controller = null;
        public CadastroTmv()
        {
            InitializeComponent();

            controller = new Tipos_movimentoController();
            txCod.ToNumeric();
            txCfop.ToNumeric();
            txDescricao.Focus();

            List<KeyValuePair<int, string>> tipos_movimentacao = new List<KeyValuePair<int, string>>();
            tipos_movimentacao.Add(new KeyValuePair<int, string>(0, "Saída"));
            tipos_movimentacao.Add(new KeyValuePair<int, string>(1, "Entrada"));
            tipos_movimentacao.Add(new KeyValuePair<int, string>(2, "Nenhum"));

            cbMov_itens.ItemsSource = tipos_movimentacao;
            cbMov_itens.DisplayMemberPath = "Value";
            cbMov_itens.SelectedValuePath = "Key";

            cbMov_valores.ItemsSource = tipos_movimentacao;
            cbMov_valores.DisplayMemberPath = "Value";
            cbMov_valores.SelectedValuePath = "Key";

            cbMov_itens.SelectedIndex = 0;
            cbMov_valores.SelectedIndex = 0;

            txCod_plano_conta.ToNumeric();
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
            Tipos_movimento tmv = (int.Parse(txCod.Text) == 0
                ? new Tipos_movimento()
                : controller.Find(int.Parse(txCod.Text)));
            
            tmv.Id = int.Parse(txCod.Text);
            tmv.Descricao = txDescricao.Text;
            tmv.Movimentacao_itens = (int)cbMov_itens.SelectedValue;
            tmv.Movimentacao_valores = (int)cbMov_valores.SelectedValue;
            tmv.Cfop = (string.IsNullOrEmpty(txCfop.Text) 
                ? 0
                : int.Parse(txCfop.Text));
            tmv.Gera_comissao = ckGera_comissao.IsChecked.Value;
            tmv.Utiliza_fornecedor = ckFornecedor.IsChecked.Value;
            tmv.Plano_conta_id = int.Parse(txCod_plano_conta.Text);

            if (controller.Save(tmv))
                LimparCampos();
        }

        private void LimparCampos()
        {
            txCod.Text = "0";
            txDescricao.Text = string.Empty;
            txCfop.Text = "0";
            ckGera_comissao.IsChecked = false;
            ckFornecedor.IsChecked = false;
            txDescricao.Focus();
            cbMov_itens.SelectedValue = 0;
            cbMov_valores.SelectedValue = 0;
            txCod_plano_conta.Text = "0";
            txPlano_conta.Text = string.Empty;
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void FillTmv(Tipos_movimento tmv)
        {
            if (tmv == null)
                return;

            txCod.Text = tmv.Id.ToString();
            txDescricao.Text = tmv.Descricao;
            cbMov_itens.SelectedValue = tmv.Movimentacao_itens;
            cbMov_valores.SelectedValue = tmv.Movimentacao_valores;
            txCfop.Text = tmv.Cfop.ToString();
            ckGera_comissao.IsChecked = tmv.Gera_comissao;
            ckFornecedor.IsChecked = tmv.Utiliza_fornecedor;
            txCod_plano_conta.Text = tmv.Plano_conta_id.ToString();
            txPlano_conta.Text = tmv.Planos_contas.Descricao;
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            Tipos_movimento tmv = controller.Next(int.Parse(txCod.Text));
            FillTmv(tmv);
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            if ((int.Parse(txCod.Text) - 1) <= 0)
            {
                LimparCampos();
                return;
            }

            Tipos_movimento tmv = controller.Prev(int.Parse(txCod.Text));
            FillTmv(tmv);
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCod.Text);
            if (id == 0)
                return;

            if (controller.Remove(id))
                LimparCampos();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F3)
            {
                PesquisarTmv pt = new PesquisarTmv();
                pt.ShowDialog();

                FillTmv(pt.Selecionado);
            }
        }

        private void btSelecionar_conta_pai_Click(object sender, RoutedEventArgs e)
        {
            SelecionarPlanoConta spc = new SelecionarPlanoConta();
            spc.ShowDialog();

            txCod_plano_conta.Text = spc.Selecionado.Id.ToString();
            txPlano_conta.Text = (spc.Selecionado.Id == 0
                ? "Não selecionado"
                : spc.Selecionado.Descricao);
        }
    }
}
