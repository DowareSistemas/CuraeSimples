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

namespace VarejoSimples.Views.PDV.MenuPDV
{
    /// <summary>
    /// Lógica interna para Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public bool CaixaFechado { get; private set; }

        public Menu()
        {
            InitializeComponent();

            CaixaFechado = false;
        }

        private void btFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btCaixa_Click(object sender, RoutedEventArgs e)
        {
            Menu_Caixa mCaixa = new Menu_Caixa();
            mCaixa.CaixaFechado += MCaixa_CaixaFechado;
            GridContainer.Children.Clear();
            GridContainer.Children.Add(mCaixa);
        }

        private void MCaixa_CaixaFechado()
        {
            CaixaFechado = true;
            Close();
        }
    }
}
