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
    /// Lógica interna para PesquisarUsuario.xaml
    /// </summary>
    public partial class PesquisarUsuario : Window
    {
        public Usuarios Selecionado = new Usuarios();

        public PesquisarUsuario()
        {
            InitializeComponent();
            dataGrid.AplicarPadroes();
            Pesquisar();
        }

        private void Pesquisar()
        {
            List<Usuarios> list = new UsuariosController().Search(txPesquisa.Text);
            dataGrid.ItemsSource = list;
        }

        private void btSelecionar_Click(object sender, RoutedEventArgs e)
        {
            Selecionar();
        }

        private void Selecionar()
        {
            Usuarios usuario = (Usuarios)dataGrid.SelectedItem;
            if (usuario == null)
                return;
            if (usuario.Id == 0)
                return;

            Selecionado = usuario;
            Close();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Pesquisar();
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Selecionar();
        }
    }
}
