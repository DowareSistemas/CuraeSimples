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

namespace VarejoSimples.Views.PDV
{
    /// <summary>
    /// Lógica interna para BuscaClientePdv.xaml
    /// </summary>
    public partial class BuscaClientePdv : Window
    {
        private ClientesController controller = null;
        public Clientes Selecionado = new Clientes();

        public BuscaClientePdv()
        {
            InitializeComponent();
            controller = new ClientesController();
            Pesquisar();
            txPesquisa.Focus();
            dataGrid.AplicarPadroes();
        }

        private void btFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Pesquisar();

                txPesquisa.IsEnabled = false;
                dataGrid.Focus();
                dataGrid.SelectedIndex = 0;
            }
        }

        private void Pesquisar()
        {
            List<Clientes> list = controller.Search(txPesquisa.Text);
            dataGrid.ItemsSource = list;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                Pesquisar();

                txPesquisa.IsEnabled = false;
                dataGrid.Focus();
                dataGrid.SelectedIndex = 0;
            }

            if (e.Key == Key.F1)
            {
                txPesquisa.IsEnabled = true;
                txPesquisa.Focus();
                txPesquisa.SelectAll();
            }
            
            if (e.Key == Key.F2)
                ShowCadastro();

            if (e.Key == Key.Escape)
                Close();
        }

        private void ShowCadastro()
        {
            CadastroRapidoCliente crc = new CadastroRapidoCliente();
            crc.ShowDialog();

            if(crc.ClienteCadastrado.Id > 0)
            {
                Selecionado = crc.ClienteCadastrado;
                Close();
            }
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Clientes cliente = (Clientes)dataGrid.SelectedItem;

                if (cliente == null)
                    return;

                Selecionado = cliente;
                Close();
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Pesquisar();
            txPesquisa.IsEnabled = false;
            dataGrid.Focus();
            dataGrid.SelectedIndex = 0;
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
