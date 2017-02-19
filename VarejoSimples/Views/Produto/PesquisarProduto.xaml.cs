using System;
using System.Collections.Generic;
using System.IO;
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

namespace VarejoSimples.Views.Produto
{
    /// <summary>
    /// Lógica interna para PesquisarProduto.xaml
    /// </summary>
    public partial class PesquisarProduto : Window
    {
        public Produtos Selecionado = new Produtos();
        public PesquisarProduto()
        {
            InitializeComponent();
            dataGrid.AplicarPadroes();
            Pesquisar();
            txPesquisa.Focus();
            txEstoque.ToNumeric();
        }

        private void Pesquisar()
        {
            List<Produtos> list = new ProdutosController().Search(txPesquisa.Text);
            dataGrid.ItemsSource = list;
        }

        private void txPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Pesquisar();
        }

        private void btSelecionar_Click(object sender, RoutedEventArgs e)
        {
            Selecionar();
        }

        private void Selecionar()
        {
            Produtos p = (Produtos)dataGrid.SelectedItem;
            if (p == null)
                return;
            if (p.Id == 0)
                return;

            Selecionado = p;
            Close();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Selecionar();
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CarregaInfoProd();
        }

        private void CarregaInfoProd()
        {
            Produtos p = (Produtos)dataGrid.SelectedItem;
            if (p == null)
                return;
            if (p.Id == 0)
                return;

            fotoProduto.Source = null;
            txProduto.Text = string.Empty;
            txMarca.Text = string.Empty;
            txLocal.Text = string.Empty;
            txEstoque.Text = "0";

            try
            {
                if (p.Foto != null)
                {
                    string filename = $@"C:\Temp\Curae\prod{p.Id + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond}.jpg";
                    File.WriteAllBytes(filename, p.Foto);
                    fotoProduto.Source = new BitmapImage(new Uri(filename));
                }
            }
            catch { }

            MarcasController mc = new MarcasController();

            txProduto.Text = p.Descricao;
            txMarca.Text = (mc.Find(p.Marca_id) == null ? string.Empty : mc.Find(p.Marca_id).Nome);
            txLocal.Text = p.Localizacao;
            txEstoque.Text = "0";
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }
    }
}
