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
using VarejoSimples.Views.Conta;
using VarejoSimples.Views.Operadora_cartao;

namespace VarejoSimples.Views.Forma_pagto
{
    /// <summary>
    /// Lógica interna para CadFormas_pag.xaml
    /// </summary>
    public partial class CadFormas_pag : Window
    {
        Formas_pagamentoController controller;
        public CadFormas_pag()
        {
            InitializeComponent();

            txDescricao.Focus();
            controller = new Formas_pagamentoController();
            List<KeyValuePair<Tipo_pagamento, string>> tipos_pag = new List<KeyValuePair<Tipo_pagamento, string>>();
            tipos_pag.Add(new KeyValuePair<Tipo_pagamento, string>(Tipo_pagamento.DINHEIRO, "DINHEIRO"));
            tipos_pag.Add(new KeyValuePair<Tipo_pagamento, string>(Tipo_pagamento.CARTAO, "CARTÃO"));
            tipos_pag.Add(new KeyValuePair<Tipo_pagamento, string>(Tipo_pagamento.CREDITO, "CRÉDITO CLIENTE"));
            tipos_pag.Add(new KeyValuePair<Tipo_pagamento, string>(Tipo_pagamento.PRAZO, "Á PRAZO"));
            tipos_pag.Add(new KeyValuePair<Tipo_pagamento, string>(Tipo_pagamento.CHEQUE, "CHEQUE"));

            cbTipo_pagamento.ItemsSource = tipos_pag;
            cbTipo_pagamento.DisplayMemberPath = "Value";
            cbTipo_pagamento.SelectedValuePath = "Key";

            List<KeyValuePair<int, string>> tipos_intev = new List<KeyValuePair<int, string>>();
            tipos_intev.Add(new KeyValuePair<int, string>(0, "Intervalo"));
            tipos_intev.Add(new KeyValuePair<int, string>(1, "Dia base"));

            cbTipo_intervalo.ItemsSource = tipos_intev;
            cbTipo_intervalo.DisplayMemberPath = "Value";
            cbTipo_intervalo.SelectedValuePath = "Key";

            cbTipo_pagamento.SelectedIndex = 0;
            cbTipo_intervalo.SelectedIndex = 0;

            txCod.ToNumeric();
            txInterv_diaBase.ToNumeric();
            txParcelas.ToNumeric();
            txCod_operadora.ToNumeric();
            txCod_conta.ToNumeric(); 
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            Formas_pagamento fpg = controller.Next(int.Parse(txCod.Text));
            FillFpg(fpg);
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCod.Text);
            if ((id - 1) <= 0)
            {
                LimparCampos();
                return;
            }

            Formas_pagamento fpg = controller.Prev(id);
            FillFpg(fpg);
        }

        private void FillFpg(Formas_pagamento fpg)
        {
            if (fpg == null)
                return;

            LimparCampos();
            txCod.Text = fpg.Id.ToString();
            txDescricao.Text = fpg.Descricao;
            cbTipo_intervalo.SelectedIndex = fpg.Tipo_intervalo;
            cbTipo_pagamento.SelectedIndex = fpg.Tipo_pagamento;
            txParcelas.Text = fpg.Parcelas.ToString();
            
            if(fpg.Conta_id > 0)
            {
                Contas conta = new ContasController().Find(fpg.Conta_id);
                txCod_conta.Text = conta.Id.ToString();
                txConta.Text = conta.Nome;
            }

            if (fpg.Tipo_intervalo == (int)Tipo_intervalo.DATA_BASE)
            {
                lbIntervalo_diaBase.Content = "Dia base";
                txInterv_diaBase.Text = fpg.Dia_base.ToString();
            }
            else
            {
                lbIntervalo_diaBase.Content = "Intervalo (dias)";
                txInterv_diaBase.Text = fpg.Intervalo.ToString();
            }

            if (fpg.Operadora_cartao_id > 0)
            {
                Operadoras_cartao op = new Operadoras_cartaoController().Find(fpg.Operadora_cartao_id);
                txCod_operadora.Text = op.Id.ToString();
                txNome_operadora.Text = op.Nome;
            }

            if (fpg.Tipo_pagamento == (int)Tipo_pagamento.DINHEIRO)
            {
                cbTipo_intervalo.IsEnabled = false;
                txInterv_diaBase.IsEnabled = false;
                txParcelas.IsEnabled = false;
                btSelecionarOperadora.IsEnabled = false;
            }

            if (fpg.Tipo_pagamento == (int)Tipo_pagamento.CREDITO)
            {
                cbTipo_intervalo.IsEnabled = false;
                txInterv_diaBase.IsEnabled = false;
                txParcelas.IsEnabled = false;
                btSelecionarOperadora.IsEnabled = false;
                btSelecionarConta.IsEnabled = false;
            }

            if(fpg.Tipo_pagamento == (int) Tipo_pagamento.CHEQUE)
            {
                cbTipo_intervalo.IsEnabled = true;
                txInterv_diaBase.IsEnabled = true;
                txParcelas.IsEnabled = true;
                btSelecionarOperadora.IsEnabled = false;
                btSelecionarConta.IsEnabled = true;
            }

            if(fpg.Tipo_pagamento == (int) Tipo_pagamento.PRAZO)
            {
                cbTipo_intervalo.IsEnabled = true;
                txInterv_diaBase.IsEnabled = true;
                txParcelas.IsEnabled = true;
                btSelecionarOperadora.IsEnabled = false;
                btSelecionarConta.IsEnabled = true;
            }

            if (fpg.Tipo_intervalo == (int)Tipo_pagamento.CARTAO)
            {
                cbTipo_intervalo.IsEnabled = false;
                txInterv_diaBase.IsEnabled = false;
                txParcelas.IsEnabled = false;
                btSelecionarOperadora.IsEnabled = true;
                btSelecionarConta.IsEnabled = true;
            }

            txDescricao.Focus();
        }

        private void Salvar()
        {
            Formas_pagamento fp = (int.Parse(txCod.Text) == 0
                ? new Formas_pagamento()
                : controller.Find(int.Parse(txCod.Text)));

            fp.Id = int.Parse(txCod.Text);
            fp.Descricao = txDescricao.Text;
            fp.Tipo_pagamento = cbTipo_pagamento.SelectedIndex;
            fp.Tipo_intervalo = cbTipo_intervalo.SelectedIndex;

            switch (fp.Tipo_intervalo)
            {
                case (int)Tipo_intervalo.DATA_BASE:
                    fp.Dia_base = int.Parse(txInterv_diaBase.Text);
                    break;

                case (int)Tipo_intervalo.INTERVALO:
                    fp.Intervalo = int.Parse(txInterv_diaBase.Text);
                    break;
            }

            fp.Parcelas = int.Parse(txParcelas.Text);
            fp.Operadora_cartao_id = int.Parse(txCod_operadora.Text);
            fp.Conta_id = int.Parse(txCod_conta.Text);

            if (controller.Save(fp))
                LimparCampos();
        }

        private void LimparCampos()
        {
            txCod.Text = "0";
            txDescricao.Text = string.Empty;
            cbTipo_intervalo.SelectedIndex = 0;
            cbTipo_pagamento.SelectedIndex = 0;
            txInterv_diaBase.Text = "0";
            txParcelas.Text = "0";
            txCod_operadora.Text = "0";
            txNome_operadora.Text = string.Empty;
            txCod_conta.Text = "0";
            txConta.Text = string.Empty;

            txDescricao.Focus();
        }

        private void btNovo_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            Salvar();
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(txCod.Text);
            if (id == 0)
                return;

            if (controller.Remove(id))
                LimparCampos();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void cbTipo_pagamento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((Tipo_pagamento)cbTipo_pagamento.SelectedValue == Tipo_pagamento.DINHEIRO)
            {
                cbTipo_intervalo.IsEnabled = false;
                txInterv_diaBase.IsEnabled = false;
                txParcelas.IsEnabled = false;
                btSelecionarOperadora.IsEnabled = false;
                btSelecionarConta.IsEnabled = false;
            }

            if ((Tipo_pagamento)cbTipo_pagamento.SelectedValue ==Tipo_pagamento.CREDITO)
            {
                cbTipo_intervalo.IsEnabled = false;
                txInterv_diaBase.IsEnabled = false;
                txParcelas.IsEnabled = false;
                btSelecionarOperadora.IsEnabled = false;
                btSelecionarConta.IsEnabled = false;
            }

            if((Tipo_pagamento )cbTipo_pagamento.SelectedValue == Tipo_pagamento.CHEQUE)
            {
                cbTipo_intervalo.IsEnabled = false;
                txInterv_diaBase.IsEnabled = false;
                txParcelas.IsEnabled = false;
                btSelecionarOperadora.IsEnabled = false;
                btSelecionarConta.IsEnabled = true;
            }

            if ((Tipo_pagamento)cbTipo_pagamento.SelectedValue == Tipo_pagamento.CARTAO)
            {
                cbTipo_intervalo.IsEnabled = false;
                txInterv_diaBase.IsEnabled = false;
                txParcelas.IsEnabled = false;
                btSelecionarOperadora.IsEnabled = true;
                btSelecionarConta.IsEnabled = true;
            }

            if((Tipo_pagamento)cbTipo_pagamento.SelectedValue == Tipo_pagamento.PRAZO)
            {
                cbTipo_intervalo.IsEnabled = true;
                txInterv_diaBase.IsEnabled = true;
                txParcelas.IsEnabled = true;
                btSelecionarOperadora.IsEnabled = false;
                btSelecionarConta.IsEnabled = true;
            }
        }

        private void cbTipo_intervalo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTipo_intervalo.SelectedIndex == (int)Tipo_intervalo.DATA_BASE)
                lbIntervalo_diaBase.Content = "Dia base";
            else
                lbIntervalo_diaBase.Content = "Intervalo (dias)";
        }

        private void btSelecionarOperadora_Click(object sender, RoutedEventArgs e)
        {
            PesquisarOperadora po = new PesquisarOperadora(false);
            po.ShowDialog();

            txCod_operadora.Text = po.Selecionado.Id.ToString();
            txNome_operadora.Text = (po.Selecionado.Id == 0
                ? "Não selecionado"
                : po.Selecionado.Nome);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F3)
            {
                PesquisarFormas_pag pF = new PesquisarFormas_pag();
                pF.ShowDialog();

                FillFpg(pF.Selecionado);
            }
        }

        private void btSelecionarConta_Click(object sender, RoutedEventArgs e)
        {
            PesquisarConta pc = new PesquisarConta(false);
            pc.ShowDialog();

            txCod_conta.Text = pc.Selecionado.Id.ToString();
            txConta.Text = (pc.Selecionado.Id == 0
                ? "Não selecionado"
                : pc.Selecionado.Nome);
        }
    }
}
