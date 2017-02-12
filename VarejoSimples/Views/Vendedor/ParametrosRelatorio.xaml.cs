using Base.Controller_Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
using VarejoSimples.Views.Reports;

namespace VarejoSimples.Views.Vendedor
{
    /// <summary>
    /// Lógica interna para ParametrosRelatorio.xaml
    /// </summary>
    public partial class ParametrosRelatorio : Window
    {
        public ParametrosRelatorio()
        {
            InitializeComponent();

            IControllerReport cr = ReportController.GetInstance();
            List<KeyValuePair<string, string>> modelos = new List<KeyValuePair<string, string>>();

            cr.ReportFiles("VND").ForEach(e => modelos.Add(new KeyValuePair<string, string>(e.FisicalName, e.Alias)));

            cbModelo.DisplayMemberPath = "Value";
            cbModelo.SelectedValuePath = "Key";
            cbModelo.ItemsSource = modelos;

            txCod_inicio.ToNumeric();
            txCod_fim.ToNumeric();
            txComissao_inicio.ToMoney();
            txComissao_fim.ToMoney();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            int cod_inicio = int.Parse(txCod_inicio.Text);
            int cod_fim = int.Parse(txCod_fim.Text);
            decimal comissao_inicio = decimal.Parse(txComissao_inicio.Text);
            decimal comissao_fim = decimal.Parse(txComissao_fim.Text);
            int loja = int.Parse(txLoja.Text);

            Expression<Func<Vendedores, bool>> query =
                (v =>
                    (v.Id >= cod_inicio && v.Id <= cod_fim) &&
                    (v.Comissao >= comissao_inicio && v.Comissao <= comissao_fim));

            if (loja > 0)
                query = query.And(v => v.Loja_id == loja);
            
            VendedoresController vc = new VendedoresController();
            List<Vendedores> listVendedores = vc.Get(query);
            IControllerReport cr = ReportController.GetInstance();
            cr.AddDataSource("Vendedores", listVendedores);
            cr.AddDataSource("Usuario", new List<Usuarios>() { UsuariosController.UsuarioAtual });
            cr.AddDataSource("Loja", new List<Lojas>() { UsuariosController.LojaAtual });

           new ReportViewWindow("Relatório de clientes - Doware Curae Varejo (Simples)", cr.GetReportDocument(cbModelo.SelectedValue.ToString()));
        }
    }
}
