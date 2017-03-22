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
                Pesquisar();
        }

        private void Pesquisar()
        {
            List<Clientes> list = controller.Search(txPesquisa.Text);
            dataGrid.ItemsSource = list;
        }
    }
}
