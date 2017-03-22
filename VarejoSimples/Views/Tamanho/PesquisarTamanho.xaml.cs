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

namespace VarejoSimples.Views.Tamanho
{
    /// <summary>
    /// Lógica interna para PesquisarTamanho.xaml
    /// </summary>
    public partial class PesquisarTamanho : Window
    {
        public Tamanhos Selecionado = new Tamanhos();
        public PesquisarTamanho()
        {
            InitializeComponent();

            dataGrid.AplicarPadroes();
            Pesquisar();
            txPesquisa.Focus();
        }

        private void Pesquisar()
        {
            List<Tamanhos> tams = new TamanhosController().Search(txPesquisa.Text);
            dataGrid.ItemsSource = tams;
        }

        private void Selecionar()
        {
            Tamanhos tam = (Tamanhos)dataGrid.SelectedItem;
            if (tam == null)
                return;
            if (tam.Id == 0)
                return;

            Selecionado = tam;
            Close();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btSelecionar_Click(object sender, RoutedEventArgs e)
        {
            Selecionar();
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Selecionar();
        }

        private void txPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Pesquisar();
        }
    }
}
