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

namespace VarejoSimples.Views.Conta
{
    /// <summary>
    /// Lógica interna para PesquisarConta.xaml
    /// </summary>
    public partial class PesquisarConta : Window
    {
        public Contas Selecionado = new Contas();
        private bool exibir_inativos = false;
        public PesquisarConta(bool exibir_inativos)
        {
            this.exibir_inativos = exibir_inativos;
            InitializeComponent();

            dataGrid.AplicarPadroes();
            txPesquisa.Focus();
            Pesquisar();
        }

        private void Pesquisar()
        {
            List<Contas> contas = new ContasController().Search(txPesquisa.Text, exibir_inativos);
            dataGrid.ItemsSource = contas;
        }
        
        private void Selecionar()
        {
            Contas conta = (Contas)dataGrid.SelectedItem;

            if (conta == null)
                return;
            if (conta.Id == 0)
                return;

            Selecionado = conta;
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

        private void txPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Pesquisar();
        }
    }
}
