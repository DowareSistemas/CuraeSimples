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
using VarejoSimples.Views.Loja;
using VarejoSimples.Views.Usuario;

namespace VarejoSimples.Views.Vendedor
{
    /// <summary>
    /// Lógica interna para CadVendedor.xaml
    /// </summary>
    public partial class CadVendedor : Window
    {
        public CadVendedor()
        {
            InitializeComponent();
            txComissao.ToMoney();
            txNumero.ToNumeric();
            txNome.Focus();
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            VendedoresController vc = new VendedoresController();
            Vendedores vend = vc.Next(int.Parse(txCod.Text));
            FillVendedor(vend);
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            if((int.Parse(txCod.Text) - 1) <= 0)
            {
                LimparCampos();
                return;
            }

            VendedoresController vc = new VendedoresController();
            Vendedores vend = vc.Prev(int.Parse(txCod.Text));
            FillVendedor(vend);
        }

        private void btNovo_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            Salvar();   
        }

        private void FillVendedor(Vendedores v)
        {
            if (v == null)
                return;

            txCod.Text = v.Id.ToString();
            txNome.Text = v.Nome;
            txApelido.Text = v.Apelido;
            txCelular1.Text = v.Celular1;
            txCelular2.Text = v.Celular2;
            txLogradouro.Text = v.Logradouro;
            txBairro.Text = v.Bairro;
            txMunicipio.Text = v.Municipio;
            txUF.Text = v.Uf;
            txNumero.Text = v.Numero.ToString();
            txComissao.Text = v.Comissao.ToString("N2");
            txCod_usuario.Text = v.Usuario_id.ToString();
            txNome_usuario.Text = v.Usuarios.Nome;
            txCod_loja.Text = v.Loja_id.ToString();
            txNome_loja.Text = v.Lojas.Razao_social;
            ckInativo.IsChecked = v.Inativo;
            txNome.Focus();
        }

        private void Salvar()
        {
            VendedoresController vc = new VendedoresController();
            Vendedores v = (int.Parse(txCod.Text) == 0
                ? new Vendedores()
                : vc.Find(int.Parse(txCod.Text)));

            v.Id = int.Parse(txCod.Text);
            v.Nome = txNome.Text;
            v.Apelido = txApelido.Text;
            v.Celular1 = txCelular1.Text;
            v.Celular2 = txCelular2.Text;
            v.Logradouro = txLogradouro.Text;
            v.Bairro = txBairro.Text;
            v.Municipio = txMunicipio.Text;
            v.Uf = txUF.Text;
            v.Numero = (string.IsNullOrWhiteSpace(txNumero.Text)
                ? 0
                : int.Parse(txNumero.Text));
            v.Comissao = (string.IsNullOrEmpty(txComissao.Text)
                ? 0
                : decimal.Parse(txComissao.Text));
            v.Usuario_id = int.Parse(txCod_usuario.Text);
            v.Loja_id = int.Parse(txCod_loja.Text);
            v.Inativo = ckInativo.IsChecked.Value;

            if (vc.Save(v))
                LimparCampos();
        }

        private void LimparCampos()
        {
            txCod.Text = "0";
            txNome.Text = string.Empty;
            txApelido.Text = string.Empty;
            txCelular1.Text = string.Empty;
            txCelular2.Text = string.Empty;
            txLogradouro.Text = string.Empty;
            txBairro.Text = string.Empty;
            txUF.Text = string.Empty;
            txNumero.Text = "0";
            txMunicipio.Text = string.Empty;
            txComissao.Text = "0";
            txCod_usuario.Text = "0";
            txNome_usuario.Text = string.Empty;
            ckInativo.IsChecked = false;
            txNome.Focus();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btSelecionarLoja_Click(object sender, RoutedEventArgs e)
        {
            PesquisarLoja pl = new PesquisarLoja();
            pl.ShowDialog();

            txCod_loja.Text = pl.Selecionado.Id.ToString();
            txNome_loja.Text = (pl.Selecionado.Id == 0
                ? "Não selecionado"
                : pl.Selecionado.Razao_social);
        }

        private void btSelecionarUsuario_Click(object sender, RoutedEventArgs e)
        {
            PesquisarUsuario pu = new PesquisarUsuario();
            pu.ShowDialog();

            txCod_usuario.Text = pu.Selecionado.Id.ToString();
            txNome_usuario.Text = (pu.Selecionado.Id == 0
                ? "Não selecionado"
                : pu.Selecionado.Nome);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F3)
            {
                PesquisarVendedor pv = new PesquisarVendedor();
                pv.ShowDialog();

                if (pv.Selecionado.Id > 0)
                    FillVendedor(new VendedoresController().Find(pv.Selecionado.Id));
            }
        }

        private void btRelatorio_Click(object sender, RoutedEventArgs e)
        {
            ParametrosRelatorio pr = new ParametrosRelatorio();
            pr.ShowDialog();
        }
    }
}
