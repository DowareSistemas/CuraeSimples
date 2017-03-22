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

namespace VarejoSimples.Views.Cor
{
    /// <summary>
    /// Lógica interna para PesquisarCor.xaml
    /// </summary>
    public partial class PesquisarCor : Window
    {
        public Cores Selecionado = new Cores();

        public PesquisarCor()
        {
            InitializeComponent();

            dataGrid.AplicarPadroes();
            txPesquisa.Focus();
            Pesquisar();
        }

        private void Pesquisar()
        {
            List<Cores> cores = new CoresController().Search(txPesquisa.Text);
            dataGrid.ItemsSource = cores;
        }

        private void btSelecionar_Click(object sender, RoutedEventArgs e)
        {
            Selecionar();
        }

        private void Selecionar()
        {
            Cores cor = (Cores)dataGrid.SelectedItem;
            if (cor == null)
                return;
            if (cor.Id == 0)
                return;

            Selecionado = cor;
            Close();
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

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Selecionar();
        }
    }
}
