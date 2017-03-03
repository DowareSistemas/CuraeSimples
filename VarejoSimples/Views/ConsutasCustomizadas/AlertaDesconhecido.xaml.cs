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

namespace VarejoSimples.Views.ConsutasCustomizadas
{
    /// <summary>
    /// Lógica interna para AlertaDesconhecido.xaml
    /// </summary>
    public partial class AlertaDesconhecido : Window
    {
        public bool Permitido { get; set; }
        public AlertaDesconhecido(string nome, string origem, DateTime data_criacao)
        {
            InitializeComponent();

            Permitido = false;
            lbNome.Content = nome;
            lbOrigem.Content = origem;
            lbData_cricao.Content = data_criacao.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void btExecutar_Click(object sender, RoutedEventArgs e)
        {
            Permitido = true;
            Close();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
