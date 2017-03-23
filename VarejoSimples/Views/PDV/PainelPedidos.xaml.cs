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

namespace VarejoSimples.Views.PDV
{
    /// <summary>
    /// Interação lógica para PainelPedidos.xam
    /// </summary>
    public partial class PainelPedidos : UserControl
    {
        Pedidos_vendaController controller = new Pedidos_vendaController();

        public PainelPedidos()
        {
            InitializeComponent();
            Pesquisar();
        }

        private void Pesquisar()
        {
            List<Pedidos_venda> list = controller.Search(txPesquisa.Text);

            sp_pedidos.Children.Clear();
            list.ForEach(e => sp_pedidos.Children.Add(new ItemPedidoPdv(e)));
        }

        private void txPesquisa_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txPesquisa.Text))
                textBlock.Text = string.Empty;

            else
                textBlock.Text = "Pesquisar por cliente, valor ou nome de produto";
        }

        private void textBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txPesquisa.Focus();
        }

        private void txPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Pesquisar();
        }
    }
}
