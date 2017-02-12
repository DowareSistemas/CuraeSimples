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

namespace VarejoSimples.Views.Loja
{
    /// <summary>
    /// Lógica interna para CadLoja.xaml
    /// </summary>
    public partial class CadLoja : Window
    {
        public CadLoja()
        {
            InitializeComponent();
            txRazao_s.Focus();
        }

        private void btNovo_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

        private void Salvar()
        {
            LojasController lc = new LojasController();
            Lojas loja = (int.Parse(txCod.Text) == 0
                ? new Lojas()
                : lc.Find(int.Parse(txCod.Text)));

            loja.Id = int.Parse(txCod.Text);
            loja.Razao_social = txRazao_s.Text;
            loja.Nome_fantasia = txNome_f.Text;
            loja.Cnpj = txCnpj.Text;
            loja.Logradouro = txLogradouro.Text;
            loja.Bairro = txBairro.Text;
            loja.Municipio = txMunicipio.Text;

            int numero = 0;
            if (int.TryParse(txNumero.Text, out numero))
                loja.Numero = int.Parse(txNumero.Text);
            else
                loja.Numero = 0;

            loja.Uf = txUF.Text;
            loja.Responsavel = txResponsavel.Text;
            loja.Telefone = txTelefone.Text;
            loja.Celular = txCelular.Text;

            if (lc.Save(loja))
                LimparCampos();
        }

        private void LimparCampos()
        {
            txCod.Text = "0";
            txRazao_s.Text = string.Empty;
            txNome_f.Text = string.Empty;
            txCnpj.Text = string.Empty;
            txLogradouro.Text = string.Empty;
            txBairro.Text = string.Empty;
            txNumero.Text = string.Empty;
            txMunicipio.Text = string.Empty;
            txUF.Text = string.Empty;
            txResponsavel.Text = string.Empty;
            txTelefone.Text = string.Empty;
            txCelular.Text = string.Empty;
            txRazao_s.Focus();
        } 

        private void FillLoja(Lojas loja)
        {
            if (loja == null)
                return;

            if(loja.Id == 0)
            {
                LimparCampos();
                return;
            }

            txCod.Text = loja.Id.ToString();
            txRazao_s.Text = loja.Razao_social;
            txNome_f.Text = loja.Nome_fantasia;
            txCnpj.Text = loja.Cnpj;
            txLogradouro.Text = loja.Logradouro;
            txBairro.Text = loja.Bairro;
            txNumero.Text = loja.Numero.ToString();
            txMunicipio.Text = loja.Municipio;
            txUF.Text = loja.Uf;
            txResponsavel.Text = loja.Responsavel;
            txTelefone.Text = loja.Telefone;
            txCelular.Text = loja.Celular;
            txCod.Focus();
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            if((int.Parse(txCod.Text) - 1) <= 0)
            {
                LimparCampos();
                return;
            }

            LojasController lc = new LojasController();
            Lojas loja = lc.Prev(int.Parse(txCod.Text));
            FillLoja(loja);
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            LojasController lc = new LojasController();
            Lojas loja = lc.Next(int.Parse(txCod.Text));
            FillLoja(loja);
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            Salvar();
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
