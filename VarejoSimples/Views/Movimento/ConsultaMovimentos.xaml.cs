﻿using System;
using System.Collections.Generic;
using System.IO;
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
using VarejoSimples.Model;
using VarejoSimples.Tasks;

namespace VarejoSimples.Views.Movimento
{
    /// <summary>
    /// Lógica interna para ConsultaMovimentos.xaml
    /// </summary>
    public partial class ConsultaMovimentos : Window
    {
        varejo_config Context = new varejo_config();
        public ConsultaMovimentos()
        {
            InitializeComponent();

            dataGrid.AplicarPadroes();
            dataGrid.AlternatingRowBackground = Brushes.Lavender;
            txData_inicio.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            txData_fim.SelectedDate = DateTime.Now;

            txNumero_paginas.ToNumeric();
            txNumero_registros.ToNumeric();
            txPagina_atual.ToNumeric();
        }

        private void AcionarBusca()
        {
            string busca = txPesquisa.Text;
            DateTime? data_inicio = txData_inicio.SelectedDate;
            DateTime? data_fim = txData_fim.SelectedDate;
            int pagina_atual = (int.Parse(txPagina_atual.Text) * int.Parse(txNumero_registros.Text));
            int numero_registros = int.Parse(txNumero_registros.Text);

            ConsultaMovimentosTask task = new ConsultaMovimentosTask(this);
            task.Execute(new object[] { busca, data_inicio, data_fim, pagina_atual, numero_registros });
        }

        bool feito = false;
        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (feito)
                return;

            MovimentosController movController = new MovimentosController();

            int numero_paginas = 0;
            numero_paginas = (movController.CountPaginacao(txPesquisa.Text, txData_inicio.SelectedDate, txData_fim.SelectedDate)
                / int.Parse(txNumero_registros.Text));
            txNumero_paginas.Text = numero_paginas.ToString();

            AcionarBusca();
            feito = true;
        }

        private void txPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MovimentosController movController = new MovimentosController();

                int numero_paginas = 0;
                numero_paginas = (movController.CountPaginacao(txPesquisa.Text, txData_inicio.SelectedDate, txData_fim.SelectedDate) / int.Parse(txNumero_registros.Text));
                txNumero_paginas.Text = numero_paginas.ToString();
                txPagina_atual.Text = "0";

                AcionarBusca();
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12)
            {
                dataGrid.IsEnabled = true;
                txPesquisa.IsEnabled = true;
                GridNavegacao.IsEnabled = true;
                imgLoading.Visibility = Visibility.Hidden;
                txPesquisa.Focus();
                Context = new varejo_config();
            }
        }

        private void btProximo_Click(object sender, RoutedEventArgs e)
        {
            int pagina_atual = int.Parse(txPagina_atual.Text);
            if ((pagina_atual + 1) > int.Parse(txNumero_paginas.Text))
                return;

            txPagina_atual.Text = (pagina_atual + 1).ToString();
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

        private void btAtualizar_Click(object sender, RoutedEventArgs e)
        {
            MovimentosController movController = new MovimentosController();

            int numero_paginas = 0;
            numero_paginas = (movController.CountPaginacao(txPesquisa.Text, txData_inicio.SelectedDate, txData_fim.SelectedDate) / int.Parse(txNumero_registros.Text));
            txNumero_paginas.Text = numero_paginas.ToString();

            while (int.Parse(txPagina_atual.Text) > int.Parse(txNumero_paginas.Text))
                txPagina_atual.Text = (int.Parse(txPagina_atual.Text) - 1).ToString();

            AcionarBusca();
        }

        private void btUltimo_Click(object sender, RoutedEventArgs e)
        {
            txPagina_atual.Text = txNumero_paginas.Text;
            AcionarBusca();
        }

        private void btPrimeiro_Click(object sender, RoutedEventArgs e)
        {
            txPagina_atual.Text = "0";
            AcionarBusca();
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MovimentosAdapter movAdp = (MovimentosAdapter)dataGrid.SelectedItem;
            if (movAdp == null)
                return;

            this.IsEnabled = false;
            DetalhesMovimento dm = new DetalhesMovimento(movAdp.Movimentos.Id);
            dm.ShowDialog();
            this.IsEnabled = true;
        }
    }

    public class MovimentosAdapter
    {
        public Movimentos Movimentos { get; set; }
        public Tipos_movimento Tipo_movimento { get; set; }

        public MovimentosAdapter(Movimentos movimento, varejo_config context)
        {
            //    try
            //    {
            Movimentos = movimento;

            if (movimento.Cliente_id > 0)
            {
                Cliente = (from cliente in context.Clientes
                           where cliente.Id == movimento.Cliente_id
                           select cliente.Nome).SingleOrDefault();
            }

            if (movimento.Fornecedor_id > 0)
            {
                Fornecedor = (from fornecedor in context.Fornecedores
                              where fornecedor.Id == movimento.Fornecedor_id
                              select fornecedor.Nome).SingleOrDefault();
            }

            Tipo_movimento = (from tm in context.Tipos_movimento
                              where tm.Id == movimento.Tipo_movimento_id
                              select tm).FirstOrDefault();

            Usuario = (from u in context.Usuarios
                       where u.Id == movimento.Usuario_id
                       select u.Nome).SingleOrDefault();
            //     }
            //    catch (Exception ex) { }
        }

        public string Cliente { get; set; }

        public string Fornecedor { get; set; }

        public string Usuario { get; set; }
    }
}
