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

namespace VarejoSimples.Views.Usuario
{
    /// <summary>
    /// Lógica interna para AlterarSenha.xaml
    /// </summary>
    public partial class AlterarSenha : Window
    {
        private Usuarios Usuario { get; set; }
        public AlterarSenha(Usuarios usuario)
        {
            InitializeComponent();

            Usuario = usuario;
            txNovaSenha.Focus();
        }

        private void txNovaSenha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                AlteraSenha();
        }

        private void AlteraSenha()
        {
            if (string.IsNullOrWhiteSpace(txNovaSenha.Password))
            {
                MessageBox.Show("Senha inválida", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }


            UsuariosController uc = new UsuariosController();
            Usuario = uc.Find(Usuario.Id);
            Usuario.Senha = txNovaSenha.Password;
            Usuario.Alteracao_pendente = false;

            if (uc.Save(Usuario))
            {
                MessageBox.Show("Senha atualizada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            else
            {
                MessageBox.Show("Ocorreu um problema ao atualizar a senha. Acione o suporte Doware.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            AlteraSenha();
        }
    }
}
