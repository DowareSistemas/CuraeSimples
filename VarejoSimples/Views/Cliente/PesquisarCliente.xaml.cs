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

namespace VarejoSimples.Views.Cliente
{
    /// <summary>
    /// Lógica interna para PesquisarCliente.xaml
    /// </summary>
    public partial class PesquisarCliente : Window
    {
        public Clientes Selecionado = new Clientes();
        public PesquisarCliente()
        {
            InitializeComponent();
            txPesquisa.Focus();
            dataGrid.AplicarPadroes();
            Pesquisar();
        }

        private void txPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Pesquisar();
        }

        private void Pesquisar()
        {
            List<Clientes> list = new ClientesController().Search(txPesquisa.Text);
            dataGrid.ItemsSource = list;
        }

        private void Selecionar()
        {
            Clientes c = (Clientes)dataGrid.SelectedItem;
            if (c == null)
                return;
            if (c.Id == 0)
                return;

            Selecionado = c;
            Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Selecionar();
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Selecionar();
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
