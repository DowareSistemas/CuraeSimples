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

namespace VarejoSimples.Views.Loja
{
    /// <summary>
    /// Lógica interna para PesquisarLoja.xaml
    /// </summary>
    public partial class PesquisarLoja : Window
    {
        public Lojas Selecionado = new Lojas();
        public PesquisarLoja()
        {
            InitializeComponent();
            Pesquisar();
            dataGrid.AplicarPadroes();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Pesquisar()
        {
            List<Lojas> list = new LojasController().Search(txPesquisa.Text);
            dataGrid.ItemsSource = list;
        }

        private void Selecionar()
        {
            Lojas loja = (Lojas)dataGrid.SelectedItem;
            if (loja == null)
                return;
            if (loja.Id == 0)
                return;

            Selecionado = loja;
            Close();
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Selecionar();
        }
    }
}
