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

namespace VarejoSimples.Views.Marca
{
    /// <summary>
    /// Lógica interna para CadMarcas.xaml
    /// </summary>
    public partial class CadMarcas : Window
    {
        MarcasController controller;
        public CadMarcas()
        {
            InitializeComponent();
            txCod.ToNumeric();
            txNome.Focus();
            controller = new MarcasController();
        }

        private void Save()
        {
            Marcas m = (int.Parse(txCod.Text) == 0
                        ? new Marcas()
                        : controller.Find(int.Parse(txCod.Text)));

            m.Id = int.Parse(txCod.Text);
            m.Nome = txNome.Text;

            if (controller.Save(m))
                LimparCampos();
        }

        private void LimparCampos()
        {
            txCod.Text = "0";
            txNome.Text = string.Empty;
            txNome.Focus();
        }

        private void FillMarcas(Marcas m)
        {
            if (m == null)
                return;

            txCod.Text = m.Id.ToString();
            txNome.Text = m.Nome;
            txNome.Focus();
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            Marcas m = controller.Next(int.Parse(txCod.Text));
            FillMarcas(m);
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            if ((int.Parse(txCod.Text) - 1) <= 0)
            {
                LimparCampos();
                return;
            }

            Marcas m = controller.Prev(int.Parse(txCod.Text));
            FillMarcas(m);
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
            if (e.Key == Key.F3)
            {
                PesquisarMarca pm = new PesquisarMarca();
                pm.ShowDialog();

                FillMarcas(pm.Selecionado);
            }
        }
    }
}
