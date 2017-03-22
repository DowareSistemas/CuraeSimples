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

namespace VarejoSimples.Views.Produto
{
    /// <summary>
    /// Lógica interna para AdicionarItemGrade.xaml
    /// </summary>
    public partial class AdicionarItemGrade : Window
    {
        public Cores Cor { get; set; }
        public Tamanhos Tamanho { get; set; }

        public AdicionarItemGrade()
        {
            InitializeComponent();

            Cor = new Cores();
            Tamanho = new Tamanhos();

            dataGrid_cores.ItemsSource = new CoresController().Search("");
            dataGrid_tamanhos.ItemsSource = new TamanhosController().Search("");

            dataGrid_tamanhos.AplicarPadroes();
            dataGrid_cores.AplicarPadroes();
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            Cor = (Cores)dataGrid_cores.SelectedItem;
            Tamanho = (Tamanhos)dataGrid_tamanhos.SelectedItem;

            Close();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
