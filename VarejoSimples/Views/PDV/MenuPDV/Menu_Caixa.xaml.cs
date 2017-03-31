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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VarejoSimples.Controller;
using VarejoSimples.Enums;
using VarejoSimples.Model;
using VarejoSimples.Tasks;

namespace VarejoSimples.Views.PDV.MenuPDV
{
    /// <summary>
    /// Interação lógica para Menu_Caixa.xam
    /// </summary>
    public partial class Menu_Caixa : UserControl
    {
        Movimentos_caixasController controller = null;

        public delegate void FechamentoCaixaEvt();
        public event FechamentoCaixaEvt CaixaFechado; 

        public Menu_Caixa()
        {
            InitializeComponent();
            controller = new Movimentos_caixasController();

            dataGrid.AplicarPadroes();
            ListarMovimentacoes();
        }

        private void ListarMovimentacoes()
        {
            Menu_CaixaTask task = new Menu_CaixaTask(this);
            task.Execute(0);
        }

        private void btEntrada_Click(object sender, RoutedEventArgs e)
        {
            ShowMovimentacao(Tipo_movimentacao_caixa.ENTRADA);
        }

        private void ShowMovimentacao(Tipo_movimentacao_caixa tipo)
        {
            EntradaValorCaixa evc = new EntradaValorCaixa(tipo);
            evc.ShowDialog();

            ListarMovimentacoes();
        }

        private void btRetirada_Click(object sender, RoutedEventArgs e)
        {
            ShowMovimentacao(Tipo_movimentacao_caixa.SAIDA);
        }

        private void btRelatorio_Click(object sender, RoutedEventArgs e)
        {
            Movimentos_caixasController mc_controller = new Movimentos_caixasController();
            
            HashSet<Usuarios> usuarios = new HashSet<Usuarios>();
            List<Caixas> caixas = new List<Caixas>() { new CaixasController().Find(mc_controller.Get_ID_CaixaAtualUsuario()) };
            HashSet<Formas_pagamento> formas_pg = new HashSet<Formas_pagamento>();

            mc_controller.DisableAntiTracking();
            List<Movimentos_caixas> movimentos = mc_controller.GetMovimentosCaixaAtual();

            foreach (Movimentos_caixas movimento in movimentos)
            {
                if (usuarios.FirstOrDefault(u => u.Id == movimento.Usuario_id) == null)
                    usuarios.Add(movimento.Usuarios);

                if (formas_pg.FirstOrDefault(f => f.Id == movimento.Forma_pagamento_id) == null)
                    formas_pg.Add(movimento.Formas_pagamento);

                movimento.Usuarios = null;
                movimento.Formas_pagamento = null;
                movimento.Caixas = null;
                movimento.Lojas = null;
            }

            IControllerReport rController = ReportController.GetInstance();
            rController.AddDataSource("Movimentos_caixas", movimentos);
            rController.AddDataSource("Usuarios", usuarios);
            rController.AddDataSource("Caixas", caixas);
            rController.AddDataSource("Formas_pagamento", formas_pg);
            rController.AddDataSource("Lojas", new List<Lojas>() { UsuariosController.LojaAtual });

            rController.BindParameter("ValorAbertura", mc_controller.GetUltimoMovimentoAbertura().Valor);
            rController.BindParameter("TotalEntradas", mc_controller.GetTotalMovimentacoesCaixaAtual(Tipo_movimentacao_caixa.ENTRADA));
            rController.BindParameter("TotalSaidas", mc_controller.GetTotalMovimentacoesCaixaAtual(Tipo_movimentacao_caixa.SAIDA));
            rController.BindParameter("TotalCaixa", mc_controller.GetTotalCaixa());
            rController.BindParameter("UsuarioImpressao", UsuariosController.UsuarioAtual.Nome);

            rController.ShowReport("Relatório de caixa", "CXACONS001");
        }

        private void btFecharCaixa_Click(object sender, RoutedEventArgs e)
        {
            FechamentoCaixa fc = new FechamentoCaixa();
            fc.ShowDialog();

            if (fc.CaixaFechado)
                CaixaFechado?.Invoke();
        }
    }
}
