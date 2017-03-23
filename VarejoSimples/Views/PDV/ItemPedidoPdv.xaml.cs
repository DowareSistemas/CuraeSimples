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
using VarejoSimples.Model;

namespace VarejoSimples.Views.PDV
{
    /// <summary>
    /// Interação lógica para ItemPedidoPdv.xam
    /// </summary>
    public partial class ItemPedidoPdv : UserControl
    {
        public ItemPedidoPdv(Pedidos_venda pedido)
        {
            InitializeComponent();

            lbId.Content = pedido.Id.ToString();
            lbNome_cliente.Content = pedido.Clientes.Nome;
            lbTotal_pedido.Content = $"R$ {pedido.Itens_pedido.Sum(e => e.Valor_final)}";
            lbData.Content = pedido.Data.ToString("dd/MM/yyyy HH:mm:ss");

            if (pedido.Itens_pedido.Count == 1)
                lbNumero_produtos.Content = $"{pedido.Itens_pedido.Count} produto";
            else
                lbNumero_produtos.Content = $"{pedido.Itens_pedido.Count} produtos";
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(@"pack://application:,,,/Curae Varejo (Simples);component/Images/ir_pedido_press.png");
            img.EndInit();
            image.Source = img;
            btAbrirPedido.Background = (new BrushConverter().ConvertFromString("Blue") as Brush);
        }

        private void StackPanel_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void btAbrirPedido_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(@"pack://application:,,,/Curae Varejo (Simples);component/Images/ir_pedido.png");
            img.EndInit();
            image.Source = img;
            btAbrirPedido.Background = (new BrushConverter().ConvertFromString("Transparent") as Brush);
        }
    }
}
