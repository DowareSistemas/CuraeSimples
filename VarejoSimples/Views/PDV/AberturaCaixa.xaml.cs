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

namespace VarejoSimples.Views.PDV
{
    /// <summary>
    /// Interaction logic for AberturaCaixa.xaml
    /// </summary>
    public partial class AberturaCaixa : Window
    {
        Movimentos_caixasController controller = null;

        public bool CaixaAberto { get; set; }

        public AberturaCaixa()
        {
            InitializeComponent();

            dataGrid.AplicarPadroes();
            controller = new Movimentos_caixasController();
            ListarCaixasDisponiveis();
            txFundo_troco.ToMoney();
            CaixaAberto = false;
        }

        private void ListarCaixasDisponiveis()
        {
            List<Caixas> list = new CaixasController().CaixasAbertos();
            dataGrid.ItemsSource = list;

            dataGrid.Focus();
            if (list.Count > 0)
                dataGrid.SelectedIndex = 0;
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            Confirmar();
        }

        private void Confirmar()
        {
            Caixas selecionado = (Caixas)dataGrid.SelectedItem;

            if (selecionado == null)
                return;

            int tipo_pgDinheiro = (int)Tipo_pagamento.DINHEIRO;

            controller.AbreCaixa(
                   selecionado.Id,
                   decimal.Parse(txFundo_troco.Text),
                   UsuariosController.UsuarioAtual.Id,
                   new Formas_pagamentoController().Get(fpg => fpg.Tipo_pagamento == tipo_pgDinheiro).Id);

            CaixaAberto = controller.CaixaAberto(UsuariosController.UsuarioAtual.Id);
            Close();
        }

        private void btFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Caixas caixa = (Caixas)dataGrid.SelectedItem;
                if (caixa == null)
                    return;

                txFundo_troco.Focus();
                txFundo_troco.SelectAll();
            }
        }

        private void txFundo_troco_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Confirmar();
        }
    }
}
