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

namespace VarejoSimples.Views.Fabricante
{
    /// <summary>
    /// Lógica interna para CadFabricantes.xaml
    /// </summary>
    public partial class CadFabricantes : Window
    {
        private FabricantesController controller;
        public CadFabricantes()
        {
            InitializeComponent();
            controller = new FabricantesController();
            txCod.ToNumeric();
            txNome.Focus();
        }

        private void FillFabricante(Fabricantes f)
        {
            if (f == null)
                return;

            txCod.Text = f.Id.ToString();
            txNome.Text = f.Nome;
        }

        private void LimparCampos()
        {
            txCod.Text = "0";
            txNome.Text = string.Empty;
            txNome.Focus();
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            Fabricantes f = controller.Next(int.Parse(txCod.Text));
            FillFabricante(f);
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            if ((int.Parse(txCod.Text) - 1) <= 0)
            {
                LimparCampos();
                return;
            }

            Fabricantes f = controller.Prev(int.Parse(txCod.Text));
            FillFabricante(f);
        }

        private void btNovo_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            Fabricantes f = (int.Parse(txCod.Text) == 0
                ? new Fabricantes()
                : controller.Find(int.Parse(txCod.Text)));

            f.Id = int.Parse(txCod.Text);
            f.Nome = txNome.Text;

            if (controller.Save(f))
                LimparCampos();
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
            if (e.Key == Key.F3)
            {
                PesquisarFabricante pf = new PesquisarFabricante();
                pf.ShowDialog();

                FillFabricante(pf.Selecionado);
            }
        }
    }
}
