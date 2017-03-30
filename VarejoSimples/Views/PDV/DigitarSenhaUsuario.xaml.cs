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
using VarejoSimples.Model;

namespace VarejoSimples.Views.PDV
{
    /// <summary>
    /// Interaction logic for DigitarSenhaUsuario.xaml
    /// </summary>
    public partial class DigitarSenhaUsuario : Window
    {
        private Usuarios Usuario { get; set; }

        public bool Autenticado { get; set; }
        public bool Cancelado { get; set; }

        public DigitarSenhaUsuario(Usuarios usuario)
        {
            InitializeComponent();

            Usuario = usuario;
            Autenticado = false;
            Cancelado = true;
            lbNomeUsuario.Content = "Senha para usuário: " + usuario.Nome;
            txSenha.Focus();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                Confirmar();

            if (e.Key == Key.Escape)
                Close();
        }

        private void Confirmar()
        {
            Autenticado = (txSenha.Password.Equals(Usuario.Senha));

            if(!Autenticado)
            {
                MessageBox.Show("Senha incorreta", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                txSenha.SelectAll();
                return;
            }

            Cancelado = false;
            Close();
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            Confirmar();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txSenha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Confirmar();
        }
    }
}
