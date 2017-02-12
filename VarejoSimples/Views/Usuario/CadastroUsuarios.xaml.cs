using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    /// Lógica interna para CadastroUsuarios.xaml
    /// </summary>
    public partial class CadastroUsuarios : Window
    {
        public CadastroUsuarios()
        {
            InitializeComponent();
            txNome.Focus();
        }

        private void Load(Usuarios u)
        {
            if (u == null)
                return;

            txCod.Text = u.Id.ToString();
            txNome.Text = u.Nome;
            txSenha.Password = u.Senha;
            ckInativo.IsChecked = u.Inativo;
            ckAdmin.IsChecked = u.Admin;
        }

        private void Salvar()
        {
            UsuariosController uc = new UsuariosController();

            Usuarios usuario = int.Parse(txCod.Text) == 0 ? new Usuarios() : uc.Find(int.Parse(txCod.Text));
            usuario.Id = int.Parse(txCod.Text);
            usuario.Nome = txNome.Text;
            usuario.Senha = txSenha.Password;
            usuario.Inativo = ckInativo.IsChecked.Value;
            usuario.Admin = ckAdmin.IsChecked.Value;
            if (uc.Save(usuario))
                LimparCampos();
        }

        private void LimparCampos()
        {
            txCod.Text = "0";
            txNome.Text = string.Empty;
            txSenha.Password = string.Empty;
            ckAdmin.IsChecked = false;
            ckInativo.IsChecked = false;
            txNome.Focus(); ;
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            Salvar();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCod.Text);
            Usuarios u = new UsuariosController().Get(us => us.Id > id).FirstOrDefault();
            Load(u);
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCod.Text);
            if((id-1) <= 0)
            {
                LimparCampos();
                return;
            }
            Usuarios u = new UsuariosController().Get(us => us.Id < id).OrderByDescending(us => us.Id).FirstOrDefault();
            Load(u);
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCod.Text);
            if (id == 0)
                return;

            if (new UsuariosController().Delete(id))
                LimparCampos();
        }

        private void btNovo_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }
    }
}
