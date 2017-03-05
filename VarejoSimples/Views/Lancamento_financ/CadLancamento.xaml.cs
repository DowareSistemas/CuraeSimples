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
using VarejoSimples.Views.Cliente;
using VarejoSimples.Views.Fornecedor;
using VarejoSimples.Views.Plano_conta;

namespace VarejoSimples.Views.Lancamento_financ
{
    /// <summary>
    /// Lógica interna para CadLancamento.xaml
    /// </summary>
    public partial class CadLancamento : Window
    {
        private Lancamentos_financeirosController controller;
        private int Conta_id { get; set; }
        public CadLancamento(int conta_id)
        {
            Conta_id = conta_id;

            InitializeComponent();

            controller = new Lancamentos_financeirosController();

            txValor_original.ToMoney();
            txDesconto.ToMoney();
            txAcrescimo.ToMoney();
            txDespesas_acessorias.ToMoney();
            txValor_final.ToMoney();

            txConta.Text = new ContasController().Find(conta_id).Nome;

            List<KeyValuePair<Tipo_lancamento, string>> tipos = new List<KeyValuePair<Tipo_lancamento, string>>();
            tipos.Add(new KeyValuePair<Tipo_lancamento, string>(Tipo_lancamento.ENTRADA, "ENTRADA"));
            tipos.Add(new KeyValuePair<Tipo_lancamento, string>(Tipo_lancamento.SAIDA, "SAIDA"));

            cbTipo.ItemsSource = tipos;
            cbTipo.DisplayMemberPath = "Value";
            cbTipo.SelectedValuePath = "Key";
            cbTipo.SelectedIndex = 0;
        }

        private void Salvar(bool close)
        {
            if (txData.SelectedDate == null)
            {
                MessageBox.Show("Informe a data do lançamento", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            Lancamentos_financeiros lancamento = new Lancamentos_financeiros();

            lancamento.Data = (DateTime)txData.SelectedDate;
            lancamento.Conta_id = Conta_id;
            lancamento.Num_documento = txNum_documento.Text;
            lancamento.Origem = (int)Origem_lancamento.NENHUM;
            lancamento.Tipo = ((int)(Tipo_lancamento)cbTipo.SelectedValue);
            lancamento.Plano_conta_id = int.Parse(txCod_planoConta.Text);
            lancamento.Valor_original = decimal.Parse(txValor_original.Text);
            lancamento.Desconto = decimal.Parse(txDesconto.Text);
            lancamento.Acrescimo = decimal.Parse(txAcrescimo.Text);
            lancamento.Despesas_acessorias = decimal.Parse(txDespesas_acessorias.Text);
            lancamento.Valor_final = decimal.Parse(txValor_final.Text);
            lancamento.Usuario_id = UsuariosController.UsuarioAtual.Id;
            lancamento.Descricao = txDescricao.Text;
            lancamento.Cliente_id = int.Parse(txCod_cliente.Text);
            lancamento.Fornecedor_id = int.Parse(txCod_fornecedor.Text);

            PagamentoLancamento pagamento = new PagamentoLancamento();
            pagamento.Exibir(lancamento.Valor_final);

            lancamento.Pagamentos_lancamentos = pagamento.Pagamentos;

            if(controller.Save(lancamento))
            {
                if (close)
                    Close();
                else
                    LimparCampos();
            }
        }

        private void LimparCampos()
        {

        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            Salvar(true);
        }

        private void btSelecionarPlano_Click(object sender, RoutedEventArgs e)
        {
            SelecionarPlanoConta spc = new SelecionarPlanoConta();
            spc.ShowDialog();

            txCod_planoConta.Text = spc.Selecionado.Id.ToString();
            txNome_plano.Text = (spc.Selecionado.Id == 0
                ? "Não selecionado"
                : spc.Selecionado.Descricao);
        }

        private void btSelecionar_cliente_Click(object sender, RoutedEventArgs e)
        {
            PesquisarCliente pc = new PesquisarCliente();
            pc.ShowDialog();

            txCod_cliente.Text = pc.Selecionado.Id.ToString();
            txNome_cliente.Text = (pc.Selecionado.Id == 0
                ? "Não selecionado"
                : pc.Selecionado.Nome);
        }

        private void btSelecionar_fornecedor_Click(object sender, RoutedEventArgs e)
        {
            PesquisarFornecedor pf = new PesquisarFornecedor();
            pf.ShowDialog();

            txCod_fornecedor.Text = pf.Selecionado.Id.ToString();
            txNome_fornecedor.Text = (pf.Selecionado.Id == 0
                ? "Não selecionado"
                : pf.Selecionado.Nome);
        }

        private void btSalvarEContinuar_Click(object sender, RoutedEventArgs e)
        {
            Salvar(false);
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
