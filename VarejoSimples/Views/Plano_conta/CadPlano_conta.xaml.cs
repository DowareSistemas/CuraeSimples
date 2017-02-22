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

namespace VarejoSimples.Views.Plano_conta
{
    /// <summary>
    /// Lógica interna para CadPlano_conta.xaml
    /// </summary>
    public partial class CadPlano_conta : Window
    {
        private Planos_contasController controller;
        public CadPlano_conta()
        {
            InitializeComponent();

            controller = new Planos_contasController();
            ListaPlanosContas();

            txCod.ToNumeric();
            txCod_conta_pai.ToNumeric();

            List<KeyValuePair<Classe_plano_conta, string>> classes_planos = new List<KeyValuePair<Classe_plano_conta, string>>();
            classes_planos.Add(new KeyValuePair<Classe_plano_conta, string>(Classe_plano_conta.RECEITA, "Receita"));
            classes_planos.Add(new KeyValuePair<Classe_plano_conta, string>(Classe_plano_conta.DESPESA, "Despesa"));
            classes_planos.Add(new KeyValuePair<Classe_plano_conta, string>(Classe_plano_conta.NAO_CLASSIFICADO, "Não classificado"));

            cbClasse.ItemsSource = classes_planos;
            cbClasse.DisplayMemberPath = "Value";
            cbClasse.SelectedValuePath = "Key";
            cbClasse.SelectedIndex = 0;

            List<KeyValuePair<Tipo_plano_conta, string>> tipos_planos = new List<KeyValuePair<Tipo_plano_conta, string>>();
            tipos_planos.Add(new KeyValuePair<Tipo_plano_conta, string>(Tipo_plano_conta.ANALITICO, "Analítico"));
            tipos_planos.Add(new KeyValuePair<Tipo_plano_conta, string>(Tipo_plano_conta.SINTETICO, "Sintético"));

            cbTipo.ItemsSource = tipos_planos;
            cbTipo.DisplayMemberPath = "Value";
            cbTipo.SelectedValuePath = "Key";
            cbTipo.SelectedIndex = 0;

            txDescricao.Focus();
            txCod_conta_pai.ToNumeric();
        }

        private void ListaPlanosContas()
        {
            treeView.Items.Clear();
            List<Planos_contas> list = controller.Search("");

            foreach (Planos_contas plano in list)
            {
                if (plano.Conta_pai > 0)
                    continue;

                TreeViewItem tItem = new TreeViewItem();
                tItem.Header = plano.Descricao;

                treeView.Items.Add(tItem);

                if (controller.TemFilhos(plano.Id))
                    ListaPlanosFilhos(tItem, plano.Id);
            }
        }

        private void ListaPlanosFilhos(TreeViewItem tItem, int plano_conta_pai)
        {
            List<Planos_contas> planos = controller.GetFilhos(plano_conta_pai);

            foreach (Planos_contas plano in planos)
            {
                TreeViewItem ntItem = new TreeViewItem();
                ntItem.Header = plano.Descricao;

                tItem.Items.Add(ntItem);
                if (controller.TemFilhos(plano.Id))
                    ListaPlanosFilhos(ntItem, plano.Id);
            }
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCod.Text);
            Planos_contas plano = controller.Next(id);
            FillPlano(plano);
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCod.Text);
            if ((id - 1) <= 0)
            {
                LimparCampos();
                return;
            }

            Planos_contas plano = controller.Prev(id);
            FillPlano(plano);
        }

        private void FillPlano(Planos_contas pc)
        {
            if (pc == null)
                return;

            txCod.Text = pc.Id.ToString();
            txDescricao.Text = pc.Descricao;

            Classe_plano_conta classe = (Classe_plano_conta)pc.Classe;
            Tipo_plano_conta tipo = (Tipo_plano_conta)pc.Tipo;

            cbClasse.SelectedValue = classe;
            cbTipo.SelectedValue = tipo;

            if(pc.Conta_pai > 0)
            {
                txCod_conta_pai.Text = pc.Conta_pai.ToString();
                txConta_pai.Text = controller.Find(pc.Conta_pai).Descricao;
            }

            txDescricao.Focus();
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
            Classe_plano_conta classe = (Classe_plano_conta)cbClasse.SelectedValue;
            Tipo_plano_conta tipo = (Tipo_plano_conta)cbTipo.SelectedValue;

            Planos_contas plano = (int.Parse(txCod.Text) == 0
                ? new Planos_contas()
                : controller.Find(int.Parse(txCod.Text)));

            plano.Descricao = txDescricao.Text;
            plano.Tipo = (int)tipo;
            plano.Classe = (int)classe;
            plano.Conta_pai = int.Parse(txCod_conta_pai.Text);

            if (controller.Save(plano))
                LimparCampos();

            ListaPlanosContas();
        }

        private void LimparCampos()
        {
            txCod.Text = "0";
            txDescricao.Text = string.Empty;
            txCod_conta_pai.Text = "0";
            txConta_pai.Text = string.Empty;

            txDescricao.Focus();
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCod.Text);
            if (id <= 0)
                return;

            if (controller.Remove(id))
                LimparCampos();

            ListaPlanosContas();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btSelecionar_conta_pai_Click(object sender, RoutedEventArgs e)
        {
            SelecionarPlanoConta spc = new SelecionarPlanoConta();
            spc.ShowDialog();

            txCod_conta_pai.Text = spc.Selecionado.Id.ToString();
            txConta_pai.Text = (spc.Selecionado.Id == 0
                ? "Não selecionado"
                : spc.Selecionado.Descricao);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F3)
            {
                SelecionarPlanoConta spc = new SelecionarPlanoConta();
                spc.ShowDialog();

                if (spc.Selecionado.Id > 0)
                    FillPlano(spc.Selecionado);
            }
        }
    }
}
