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
    /// Lógica interna para PesquisarFabricante.xaml
    /// </summary>
    public partial class PesquisarFabricante : Window
    {
        public Fabricantes Selecionado = new Fabricantes();
        public PesquisarFabricante()
        {
            InitializeComponent();
            dataGrid.AplicarPadroes();
            Pesquisar();
            txPesquisa.Focus();
        }

        private void btSelecionar_Click(object sender, RoutedEventArgs e)
        {
            Selecionar();
        }

        private void Pesquisar()
        {
            List<Fabricantes> list = new FabricantesController().Search(txPesquisa.Text);
            dataGrid.ItemsSource = list;
        }

        private void Selecionar()
        {
            Fabricantes f = (Fabricantes)dataGrid.SelectedItem;
            if (f == null)
                return;
            if (f.Id == 0)
                return;

            Selecionado = f;
            Close();
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Selecionar();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Pesquisar();
        }
    }
}
