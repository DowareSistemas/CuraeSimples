using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VarejoSimples.Controller;
using VarejoSimples.Model;
using VarejoSimples.Views.Fabricante;
using VarejoSimples.Views.Marca;
using VarejoSimples.Views.Unidade;

namespace VarejoSimples.Views.Produto
{
    /// <summary>
    /// Lógica interna para CadProduto.xaml
    /// </summary>
    public partial class CadProduto : Window
    {
        private ProdutosController controller;
        private string Path_arquivo_foto { get; set; }
        public CadProduto()
        {
            InitializeComponent();

            txCod.ToNumeric();
            txValor.ToMoney();
            txCod_unidade.ToNumeric();
            txCod_fabricante.ToNumeric();
            txCod_marca.ToNumeric();
            txEan.ToNumeric();
            txNcm.ToNumeric();
            txAliquota.ToMoney();
            this.controller = new ProdutosController();
            txDescricao.Focus();
            
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            controller = new ProdutosController();
            Produtos p = this.controller.Next(int.Parse(txCod.Text));
            FillProd(p);
        }

        private void FillProd(Produtos p)
        {
            if (p == null)
                return;

            LimparCampos();
            fotoProduto.Source = null;

            txCod.Text = p.Id.ToString();
            txDescricao.Text = p.Descricao;
            txReferencia.Text = p.Referencia;
            txEan.Text = p.Ean;
            txNcm.Text = p.Ncm;
            txCod_unidade.Text = p.Unidade_id.ToString();
            txNome_unidade.Text = p.Unidades.Nome;
            txValor.Text = p.Valor_unit.ToString("N2");
            ckControla_lote.IsChecked = p.Controla_lote;
            txAliquota.Text = p.Aliquota.ToString("N2");

            Fabricantes fab = new FabricantesController().Find(p.Fabricante_id);
            if (fab != null)
            {
                txCod_fabricante.Text = fab.Id.ToString();
                txNome_fabricante.Text = fab.Nome;
            }

            Marcas mar = new MarcasController().Find(p.Marca_id);
            if (mar != null)
            {
                txCod_marca.Text = mar.Id.ToString();
                txNome_marca.Text = mar.Nome;
            }

            txLocalizacao.Text = p.Localizacao;

            try
            {
                if (p.Foto != null)
                {
                    string name = $@"C:\Temp\Curae\prod{p.Id + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond}.jpg";
                    File.WriteAllBytes(name, p.Foto);
                    fotoProduto.Source = new BitmapImage(new Uri(name));
                }
            }
            catch (Exception ex)
            {
                //BStatus.Alert(ex.Message);
            }

            txDescricao.Focus();
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            controller = new ProdutosController();
            int id = int.Parse(txCod.Text);
            if ((id - 1) <= 0)
            {
                LimparCampos();
                return;
            }

            Produtos p = controller.Prev(id);
            FillProd(p);
        }

        private void btNovo_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            Salvar();
        }

        private void Salvar()
        {
            ProdutosController pc = new ProdutosController();
            Produtos p = (int.Parse(txCod.Text) == 0
                ? new Produtos()
                : pc.Find(int.Parse(txCod.Text)));

          //  p.Id = int.Parse(txCod.Text);
            p.Descricao = txDescricao.Text;
            p.Ean = txEan.Text;
            p.Referencia = txReferencia.Text;
            p.Ncm = txNcm.Text;
            p.Unidade_id = int.Parse(txCod_unidade.Text);
            p.Valor_unit = decimal.Parse(txValor.Text);
            p.Fabricante_id = int.Parse(txCod_fabricante.Text);
            p.Marca_id = int.Parse(txCod_marca.Text);
            p.Localizacao = txLocalizacao.Text;
            p.Controla_lote = ckControla_lote.IsChecked.Value;
            p.Aliquota = decimal.Parse(txAliquota.Text);

            if ((p.Foto == null) && !string.IsNullOrWhiteSpace(Path_arquivo_foto))
                p.Foto = (string.IsNullOrEmpty(Path_arquivo_foto)
                    ? null
                    : File.ReadAllBytes(Path_arquivo_foto));

            if (pc.Save(p))
                LimparCampos();
        }

        private void LimparCampos()
        {
            txCod.Text = "0";
            txDescricao.Text = string.Empty;
            txReferencia.Text = string.Empty;
            txEan.Text = string.Empty;
            txNcm.Text = string.Empty;
            txCod_unidade.Text = "0";
            txNome_unidade.Text = string.Empty;
            txValor.Text = "0,00";
            txCod_fabricante.Text = "0";
            txNome_fabricante.Text = string.Empty;
            txCod_marca.Text = "0";
            txAliquota.Text = "0,00";
            ckControla_lote.IsChecked = false;
            txNome_marca.Text = string.Empty;
            txLocalizacao.Text = string.Empty;
            fotoProduto.Source = null;

            txDescricao.Focus();
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

        private void lbSelecionarImagem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                OpenFileDialog o = new OpenFileDialog();
                o.Filter = "Arquivos de imagem(*.jpg)|*.jpg;";
                o.ShowDialog();
                if (string.IsNullOrEmpty(o.FileName))
                    return;

                Path_arquivo_foto = o.FileName;
                fotoProduto.Source = new BitmapImage(new Uri(o.FileName));
            }
            catch
            {

            }
        }

        private void btSelecionar_unidade_Click(object sender, RoutedEventArgs e)
        {
            PesquisarUnidade pesq = new PesquisarUnidade();
            pesq.ShowDialog();

            txCod_unidade.Text = pesq.Selecionado.Id.ToString();
            txNome_unidade.Text = (pesq.Selecionado.Id == 0
                ? "Não selecionado"
                : pesq.Selecionado.Nome);
        }

        private void btSelecionar_fab_Click(object sender, RoutedEventArgs e)
        {
            PesquisarFabricante pf = new PesquisarFabricante();
            pf.ShowDialog();

            txCod_fabricante.Text = pf.Selecionado.Id.ToString();
            txNome_fabricante.Text = (pf.Selecionado.Id == 0
                ? "Não selecionado"
                : pf.Selecionado.Nome);
        }

        private void btSelecionar_marca_Click(object sender, RoutedEventArgs e)
        {
            PesquisarMarca pm = new PesquisarMarca();
            pm.ShowDialog();

            txCod_marca.Text = pm.Selecionado.Id.ToString();
            txNome_marca.Text = (pm.Selecionado.Id == 0
                ? "Não selecionado"
                : pm.Selecionado.Nome);
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.F3)
            {
                PesquisarProduto pp = new PesquisarProduto();
                pp.ShowDialog();

               if (pp.Selecionado.Id > 0)
                    FillProd(pp.Selecionado);
            }
        }
    }
}
