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

namespace VarejoSimples.Views.Plano_conta
{
    /// <summary>
    /// Lógica interna para SelecionarPlanoConta.xaml
    /// </summary>
    public partial class SelecionarPlanoConta : Window
    {
        public Planos_contas Selecionado = new Planos_contas();

        public SelecionarPlanoConta()
        {
            InitializeComponent();

            dataGrid.AplicarPadroes();
            Pesquisar();
            txPesquisa.Focus();
        }

        private void Pesquisar()
        {
            List<Planos_contas> list = new Planos_contasController().Search(txPesquisa.Text);
            dataGrid.ItemsSource = list;
        }

        private void txPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Pesquisar();
        }

        private void Selecionar()
        {
            Planos_contas plano = (Planos_contas)dataGrid.SelectedItem;
            if (plano == null)
                return;
            if (plano.Id == 0)
                return;

            Selecionado = plano;
            Close();
        }

        private void btSelecionar_Click(object sender, RoutedEventArgs e)
        {
            Selecionar();
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
