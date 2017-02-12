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
    /// Lógica interna para PesquisarMarca.xaml
    /// </summary>
    public partial class PesquisarMarca : Window
    {
        public Marcas Selecionado = new Marcas();
        public PesquisarMarca()
        {
            InitializeComponent();
            Pesquisar();
            dataGrid.AplicarPadroes();
            txPesquisa.Focus();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btSelecionar_Click(object sender, RoutedEventArgs e)
        {
            Selecionar();
        }

        private void Selecionar()
        {
            Marcas m = (Marcas)dataGrid.SelectedItem;
            if (m == null)
                return;
            if (m.Id == 0)
                return;

            Selecionado = m;
            Close();
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Selecionar();
        }

        private void txPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Pesquisar();
        }

        private void Pesquisar()
        {
            List<Marcas> list = new MarcasController().Search(txPesquisa.Text);
            dataGrid.ItemsSource = list;
        }
    }
}
