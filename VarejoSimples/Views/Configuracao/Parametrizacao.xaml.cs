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

namespace VarejoSimples.Views.Configuracao
{
    /// <summary>
    /// Lógica interna para Parametrizacao.xaml
    /// </summary>
    public partial class Parametrizacao : Window
    {
        private ParametrosController controller;
        public Parametrizacao()
        {
            InitializeComponent();

            controller = new ParametrosController();
            dataGrid.AplicarPadroes();
            Pesquisar();
        }

        private void Pesquisar()
        {
            List<Parametros> list =  new ParametrosController().Search(txPesquisa.Text);
            dataGrid.ItemsSource = list;
        }

        private void btAplicar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txNome.Text))
            {
                MessageBox.Show("Não existe parametro selecionado!", "ARQVAZIO", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            Parametros param = (Parametros)dataGrid.SelectedItem;

            if (param == null)
            {
                MessageBox.Show("Nenhum parâmetro selecionado!", "ARQVAZIO", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            controller.SetValorParametro(txNome.Text, param.Computador, txComputador.Text, txValor.Text);
            Pesquisar();
            LimparCampos();
        }

        private void LimparCampos()
        {
            txNome.Text = string.Empty;
            txDescricao.Text = string.Empty;
            txComputador.Text = string.Empty;
            txValor.Text = string.Empty;
        }

        private void txPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Pesquisar();
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Parametros param = (Parametros)dataGrid.SelectedItem;

            if (param == null)
                return;
            if (param.Nome == null)
                return;

            txNome.Text = param.Nome;
            txDescricao.Text = param.Descricao;
            txComputador.Text = param.Computador;
            txValor.Text = param.Valor;

            txComputador.IsEnabled = param.Permite_multi;
        }

        private void btNovo_Click(object sender, RoutedEventArgs e)
        {
            Parametros param = (Parametros)dataGrid.SelectedItem;
            if (param == null)
                return;
            if (string.IsNullOrEmpty(param.Nome))
                return;

            if (!param.Permite_multi)
            {
                MessageBox.Show("Este parâmetro é de nível global e não permite criação de novas regras.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            NovaRegraParametro nrp = new NovaRegraParametro(txNome.Text);
            nrp.ShowDialog();
            LimparCampos();
            Pesquisar();
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            Parametros param = (Parametros)dataGrid.SelectedItem;
            if (param == null)
                return;
            if (string.IsNullOrEmpty(param.Nome))
                return;

            controller.ExcluirRegra(param.Nome, param.Computador);
            LimparCampos();
            Pesquisar();
        }
    }
}
