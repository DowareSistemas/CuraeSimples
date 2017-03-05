﻿using System;
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
using VarejoSimples.Views.Conta;

namespace VarejoSimples.Views.Lancamento_financ
{
    /// <summary>
    /// Lógica interna para Lancamentos.xaml
    /// </summary>
    public partial class Lancamentos : Window
    {
        private int Conta_id { get; set; }
        private varejo_config context = new varejo_config();
        private Thread thread_busca;

        public Lancamentos()
        {
            InitializeComponent();
        }

        private void SelecionarConta()
        {
            PesquisarConta pc = new PesquisarConta(false);
            pc.ShowDialog();

            if (pc.Selecionado.Id == 0)
            {
                MessageBox.Show("Para realizar consultas ou lançamentos financeiros, é necessário selecionar uma conta para iniciar a sessão. \nSelecione uma conta.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                SelecionarConta();
            }
            else
            {
                Conta_id = pc.Selecionado.Id;
                lbNomeConta.Content = pc.Selecionado.Nome;
            }
        }

        private void LoadComboBox()
        {
            List<KeyValuePair<int, string>> meses = new List<KeyValuePair<int, string>>();
            meses.Add(new KeyValuePair<int, string>(1, "Janeiro"));
            meses.Add(new KeyValuePair<int, string>(2, "Fevereiro"));
            meses.Add(new KeyValuePair<int, string>(3, "Março"));
            meses.Add(new KeyValuePair<int, string>(4, "Abril"));
            meses.Add(new KeyValuePair<int, string>(5, "Maio"));
            meses.Add(new KeyValuePair<int, string>(6, "Junho"));
            meses.Add(new KeyValuePair<int, string>(7, "Julho"));
            meses.Add(new KeyValuePair<int, string>(8, "Agosto"));
            meses.Add(new KeyValuePair<int, string>(9, "Setembro"));
            meses.Add(new KeyValuePair<int, string>(10, "Outubro"));
            meses.Add(new KeyValuePair<int, string>(11, "Novembro"));
            meses.Add(new KeyValuePair<int, string>(12, "Dezembro"));

            cbMes.ItemsSource = meses;
            cbMes.SelectedValuePath = "Key";
            cbMes.DisplayMemberPath = "Value";
            cbMes.SelectedIndex = 0;
            cbMes.SelectedValue = DateTime.Now.Month;
        }

        private void BuscaBasica(int pagina_atual, int numero_registros, int mes)
        {
            if (Conta_id == 0)
                return;

            string saldoConta = context.Contas.Find(Conta_id).Saldo.ToString("N2");

            Lancamentos_financeirosController controller = new Lancamentos_financeirosController();

            GridNavegacao.Dispatcher.Invoke(new Action<Grid>(grd => GridNavegacao.IsEnabled = false), GridNavegacao);
            dataGrid.Dispatcher.Invoke(new Action<DataGrid>(dt => dataGrid.ItemsSource = null), dataGrid);
            imgLoading.Dispatcher.Invoke(new Action<Image>(img => imgLoading.Visibility = Visibility.Visible), imgLoading);

            List<Lancamentos_financeiros> list = controller.BuscaSimples(pagina_atual, numero_registros, mes, Conta_id);
            List<Lancamentos_financeirosAdapter> listAdp = new List<Lancamentos_financeirosAdapter>();
            list.ForEach(e => listAdp.Add(new Lancamentos_financeirosAdapter(e, context)));

            GridNavegacao.Dispatcher.Invoke(new Action<Grid>(grd => GridNavegacao.IsEnabled = true), GridNavegacao);
            dataGrid.Dispatcher.Invoke(new Action<DataGrid>(dt => dataGrid.ItemsSource = listAdp), dataGrid);
            lbSaldoConta.Dispatcher.Invoke(new Action<Label>(lb => lbSaldoConta.Content = saldoConta), lbSaldoConta);
            imgLoading.Dispatcher.Invoke(new Action<Image>(img => imgLoading.Visibility = Visibility.Hidden), imgLoading);
            thread_busca.Abort();
        }

        private void AcionarBusca()
        {
            context = new varejo_config();
            int pagina_atual = (int.Parse(txPagina_atual.Text) * int.Parse(txNumero_registros.Text));
            int numero_registros = int.Parse(txNumero_registros.Text);
            int mes = (int)cbMes.SelectedValue;

            thread_busca = new Thread(() =>
            BuscaBasica(pagina_atual, numero_registros, mes));
            thread_busca.Start();
        }

        private void btAtualizar_Click(object sender, RoutedEventArgs e)
        {
            Lancamentos_financeirosController controller = new Lancamentos_financeirosController();
            
            int numero_paginas = 0;
            numero_paginas = (controller.CountBusca((int)cbMes.SelectedValue, Conta_id) / int.Parse(txNumero_registros.Text));
            txNumero_paginas.Text = numero_paginas.ToString();

            while (int.Parse(txPagina_atual.Text) > int.Parse(txNumero_paginas.Text))
                txPagina_atual.Text = (int.Parse(txPagina_atual.Text) - 1).ToString();

            AcionarBusca();
        }

        private void btPrimeiro_Click(object sender, RoutedEventArgs e)
        {
            txPagina_atual.Text = "0";
            AcionarBusca();
        }

        private void btAnterior_Click(object sender, RoutedEventArgs e)
        {
            int pagina_atual = int.Parse(txPagina_atual.Text);
            if ((pagina_atual - 1) < 0)
                return;

            txPagina_atual.Text = (pagina_atual - 1).ToString();
            AcionarBusca();
        }

        private void btProximo_Click(object sender, RoutedEventArgs e)
        {
            int pagina_atual = int.Parse(txPagina_atual.Text);
            if ((pagina_atual + 1) > int.Parse(txNumero_paginas.Text))
                return;

            txPagina_atual.Text = (pagina_atual + 1).ToString();
            AcionarBusca();
        }

        private void btUltimo_Click(object sender, RoutedEventArgs e)
        {
            txPagina_atual.Text = txNumero_paginas.Text;
            AcionarBusca();
        }

        private void btBuscaDetalhada_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btNovo_Click(object sender, RoutedEventArgs e)
        {
            CadLancamento cadastro = new CadLancamento(Conta_id);
            cadastro.ShowDialog();

            AcionarBusca();
        }

        bool enabledChangeCb = false;

        private void cbMes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!enabledChangeCb)
                return;

            if (cbMes == null)
                return;

            int count = (new Lancamentos_financeirosController().CountBusca((int)cbMes.SelectedValue, Conta_id));
            txNumero_paginas.Text = (count / int.Parse(txNumero_registros.Text)).ToString();
            txPagina_atual.Text = "0";

            AcionarBusca();
        }

        bool feito = false;
        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (feito)
                return;
            enabledChangeCb = true;
            LoadComboBox();

            BStatus.Attach(3, lbStatus, image);

            dataGrid.AplicarPadroes();
            dataGrid.FontSize = 15;
            dataGrid.MinRowHeight = 20;
            dataGrid.AlternatingRowBackground = Brushes.Lavender;
            SelecionarConta();
            AcionarBusca();
            feito = true;
        }

        private void btTransferencia_Click(object sender, RoutedEventArgs e)
        {
            Transferencia.Transferencia transf = new Transferencia.Transferencia();
            transf.ShowDialog();

            AcionarBusca();
        }
    }

    public class Lancamentos_financeirosAdapter
    {
        public Lancamentos_financeiros Lancamento { get; set; }
        public string Tipo { get; set; }
        public string Fornecedor { get; set; }
        public string Cliente { get; set; }

        public Lancamentos_financeirosAdapter(Lancamentos_financeiros lancamento, varejo_config context)
        {
            Lancamento = lancamento;

            Tipo = (lancamento.Tipo == (int)Tipo_lancamento.ENTRADA
                ? "ENTRADA"
                : "SAIDA");

            if (lancamento.Fornecedor_id > 0)
                Fornecedor = context.Fornecedores.Find(lancamento.Fornecedor_id).Nome;

            if (lancamento.Cliente_id > 0)
                Cliente = context.Clientes.Find(lancamento.Cliente_id).Nome;
        }
    }
}
