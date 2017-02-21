using Base.Controller_Reports;
using System;
using System.Collections.Generic;
using System.Data;
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
using VarejoSimples.DataSets;
using VarejoSimples.Model;
using VarejoSimples.Views.Reports;

namespace VarejoSimples.Views.Consultas
{
    /// <summary>
    /// Lógica interna para SaldosFisicosFinan.xaml
    /// </summary>
    public partial class SaldosFisicosFinan : Window
    {
        private EstoqueController controller;
        public SaldosFisicosFinan()
        {
            InitializeComponent();

            controller = new EstoqueController();
            dataGrid.AplicarPadroes();
            Pesquisar();
        }

        private void Pesquisar()
        {
            List<Estoque> list = controller.Search(txPesquisa.Text);
            if (list == null)
                return;

            List<EstoqueAdapter> listEA = new List<EstoqueAdapter>();
            list.ForEach(e => listEA.Add(new EstoqueAdapter(e, (e.Produtos.Valor_unit * e.Quant))));
            dataGrid.ItemsSource = listEA;
        }

        private void txPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Pesquisar();
        }

        private void btRelatorio_Click(object sender, RoutedEventArgs e)
        {
            List<Estoque> list = controller.Search(txPesquisa.Text);
            if (list == null)
                return;

            DataTable dtEstoque = new DsSaldosFisicosFinanceiros().Tables["Estoque"];
            list.ForEach(es => dtEstoque.Rows.Add(
                es.Produtos.Descricao,
                es.Produtos.Valor_unit,
                es.Quant,
                (es.Produtos.Valor_unit * es.Quant),
                es.Data_entrada,
                es.Data_entrada,
                es.Loja_id));

            IControllerReport cr = ReportController.GetInstance();
            cr.AddDataSource("Estoque", dtEstoque);
            cr.AddDataSource("Usuario", new List<Usuarios> { UsuariosController.UsuarioAtual });
            cr.AddDataSource("Loja", new List<Lojas> { UsuariosController.LojaAtual });

            ReportViewWindow rvw = new ReportViewWindow("Saldos de estoque", cr.GetReportDocument("SLD001"));
        }
    }

    public class EstoqueAdapter
    {
        public EstoqueAdapter(Estoque e, decimal valor_saldo)
        {
            Estoque = e;
            Valor_saldo = valor_saldo;
        }

        public Estoque Estoque { get; set; }
        public decimal Valor_saldo { get; set; }
    }
}
