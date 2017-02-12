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

namespace VarejoSimples.Views.Unidade
{
    /// <summary>
    /// Lógica interna para PesquisarUnidade.xaml
    /// </summary>
    public partial class PesquisarUnidade : Window
    {
        public Unidades Selecionado = new Unidades();
        public PesquisarUnidade()
        {
            InitializeComponent();
            dataGrid.AplicarPadroes();
            txPesquisa.Focus();
            Pesquisar();
        }

        private void Pesquisar()
        {
            List<Unidades> list = new UnidadesController().Search(txPesquisa.Text);
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
            Unidades un = (Unidades)dataGrid.SelectedItem;
            if (un == null)
                return;
            if (un.Id == 0)
                return;

            Selecionado = un;
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
    }
}
