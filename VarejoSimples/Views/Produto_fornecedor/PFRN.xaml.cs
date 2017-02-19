using Base.Controller_Reports;
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
using VarejoSimples.Views.Fornecedor;
using VarejoSimples.Views.Produto;
using VarejoSimples.Views.Reports;

namespace VarejoSimples.Views.Produto_fornecedor
{
    /// <summary>
    /// Lógica interna para PFRN.xaml
    /// </summary>
    public partial class PFRN : Window
    {
        public PFRN()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbModelos.Focus();

            IControllerReport rc = ReportController.GetInstance();

            List<KeyValuePair<string, string>> reports = new List<KeyValuePair<string, string>>();
            rc.ReportFiles("PFRN").ForEach(rf => reports.Add(new KeyValuePair<string, string>(rf.LogicalName, rf.Alias)));

            cbModelos.ItemsSource = reports;
            cbModelos.SelectedValuePath = "Key";
            cbModelos.DisplayMemberPath = "Value";

            GridFornecedor.IsEnabled = false;
            GridProduto.IsEnabled = false;
        }

        private void btSelecionarFornecedor_Click(object sender, RoutedEventArgs e)
        {
            PesquisarFornecedor pf = new PesquisarFornecedor();
            pf.ShowDialog();

            txCod_forn.Text = pf.Selecionado.Id.ToString();
            txFornecedor.Text = (pf.Selecionado.Id == 0
                ? "Não selecionado"
                : pf.Selecionado.Nome);
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void PFRN001()
        {
            IControllerReport cr = ReportController.GetInstance();

            int forn_id = int.Parse(txCod_forn.Text);
            Produtos_fornecedoresController pfc = new Produtos_fornecedoresController();
            List<Produtos_fornecedores> pfs = pfc.Get(p => p.Fornecedor_id == forn_id);

            if (pfs.Count == 0)
            {
                MessageBox.Show("Não existem registros para exibir no relatorio", "ARQVAZIO", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            HashSet<Fornecedores> fornecedores = new HashSet<Fornecedores>();
            pfs.ForEach(p => fornecedores.Add(p.Fornecedores));

            List<Produtos> produtos = new List<Produtos>();
            pfs.ForEach(p => produtos.Add(p.Produtos));

            HashSet<Marcas> marcas = new HashSet<Marcas>();
            foreach (Produtos p in produtos)
            {
                if (marcas.FirstOrDefault(e => e.Id == p.Marca_id) == null)
                    marcas.Add(new MarcasController().Find(p.Marca_id));
            }

            HashSet<Fabricantes> fabricantes = new HashSet<Fabricantes>();
            foreach (Produtos p in produtos)
            {
                if (fabricantes.FirstOrDefault(e => e.Id == p.Fabricante_id) == null)
                    fabricantes.Add(new FabricantesController().Find(p.Fabricante_id));
            }

            HashSet<Unidades> unidades = new HashSet<Unidades>();
            foreach (Produtos_fornecedores pf in pfs)
                unidades.Add(pf.Unidades);

            cr.AddDataSource("Marcas", marcas);
            cr.AddDataSource("Lojas", new List<Lojas>() { UsuariosController.LojaAtual });
            cr.AddDataSource("Produtos", produtos);
            cr.AddDataSource("Unidades", unidades);
            cr.AddDataSource("Fornecedores", fornecedores);
            cr.AddDataSource("Fabricantes", fabricantes);
            cr.AddDataSource("Produtos_fornecedores", pfs);
            cr.AddDataSource("Usuarios", new List<Usuarios> { UsuariosController.UsuarioAtual });

            ReportViewWindow rvw = new ReportViewWindow("Relatório de Produtos X Fornecedores", cr.GetReportDocument(cbModelos.SelectedValue.ToString()));
        }

        private void PFRN002()
        {
            IControllerReport cr = ReportController.GetInstance();

            int prod_id = int.Parse(txCod_prod.Text);
            Produtos_fornecedoresController pfc = new Produtos_fornecedoresController();
            List<Produtos_fornecedores> pfs = pfc.Get(p => p.Produto_id == prod_id);

            if (pfs.Count == 0)
            {
                MessageBox.Show("Não existem registros para exibir no relatorio", "ARQVAZIO", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            HashSet<Fornecedores> fornecedores = new HashSet<Fornecedores>();
            pfs.ForEach(p => fornecedores.Add(p.Fornecedores));

            Produtos produto = pfs.First().Produtos;
            Marcas marca = (produto.Marca_id == 0
                ? new Marcas()
                : new MarcasController().Find(produto.Marca_id));

            Fabricantes fabricante = (produto.Fabricante_id == 0
                ? new Fabricantes()
                : new FabricantesController().Find(produto.Fabricante_id));

            HashSet<Unidades> unidades = new HashSet<Unidades>();
            foreach (Produtos_fornecedores pf in pfs)
                if (unidades.FirstOrDefault(e => e.Id == pf.Unidade_id) == null)
                    unidades.Add(pf.Unidades);

            cr.AddDataSource("Marcas", new List<Marcas>() { marca });
            cr.AddDataSource("Lojas", new List<Lojas>() { UsuariosController.LojaAtual });
            cr.AddDataSource("Produtos", new List<Produtos>() { produto });
            cr.AddDataSource("Unidades", unidades);
            cr.AddDataSource("Fornecedores", fornecedores);
            cr.AddDataSource("Fabricantes", new List<Fabricantes>() { fabricante});
            cr.AddDataSource("Produtos_fornecedores", pfs);
            cr.AddDataSource("Usuarios", new List<Usuarios> { UsuariosController.UsuarioAtual });

            ReportViewWindow rvw = new ReportViewWindow("Relatório de Produtos X Fornecedores", cr.GetReportDocument(cbModelos.SelectedValue.ToString()));

        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (cbModelos.SelectedValue == null)
            {
                MessageBox.Show("Selecione o modelo do relatório", "Ateção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            switch (cbModelos.SelectedValue.ToString())
            {
                case "PFRN001":
                    PFRN001();
                    break;

                case "PFRN002":
                    PFRN002();
                    break;
            }
        }

        private void btSelecionarProdutos_Click(object sender, RoutedEventArgs e)
        {
            PesquisarProduto pp = new PesquisarProduto();
            pp.ShowDialog();

            if (pp.Selecionado.Id == 0)
                return;

            txCod_prod.Text = pp.Selecionado.Id.ToString();
            txProduto.Text = (pp.Selecionado.Id == 0
                ? "Não selecionado"
                : pp.Selecionado.Descricao);
        }

        private void cbModelos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbModelos == null)
                return;

            if (cbModelos.SelectedValue == null)
                return;

            if (cbModelos.SelectedValue.Equals("PFRN001"))
            {
                GridFornecedor.IsEnabled = true;
                GridProduto.IsEnabled = false;
            }

            if (cbModelos.SelectedValue.Equals("PFRN002"))
            {
                GridFornecedor.IsEnabled = false;
                GridProduto.IsEnabled = true;
            }
        }
    }
}
