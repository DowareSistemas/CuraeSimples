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
using VarejoSimples.Enums;
using VarejoSimples.Model;
using VarejoSimples.Views.Reports;

namespace VarejoSimples.Views.Consultas
{
    /// <summary>
    /// Lógica interna para ProdutosVencendo.xaml
    /// </summary>
    public partial class ProdutosVencendo : Window
    {
        public ProdutosVencendo()
        {
            InitializeComponent();

            List<KeyValuePair<Tipo_produto_filtro_validade, string>> tipos = new List<KeyValuePair<Tipo_produto_filtro_validade, string>>();
            tipos.Add(new KeyValuePair<Tipo_produto_filtro_validade, string>(Tipo_produto_filtro_validade.TODOS, "TODOS"));
            tipos.Add(new KeyValuePair<Tipo_produto_filtro_validade, string>(Tipo_produto_filtro_validade.APENAS_COM_LOTE, "APENAS COM LOTE"));
            tipos.Add(new KeyValuePair<Tipo_produto_filtro_validade, string>(Tipo_produto_filtro_validade.APENAS_SEM_LOTE, "APENAS SEM LOTE"));

            cbTipo.ItemsSource = tipos;
            cbTipo.DisplayMemberPath = "Value";
            cbTipo.SelectedValuePath = "Key";

            cbTipo.SelectedIndex = 0;

            txDias.ToNumeric();
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            Tipo_produto_filtro_validade tipo = (Tipo_produto_filtro_validade)cbTipo.SelectedValue;
            EstoqueController ec = new EstoqueController();
            List<Estoque> list = ec.ProdutosVencendo(int.Parse(txDias.Text), tipo);

            DataTable dtProds = new DsProdutosVencendoValidade().Tables["Produtos"];
            list.ForEach(es => dtProds.Rows.Add(
                   es.Produtos.Referencia,
                   es.Produtos.Ean,
                   (string.IsNullOrEmpty(es.Lote) ? "" : (es.Lote + "SL" + es.Sublote)),
                   es.Produtos.Descricao,
                   es.Quant,
                   es.Data_validade,
                   es.Produtos.Localizacao
                ));

            IControllerReport cr = ReportController.GetInstance();
            cr.AddDataSource("Produtos", dtProds);
            cr.AddDataSource("Loja", new List<Lojas>() { UsuariosController.LojaAtual });
            cr.AddDataSource("Usuario", new List<Usuarios>() { UsuariosController.UsuarioAtual });

            ReportViewWindow rv = new ReportViewWindow("Produtos vencidos e vencendo", cr.GetReportDocument("SLD003"));
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
