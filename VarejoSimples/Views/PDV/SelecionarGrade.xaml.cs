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
    /// Lógica interna para SelecionarGrade.xaml
    /// </summary>
    public partial class SelecionarGrade : Window
    {
        public Estoque Selecionado = new Estoque();
        public SelecionarGrade(Produtos produto)
        {
            InitializeComponent();

            lbDescricaoProduto.Content = produto.Descricao;
            dataGrid.AplicarPadroes();
            dataGrid.ItemsSource = new Grades_produtosController().ListByProduto(produto.Id);
            dataGrid.Focus();
            dataGrid.SelectedIndex = 0;
        }

        private void btFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Grades_produtos grade = (Grades_produtos)dataGrid.SelectedItem;
                if (grade == null)
                    return;

                Selecionado = new EstoqueController().BuscarPorGrade(grade.Identificador);
                Close();
            }
        }
    }
}
