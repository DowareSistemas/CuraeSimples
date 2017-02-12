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

namespace VarejoSimples.Views.Cliente
{
    /// <summary>
    /// Lógica interna para CadCliente.xaml
    /// </summary>
    public partial class CadCliente : Window
    {
        private ClientesController controller;
        public CadCliente()
        {
            InitializeComponent();
            controller = new ClientesController();
            txCod.ToNumeric();
            txNumero.ToNumeric();
            txNome.Focus();
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            Clientes c = controller.Next(int.Parse(txCod.Text));
            FillCli(c);
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCod.Text);
            if ((id - 1) <= 0)
            {
                LimparCampos();
                return;
            }

            Clientes c = controller.Prev(id);
            FillCli(c);
        }

        private void Salvar()
        {
            Clientes c = (int.Parse(txCod.Text) == 0
                ? new Clientes()
                : controller.Find(int.Parse(txCod.Text)));

            c.Id = int.Parse(txCod.Text);
            c.Nome = txNome.Text;
            c.Cpf = txCpf.Text;
            c.Email = txEmail.Text;
            c.Telefone = txTelefone.Text;
            c.Celular = txCelular.Text;
            c.Logradouro = txLogradouro.Text;
            c.Numero = (string.IsNullOrWhiteSpace(txNumero.Text) ? 0 : int.Parse(txNumero.Text));
            c.Bairro = txBairro.Text;
            c.Municipio = txMunicipio.Text;
            c.Uf = txUf.Text;
            c.Cep = txCep.Text;

            if (controller.Save(c))
                LimparCampos();
        }

        private void LimparCampos()
        {
            txCod.Text = "0";
            txNome.Text = string.Empty;
            txCpf.Text = string.Empty;
            txEmail.Text = string.Empty;
            txTelefone.Text = string.Empty;
            txCelular.Text = string.Empty;
            txLogradouro.Text = string.Empty;
            txBairro.Text = string.Empty;
            txNumero.Text = "0";
            txMunicipio.Text = string.Empty;
            txCep.Text = string.Empty;
            txUf.Text = string.Empty;

            txNome.Focus();

        }

        private void FillCli(Clientes c)
        {
            if (c == null)
                return;

            txCod.Text = c.Id.ToString();
            txNome.Text = c.Nome;
            txCpf.Text = c.Cpf;
            txEmail.Text = c.Email;
            txTelefone.Text = c.Telefone;
            txCelular.Text = c.Celular;
            txLogradouro.Text = c.Logradouro;
            txBairro.Text = c.Bairro;
            txNumero.Text = c.Numero.ToString();
            txMunicipio.Text = c.Municipio;
            txCep.Text = c.Cep;
            txUf.Text = c.Uf;

            txNome.Focus();
        }

        private void btNovo_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            Salvar();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCod.Text);
            if (id == 0)
                return;

            if (controller.Remove(id))
                LimparCampos();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F3)
            {
                PesquisarCliente pc = new PesquisarCliente();
                pc.ShowDialog();

                FillCli(pc.Selecionado);
            }
        }
    }
}
