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
using VarejoSimples.Enums;
using VarejoSimples.Model;
using VarejoSimples.Views.Lancamento_financ;
using VarejoSimples.Views.Movimento;

namespace VarejoSimples.Views.Parcela
{
    /// <summary>
    /// Lógica interna para DetalhesParcela.xaml
    /// </summary>
    public partial class DetalhesParcela : Window
    {
        private Parcelas Parcela { get; set; }
        public DetalhesParcela(int parcela_id)
        {
            InitializeComponent();

            grid_movimento.Visibility = Visibility.Hidden;
            grid_parcelaAnterior.Visibility = Visibility.Hidden;
            grid_proximaParcela.Visibility = Visibility.Hidden;
            grid_lancamento.Visibility = Visibility.Hidden;
            gp_cheque.IsEnabled = false;

            FillParcela(parcela_id);
        }

        private void FillParcela(int parcela_id)
        {
            ParcelasController pController = new ParcelasController();

            Parcelas parcela = pController.Find(parcela_id);
            Parcela = parcela;

            txCod.Text = parcela.Id.ToString();
            txTipo.Text = (parcela.Tipo_parcela == (int)Tipo_parcela.PAGAR
                ? "PAGAR"
                : "RECEBER");
            txNum_documento.Text = parcela.Num_documento;
            txData_lancamento.Text = parcela.Data_lancamento.ToString("dd/MM/yyyy");
            txData_vencimento.Text = parcela.Data_vencimento.ToString("dd/MM/yyyy");
            grid_parcelaAnterior.Visibility = (pController.ExisteParcelaAnterior(parcela_id)
                ? Visibility.Visible
                : Visibility.Hidden);

            if (parcela.Cliente_id > 0)
                txCliente.Text = new ClientesController().Find(parcela.Cliente_id).Nome;

            if (parcela.Fornecedor_id > 0)
                txFornecedor.Text = new FornecedoresController().Find(parcela.Fornecedor_id).Nome;

            if (parcela.Portador > 0)
                txPortador.Text = new ContasController().Find(parcela.Portador).Nome;

            txValor.Text = parcela.Valor.ToString("N2");
            txDias_compensacao.Text = parcela.Dias_compensacao.ToString();
            txJuros_atraso.Text = parcela.Juros_atraso.ToString();
            txObs.Text = parcela.Parcela_descricao;

            if(!string.IsNullOrEmpty(parcela.Numero_cheque))
            {
                gp_cheque.IsEnabled = true;

                txNumero_cheque.Text = parcela.Numero_cheque;
                txBanco.Text = parcela.Banco;
                txAgencia.Text = parcela.Agencia;
                txDias_compensacao.Text = parcela.Dias_compensacao.ToString();
                txConta.Text = parcela.Conta;

                if(parcela.Tipo_parcela == (int)Tipo_parcela.PAGAR)
                    gp_cheque.IsEnabled = false;
            }

            switch(parcela.Situacao)
            {
                case (int)Situacao_parcela.PAGA:
                   txSituacao.Text =  "PAGA";
                    break;

                case (int)Situacao_parcela.EM_ABERTO:
                    txSituacao.Text = "EM ABERTO";
                    break;

                case (int)Situacao_parcela.CANCELADA:
                    txSituacao.Text = "CANCELADA";
                    break;

                case (int)Situacao_parcela.RENEGOCIADA:
                    txSituacao.Text = "RENEGOCIADA";
                    grid_proximaParcela.Visibility = Visibility.Visible;
                    break;
            }

            grid_movimento.Visibility = (parcela.Item_pagamento_id > 0
                ? Visibility.Visible
                : Visibility.Hidden);

            grid_lancamento.Visibility = (parcela.Pagamento_lancamento_id > 0
                ? Visibility.Visible
                : Visibility.Hidden);
        }
       
        private void Run_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Itens_pagamentoController itens_pg = new Itens_pagamentoController();
            Movimentos movimento = itens_pg.FindMovimentoByCodItemPg(Parcela.Item_pagamento_id);

            DetalhesMovimento detalhesMov = new DetalhesMovimento(movimento.Id);
            detalhesMov.ShowDialog();
        }

        private void Run_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            Pagamentos_lancamentosController plController = new Pagamentos_lancamentosController();
            Lancamentos_financeiros lf = plController.FindLancamentoByPagamentoId(Parcela.Pagamento_lancamento_id);

            DetalhesLancamento detalhes = new DetalhesLancamento(lf.Id);
            detalhes.ShowDialog();
        }
    }
}
