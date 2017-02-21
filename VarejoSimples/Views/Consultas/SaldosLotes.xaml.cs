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
    /// Lógica interna para SaldosLotes.xaml
    /// </summary>
    public partial class SaldosLotes : Window
    {
        private EstoqueController controller;
        public SaldosLotes()
        {
            InitializeComponent();

            controller = new EstoqueController();
            dataGrid.AplicarPadroes();
            Pesquisar();
        }

        private void Pesquisar()
        {
            List<Estoque> list = controller.Search(txPesquisa.Text, true);
            if (list == null)
                return;

            List<EstoqueAdapter> lEA = new List<EstoqueAdapter>();
            list.ForEach(e => lEA.Add(new EstoqueAdapter(e, (e.Produtos.Valor_unit * e.Quant))));

            dataGrid.ItemsSource = lEA;
        }

        private void txPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Pesquisar();
        }

        private void btRelatorio_Click(object sender, RoutedEventArgs e)
        {
            List<Estoque> list = controller.Search(txPesquisa.Text, true);
            if (list == null)
                return;

            DataTable dtEstoque = new DsSaldosLotes().Tables["Estoque"];
            list.ForEach(es => dtEstoque.Rows.Add(
                  es.Produtos.Descricao,
                  es.Produtos.Valor_unit,
                  es.Quant,
                  (es.Produtos.Valor_unit * es.Quant),
                  es.Data_entrada,
                  es.Data_entrada,
                  es.Lote,
                  es.Sublote,
                  es.Loja_id
                ));

            DataTable dtLote = new DsSaldosLotes().Tables["Lotes"];
            HashSet<string> lotes = new HashSet<string>();
            list.ForEach(es => lotes.Add(es.Lote));

            foreach (string str in lotes)
                dtLote.Rows.Add(str);

            IControllerReport cr = ReportController.GetInstance();
            cr.AddDataSource("Estoque", dtEstoque);
            cr.AddDataSource("Lotes", dtLote);
            cr.AddDataSource("Loja", new List<Lojas>() { UsuariosController.LojaAtual });
            cr.AddDataSource("Usuario", new List<Usuarios>() { UsuariosController.UsuarioAtual });

            ReportViewWindow rv = new ReportViewWindow("Saldos por lote", cr.GetReportDocument("SLD002"));
        }

    }
}
