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
using VarejoSimples.Interfaces;
using VarejoSimples.Model;

namespace VarejoSimples.Views.PDV
{
    /// <summary>
    /// Interaction logic for CancelarItem.xaml
    /// </summary>
    public partial class CancelarItem : Window
    {
        public CancelarItem(IPainelVenda painelVenda)
        {
            InitializeComponent();

            dataGrid.ItemsSource = painelVenda.GetItensMovimento();
            dataGrid.AplicarPadroes();
            dataGrid.Focus();
            dataGrid.SelectedIndex = 0;
        }

        private void btFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
