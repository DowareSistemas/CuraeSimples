using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VarejoSimples.Controller;
using VarejoSimples.Enums;
using VarejoSimples.Model;
using VarejoSimples.Views.Parcela;

namespace VarejoSimples.Views.Lancamento_financ
{
    /// <summary>
    /// Lógica interna para DetalhesLancamento.xaml
    /// </summary>
    public partial class DetalhesLancamento : Window
    {
        public DetalhesLancamento(int lancamento_id)
        {
            InitializeComponent();

            dataGrid_pagamentos.AplicarPadroes();
            dataGrid_pagamentos.FontSize = 13;
            dataGrid_pagamentos.MinRowHeight = 13;

            dataGrid_parcelas.AplicarPadroes();
            dataGrid_parcelas.FontSize = 13;
            dataGrid_parcelas.MinRowHeight = 13;

            txValor_original.ToMoney();
            txDesconto.ToMoney();
            txAcrescimo.ToMoney();
            txDespesas_acessorias.ToMoney();
            txValor_final.ToMoney();

            Fill(lancamento_id);
        }

        private void Fill(int lancamento_id)
        {
            Lancamentos_financeirosController controller = new Lancamentos_financeirosController();
            Lancamentos_financeiros lancamento = controller.Find(lancamento_id);

            txCod.Text = lancamento.Id.ToString();
            txData.Text = lancamento.Data.ToString("dd/MM/yyyy");
            txTipo.Text = (lancamento.Tipo == (int)Tipo_lancamento.ENTRADA
                ? "ENTRADA"
                : "SAIDA");
            txNum_documento.Text = lancamento.Num_documento;
            txCliente.Text = (lancamento.Cliente_id == 0
                ? string.Empty
                : new ClientesController().Find(lancamento.Cliente_id).Nome);
            txFornecedor.Text = (lancamento.Fornecedor_id == 0
                ? string.Empty
                : new FornecedoresController().Find(lancamento.Fornecedor_id).Nome);
            txUsuario.Text = lancamento.Usuarios.Nome;
            txValor_original.Text = lancamento.Valor_original.ToString("N2");
            txDesconto.Text = lancamento.Desconto.ToString("N2");
            txAcrescimo.Text = lancamento.Acrescimo.ToString("N2");
            txDespesas_acessorias.Text = lancamento.Despesas_acessorias.ToString("N2");
            txValor_final.Text = lancamento.Valor_final.ToString("N2");
            txDescricao.Text = lancamento.Descricao;

            dataGrid_pagamentos.ItemsSource = lancamento.Pagamentos_lancamentos;

            new Thread(() =>
            {
                ParcelasController parcController = new ParcelasController();

                varejo_config context = new varejo_config();
                List<Parcelas> parcelas = parcController.ListByPagamentosLancamento(lancamento.Pagamentos_lancamentos.ToList());
                List<ParcelaAdapter> listAdp = new List<ParcelaAdapter>();
                parcelas.ForEach(e => listAdp.Add(new ParcelaAdapter(e, context)));

                dataGrid_parcelas.Dispatcher.Invoke(new Action<DataGrid>(dt => dataGrid_parcelas.ItemsSource = listAdp), dataGrid_parcelas);
            }).Start();
        }
    }
}
