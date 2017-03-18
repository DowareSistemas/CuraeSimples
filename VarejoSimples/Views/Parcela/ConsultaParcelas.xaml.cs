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
using VarejoSimples.Tasks;

namespace VarejoSimples.Views.Parcela
{
    /// <summary>
    /// Lógica interna para ConsultaParcelas.xaml
    /// </summary>
    public partial class ConsultaParcelas : Window
    {
        private Tipo_parcela Tipo { get; set; }

        varejo_config Context = new varejo_config();
        private bool enabledChangeCb = false;

        public ConsultaParcelas(Tipo_parcela tipo)
        {
            InitializeComponent();

            Tipo = tipo;

            switch(tipo)
            {
                case Tipo_parcela.PAGAR:
                    this.Title = "Contas a pagar";
                    dataGrid.Columns[8].Visibility = Visibility.Hidden;
                    break;

                case Tipo_parcela.RECEBER:
                    this.Title = "Contas a receber";
                    dataGrid.Columns[9].Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void AcionarBusca()
        {
            int pagina_atual = (int.Parse(txPagina_atual.Text) * int.Parse(txNumero_registros.Text));
            int numero_registros = int.Parse(txNumero_registros.Text);
            int mes = (int)cbMes.SelectedValue;

            ConsultaParcelasTask task = new ConsultaParcelasTask(this);
            task.Execute(new object[] { Tipo, pagina_atual, numero_registros, mes });
        }

        private void btAtualizar_Click(object sender, RoutedEventArgs e)
        {
            ParcelasController controller = new ParcelasController();

            int numero_paginas = 0;
            numero_paginas = (controller.CountBusca(Tipo, (int)cbMes.SelectedValue) / int.Parse(txNumero_registros.Text));
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

        private void cbMes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!enabledChangeCb)
                return;

            if (cbMes == null)
                return;

            int count = (new ParcelasController().CountBusca(Tipo,  (int)cbMes.SelectedValue));
            txNumero_paginas.Text = (count / int.Parse(txNumero_registros.Text)).ToString();
            txPagina_atual.Text = "0";

            AcionarBusca();
        }

        bool feito = false;
        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (feito)
                return;

            enabledChangeCb = false;

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

            dataGrid.AplicarPadroes();
            dataGrid.FontSize = 15;
            dataGrid.MinRowHeight = 20;
            dataGrid.AlternatingRowBackground = Brushes.Lavender;

            txNumero_paginas.ToNumeric();
            txNumero_registros.ToNumeric();
            txPagina_atual.ToNumeric();

            int count =  (new ParcelasController().CountBusca(Tipo, (int)cbMes.SelectedValue));
            txNumero_paginas.Text = (count / int.Parse(txNumero_registros.Text)) .ToString();

            AcionarBusca();
            feito = true;
            enabledChangeCb = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ParcelaAdapter selecionado = (ParcelaAdapter)dataGrid.SelectedItem;
            if (selecionado == null)
                return;


            DetalhesParcela detalhes = new DetalhesParcela(selecionado.Parcela.Id);
            detalhes.ShowDialog();
        }
    }

    public class ParcelaAdapter
    {
        public Parcelas Parcela { get; set; }
        public BitmapImage ImgStatus { get; set; }
        public string Portador { get; set; }
        public string Cliente { get; set; }
        public string Fornecedor { get; set; }
        public string Tipo { get; set; }

        public ParcelaAdapter(Parcelas parcela, varejo_config context)
        {
            Parcela = parcela;

            switch(Parcela.Tipo_parcela)
            {
                case (int)Tipo_parcela.PAGAR:
                    Tipo = "PAGAR";
                    break;

                case (int)Tipo_parcela.RECEBER:
                    Tipo = "RECEBER";
                    break; 
            }

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                switch (parcela.Situacao)
                {
                    case (int)Situacao_parcela.EM_ABERTO:
                        ImgStatus = new BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "/Images/verde.png"));
                        break;

                    case (int)Situacao_parcela.PAGA:
                        ImgStatus = new BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "/Images/vermelho.png"));
                        break;

                    case (int)Situacao_parcela.CANCELADA:
                        ImgStatus = new BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "/Images/cinza.png"));
                        break;

                    case (int)Situacao_parcela.RENEGOCIADA:
                        ImgStatus = new BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "/Images/amarelo.png"));
                        break;
                }
            }));

            if (parcela.Portador > 0)
                Portador = context.Contas.Find(parcela.Portador).Nome;

            if (parcela.Cliente_id > 0)
                Cliente = context.Clientes.Find(parcela.Cliente_id).Nome;

            if (parcela.Fornecedor_id > 0)
                Fornecedor = context.Fornecedores.Find(parcela.Fornecedor_id).Nome;
        }
    }
}
