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

namespace VarejoSimples.Views.Configuracao
{
    /// <summary>
    /// Lógica interna para NovaRegraParametro.xaml
    /// </summary>
    public partial class NovaRegraParametro : Window
    {
        public NovaRegraParametro(string parametro)
        {
            InitializeComponent();

            txNome.Text = parametro;
        }

        private void btAplicar_Click(object sender, RoutedEventArgs e)
        {
            ParametrosController pc = new ParametrosController();
            if (pc.InsereParametro(txNome.Text, txComputador.Text, txValor.Text))
                Close();
        }
    }
}
