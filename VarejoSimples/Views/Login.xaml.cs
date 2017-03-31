using DarumaFramework_NFCe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using VarejoSimples.Views.Loja;

namespace VarejoSimples.Views
{
    /// <summary>
    /// Lógica interna para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public delegate void LoginEfetuado();
        public event LoginEfetuado EfetuouLogin;

        public Login()
        {
            InitializeComponent();
        }

        public void Start(Iniciando ini)
        {
            LojasController lc = new LojasController();
            Lojas loja = null;


            try
            {
                this.Visibility = Visibility.Hidden;

                loja = lc.Search("").FirstOrDefault();

                if (loja == null)
                {
                    CadLoja cadatroLoja = new CadLoja();
                    cadatroLoja.ShowDialog();
                    loja = lc.Search("").FirstOrDefault();
                }

                txCod_loja.Text = loja.Id.ToString();
                txNome_loja.Text = loja.Razao_social;
                txUsuario.Focus();

                this.Visibility = Visibility.Visible;
                ini.EnabledClose = true;
                ini.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Não foi possível conectar-se com o servidor.
Verifique a configuração de rede do computador.
Verifique se o cabo de rede ou Wi-Fi está conectado e
tente novamente.

Caso o problema persista, acione o suporte Doware.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

            feito = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void btEntrar_Click(object sender, RoutedEventArgs e)
        {
            EfetuaLogin();
        }

        private void EfetuaLogin()
        {
            UsuariosController uc = new UsuariosController();
            if (uc.EfetuaLogin(txUsuario.Text, txSenha.Password, txCod_loja.Text))
            {
                if (EfetuouLogin != null) EfetuouLogin();
                Hide();
            }
            else
                MessageBox.Show("Usuário ou senha inválidos!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void btSelecionarLoja_Click(object sender, RoutedEventArgs e)
        {
            PesquisarLoja pl = new PesquisarLoja();
            pl.ShowDialog();

            if (pl.Selecionado.Id == 0)
                return;

            txCod_loja.Text = pl.Selecionado.Id.ToString();
            txNome_loja.Text = (pl.Selecionado.Razao_social);
        }

        private void txUsuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                txSenha.Focus();
        }

        private void txSenha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                EfetuaLogin();
        }

        private void btSair_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private bool feito = false;
        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (feito)
                return;

        }
    }
}
