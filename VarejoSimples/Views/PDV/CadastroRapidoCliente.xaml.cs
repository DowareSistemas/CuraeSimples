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

namespace VarejoSimples.Views.PDV
{
    /// <summary>
    /// Lógica interna para CadastroRapidoCliente.xaml
    /// </summary>
    public partial class CadastroRapidoCliente : Window
    {
        public Clientes ClienteCadastrado = new Clientes();
        private ClientesController controller = null;
        public CadastroRapidoCliente()
        {
            InitializeComponent();

            controller = new ClientesController();
            txNome.Focus();
        }

        private void btFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                Salvar();

            if (e.Key == Key.Escape)
                Close();
        }

        private void Salvar()
        {
            ClienteCadastrado.Nome = txNome.Text;
            ClienteCadastrado.Cpf = txCpf.Text;
            ClienteCadastrado.Telefone = txTelefone.Text;
            ClienteCadastrado.Celular = txCelular.Text;
            ClienteCadastrado.Email = string.Empty;
            ClienteCadastrado.Bairro = string.Empty;
            ClienteCadastrado.Cep = string.Empty;
            ClienteCadastrado.Logradouro = string.Empty;
            ClienteCadastrado.Municipio = string.Empty;
            ClienteCadastrado.Uf = string.Empty;

            if (controller.Save(ClienteCadastrado))
                Close();
        }
    }
}
