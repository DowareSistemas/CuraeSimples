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

namespace VarejoSimples.Views.Tipo_movimento
{
    /// <summary>
    /// Lógica interna para PesquisarTmv.xaml
    /// </summary>
    public partial class PesquisarTmv : Window
    {
        public Tipos_movimento Selecionado = new Tipos_movimento();
        public PesquisarTmv()
        {
            InitializeComponent();
            dataGrid.AplicarPadroes();
            Pesquisar();
            txPesquisa.Focus();
        }

        private void Pesquisar()
        {
            List<Tipos_movimento> list = new Tipos_movimentoController().Search(txPesquisa.Text);
            dataGrid.ItemsSource = list;
        }

        private void txPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Pesquisar();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Selecionar();
        }

        private void Selecionar()
        {
            Tipos_movimento tmv = (Tipos_movimento)dataGrid.SelectedItem;
            if (tmv == null)
                return;
            if (tmv.Id == 0)
                return;

            Selecionado = tmv;
            Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Selecionar();
        }
    }
}
