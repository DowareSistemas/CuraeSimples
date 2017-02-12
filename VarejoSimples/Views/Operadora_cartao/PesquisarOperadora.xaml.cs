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

namespace VarejoSimples.Views.Operadora_cartao
{
    /// <summary>
    /// Lógica interna para PesquisarOperadora.xaml
    /// </summary>
    public partial class PesquisarOperadora : Window
    {
        private bool MostrarInativo = false;
        public Operadoras_cartao Selecionado = new Operadoras_cartao();

        public PesquisarOperadora(bool inativos)
        {
            InitializeComponent();

            dataGrid.AplicarPadroes();
            MostrarInativo = inativos;
            Pesquisar();
        }

        private void Pesquisar()
        {
            List<Operadoras_cartao> list = new Operadoras_cartaoController().Search(txPesquisa.Text, MostrarInativo);
            dataGrid.ItemsSource = list;
        }

        private void Selecionar()
        {
            Operadoras_cartao op = (Operadoras_cartao)dataGrid.SelectedItem;
            if (op == null)
                return;
            if (op.Id == 0)
                return;

            Selecionado = op;
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

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Pesquisar();
        }
    }
}
