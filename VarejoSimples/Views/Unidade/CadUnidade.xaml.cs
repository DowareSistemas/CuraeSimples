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

namespace VarejoSimples.Views.Unidade
{
    /// <summary>
    /// Lógica interna para CadUnidade.xaml
    /// </summary>
    public partial class CadUnidade : Window
    {
        private UnidadesController controller;
        public CadUnidade()
        {
            InitializeComponent();
            controller = new UnidadesController();
            txCod.ToNumeric();
            txSigla.Focus();
        }

        private void Save()
        {
            Unidades un = (int.Parse(txCod.Text) == 0
                               ? new Unidades()
                               : controller.Find(int.Parse(txCod.Text)));

            un.Id = int.Parse(txCod.Text);
            un.Sigla = txSigla.Text;
            un.Nome = txNome.Text;

            if (controller.Save(un))
                LimparCampos();
        }

        private void LimparCampos()
        {
            txCod.Text = "0";
            txSigla.Text = string.Empty;
            txNome.Text = string.Empty;
            txSigla.Focus();
        }

        private void FillUn(Unidades un)
        {
            if (un == null)
                return;

            txCod.Text = un.Id.ToString();
            txSigla.Text = un.Sigla;
            txNome.Text = un.Nome;

            txSigla.Focus();
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            Unidades un = controller.Next(int.Parse(txCod.Text));
            FillUn(un);
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCod.Text);
            if ((id - 1) <= 0)
            {
                LimparCampos();
                return;
            }

            Unidades un = controller.Prev(id);
            FillUn(un);
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
            if (id <= 0)
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
            if (e.Key == Key.F3)
            {
                PesquisarUnidade pu = new PesquisarUnidade();
                pu.ShowDialog();

                FillUn(pu.Selecionado);
            }
        }
    }
}
