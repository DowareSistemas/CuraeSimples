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
using VarejoSimples.Model;

namespace VarejoSimples.Views.Vendedor
{
    /// <summary>
    /// Interação lógica para PesquisarVendedor.xam
    /// </summary>
    public partial class PesquisarVendedor : Window
    {
        public Vendedores Selecionado = new Vendedores();
        public PesquisarVendedor()
        {
            InitializeComponent();

            dataGrid.AplicarPadroes();
            txPesquisa.Focus();
            Pesquisar();
        }

        private void Pesquisar()
        {
            List<Vendedores> list = new VendedoresController().Search(txPesquisa.Text);
            dataGrid.ItemsSource = list;
        }

        private void txPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Pesquisar();
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Selecionar();
        }

        private void Selecionar()
        {
            Vendedores v = (Vendedores)dataGrid.SelectedItem;
            if (v == null)
                return;
            if (v.Id == 0)
                return;

            Selecionado = v;
            Close();
        }

        private void btSelecionar_Click(object sender, RoutedEventArgs e)
        {
            Selecionar();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
