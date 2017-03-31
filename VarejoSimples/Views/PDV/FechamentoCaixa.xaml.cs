using Base.Controller_Reports;
using CrystalDecisions.CrystalReports.Engine;
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

namespace VarejoSimples.Views.PDV
{
    /// <summary>
    /// Interaction logic for FechamentoCaixa.xaml
    /// </summary>
    public partial class FechamentoCaixa : Window
    {
        private Movimentos_caixasController controller = null;
        private List<ItemFpgFechamentoCaixa> Itens_pagamento { get; set; }

        public bool CaixaFechado { get; set; }

        public FechamentoCaixa()
        {
            InitializeComponent();

            Itens_pagamento = new List<ItemFpgFechamentoCaixa>();
            controller = new Movimentos_caixasController();
            CarregarFormasPag();

            CaixaFechado = false;
        }

        private void CarregarFormasPag()
        {
            List<KeyValuePair<Formas_pagamento, decimal>> list = controller.GetTotaisPorFormaPagamentoCaixaAtual();
            if (list.Count > 0)
                list.ForEach(e => Itens_pagamento.Add(new ItemFpgFechamentoCaixa(e.Key, e.Value)));

            if (Itens_pagamento.Count > 0)
            {
                Itens_pagamento.ForEach(item => sp_formas_pag.Children.Add(item));
                Itens_pagamento.First().txValor.Focus();
                Itens_pagamento.First().txValor.SelectAll();
            }
        }

        private void btFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            Confirmar();
        }

        private void Confirmar()
        {
            DataTable dataTable = new DsFechamentoCaixa().Tables[0];

            IControllerReport rController = ReportController.GetInstance();
            rController.AddDataSource("Itens_pagamento", dataTable);
            rController.BindParameter("NomeFantasia", UsuariosController.LojaAtual.Nome_fantasia);
            rController.BindParameter("RazaoSocial", UsuariosController.LojaAtual.Razao_social);
            rController.BindParameter("Usuario", UsuariosController.UsuarioAtual.Nome);
            rController.BindParameter("Caixa", new CaixasController().Find(controller.Get_ID_CaixaAtualUsuario()).Nome);
            rController.BindParameter("ValorAbertura", controller.GetUltimoMovimentoAbertura().Valor);
            rController.BindParameter("TotalEntradas", controller.GetTotalMovimentacoesCaixaAtual(Tipo_movimentacao_caixa.ENTRADA));
            rController.BindParameter("TotalSaidas", controller.GetTotalMovimentacoesCaixaAtual(Tipo_movimentacao_caixa.SAIDA));
            rController.BindParameter("TotalTroco", controller.GetTotalMovimentacoesCaixaAtual(Tipo_movimentacao_caixa.TROCO));

            int usuario_id = UsuariosController.UsuarioAtual.Id;
            foreach (ItemFpgFechamentoCaixa item in Itens_pagamento)
            {
                bool operationResult = controller.MovimentarCaixa(
                     Tipo_movimentacao_caixa.FECHAMENTO,
                     item.Valor_informado,
                     item.Forma_pagamento.Id,
                     usuario_id,
                     0,
                     $"FECHAMENTO DO CAIXA ({item.Forma_pagamento.Descricao})");

                dataTable.Rows.Add(item.Forma_pagamento.Descricao, item.Valor_informado);
                if (item.Forma_pagamento.Tipo_pagamento == (int)Tipo_pagamento.DINHEIRO)
                    EfetuarTransferenciaConta();
            }

            CaixaFechado = true;
            ReportDocument rpt = rController.GetReportDocument("CXACONS002");
            rpt.PrintToPrinter(1, false, 1, 1);

            Close();
        }

        private void EfetuarTransferenciaConta()
        {

        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
                Confirmar();
        }
    }
}
