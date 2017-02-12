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

namespace VarejoSimples.Views.Fornecedor
{
    /// <summary>
    /// Lógica interna para CadFornecedor.xaml
    /// </summary>
    public partial class CadFornecedor : Window
    {
        private FornecedoresController controller;
        public CadFornecedor()
        {
            InitializeComponent();
            controller = new FornecedoresController();
            txCod.ToNumeric();
            txNumero.ToNumeric();
            txNome.Focus();
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            Fornecedores f = controller.Next(int.Parse(txCod.Text));
            FillFrn(f);
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCod.Text);
            if ((id - 1) <= 0)
            {
                LimparCampos();
                return;
            }

            Fornecedores f = controller.Prev(int.Parse(txCod.Text));
            FillFrn(f);
        }

        private void Salvar()
        {
            Fornecedores f = (int.Parse(txCod.Text) == 0
                ? new Fornecedores()
                : controller.Find(int.Parse(txCod.Text)));

            f.Id = int.Parse(txCod.Text);
            f.Nome = txNome.Text;
            f.Cnpj = txCnpj.Text;
            f.Logradouro = txLogradouro.Text;
            f.Bairro = txBairro.Text;
            f.Municipio = txMunicipio.Text;
            f.Numero = string.IsNullOrWhiteSpace(txNumero.Text) ? 0 : int.Parse(txNumero.Text);
            f.Cep = txCep.Text;
            f.Uf = txUf.Text.ToUpper();
            f.Telefone = txTel.Text;
            f.Email = txEmail.Text;

            if (controller.Save(f))
                LimparCampos();
        }

        private void LimparCampos()
        {
            txCod.Text = "0";
            txNome.Text = string.Empty;
            txCnpj.Text = string.Empty;
            txLogradouro.Text = string.Empty;
            txBairro.Text = string.Empty;
            txMunicipio.Text = string.Empty;
            txNumero.Text = "0";
            txCep.Text = string.Empty;
            txUf.Text = string.Empty;
            txTel.Text = string.Empty;
            txEmail.Text = string.Empty;

            txNome.Focus();
        }

        private void FillFrn(Fornecedores f)
        {
            if (f == null)
                return;

            txCod.Text = f.Id.ToString();
            txNome.Text = f.Nome;
            txCnpj.Text = f.Cnpj;
            txLogradouro.Text = f.Logradouro;
            txBairro.Text = f.Bairro;
            txMunicipio.Text = f.Municipio;
            txNumero.Text = f.Numero.ToString();
            txCep.Text = f.Cep;
            txUf.Text = f.Uf;
            txTel.Text = f.Telefone;
            txEmail.Text = f.Email;

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

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCod.Text);
            if (id == 0)
                return;
            if (controller.Remove(id))
                LimparCampos();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F3)
            {
                PesquisarFornecedor pf = new PesquisarFornecedor();
                pf.ShowDialog();

                FillFrn(pf.Selecionado);
            }
        }
    }
}
