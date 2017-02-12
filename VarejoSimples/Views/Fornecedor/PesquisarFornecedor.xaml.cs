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

namespace VarejoSimples.Views.Fornecedor
{
    /// <summary>
    /// Lógica interna para PesquisarFornecedor.xaml
    /// </summary>
    public partial class PesquisarFornecedor : Window
    {
        public Fornecedores Selecionado = new Fornecedores();
        public PesquisarFornecedor()
        {
            InitializeComponent();
            txPesquisa.Focus();
            dataGrid.AplicarPadroes();
            Pesquisar();
        }

        private void Pesquisar()
        {
            List<Fornecedores> list = new FornecedoresController().Search(txPesquisa.Text);
            dataGrid.ItemsSource = list;
        }

        private void txPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Pesquisar();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Selecionar();
        }

        private void Selecionar()
        {
            Fornecedores f = (Fornecedores)dataGrid.SelectedItem;
            if (f == null)
                return;
            if (f.Id == 0)
                return;

            Selecionado = f;
            Close();
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Selecionar();
        }
    }
}
