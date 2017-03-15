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
using VarejoSimples.Model;
using VarejoSimples.Views.PDV;

namespace VarejoSimples.Views.PDV
{
    /// <summary>
    /// Lógica interna para PagamentosPDV.xaml
    /// </summary>
    public partial class PagamentosPDV : Window
    {
        private List<KeyValuePair<int, Formas_pagamento>> Atalhos { get; set; }
        public List<Itens_pagamento> Itens_pagamento { get; set; }
        private Formas_pagamento Fpg_Atual { get; set; }
        private int AtalhoAtual { get; set; }
        private decimal ValorTotal { get; set; }
        public bool Pago { get; set; }
        public PagamentosPDV(List<KeyValuePair<int, Formas_pagamento>> atalhosPagamentos, decimal valorTotal)
        {
            InitializeComponent();

            Pago = false;
            Atalhos = atalhosPagamentos;
            Itens_pagamento = new List<Model.Itens_pagamento>();
            Atalhos.ForEach(e => sp_formas_pag.Children.Add(new CardPagamento(e.Key, e.Value.Descricao, 0)));
            txValorPagar.ToMoney();
            lbForma_pagamento.Content = "Pressione uma das teclas ao lado";
            txValorPagar.IsEnabled = false;
            btConfirmar.IsEnabled = false;
            lbDicaEnter.Visibility = Visibility.Hidden;
            ValorTotal = valorTotal;

            lbTotal.Content = $"R$ {ValorTotal.ToString("N2")}";
            lbFalta.Content = $"R$ {ValorTotal.ToString("N2")}";
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if((e.Key == Key.Enter) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Confirmar();
                return;
            }

            switch (e.Key)
            {
                case Key.F1:
                    MostraFormaPagamento(1);
                    break;

                case Key.F2:
                    MostraFormaPagamento(2);
                    break;
            }
        }

        private void MostraFormaPagamento(int atalho)
        {
            KeyValuePair<int, Formas_pagamento> pair = Atalhos.FirstOrDefault(e => e.Key == atalho);
            if (pair.Key == 0)
                return;

            txValorPagar.IsEnabled = true;
            lbDicaEnter.Visibility = Visibility.Visible;
            AtalhoAtual = pair.Key;
            Fpg_Atual = pair.Value;
            lbForma_pagamento.Content = pair.Value.Descricao;
            txValorPagar.Focus();
            txValorPagar.SelectAll();
        }

        private void txValorPagar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                Itens_pagamento.Add(new Model.Itens_pagamento()
                {
                    Formas_pagamento = Fpg_Atual,
                    Forma_pagamento_id = Fpg_Atual.Id,
                    Valor = decimal.Parse(txValorPagar.Text),
                });

                foreach (CardPagamento cp in sp_formas_pag.Children)
                    if (cp.Atalho == AtalhoAtual)
                        cp.ValorPago += decimal.Parse(txValorPagar.Text);

                decimal total_pago = Itens_pagamento.Sum(i => i.Valor);
                decimal falta_pagar = (ValorTotal - total_pago);

                lbPago.Content = $"R$ {total_pago.ToString("N2")}";

                if (falta_pagar <= 0)
                {
                    lbFalta.Content = "R$ 0,00";
                    lbTroco.Content = $"R$ {(falta_pagar * (-1)).ToString("N2")}";
                }
                else
                {
                    lbFalta.Content = $"R$ {falta_pagar.ToString("N2")}";
                    lbTroco.Content = "R$ 0,00";
                }

                AtalhoAtual = 0;
                Fpg_Atual = null;
                lbForma_pagamento.Content = "Pressione uma das teclas ao lado";
                txValorPagar.Text = "0,00";
                txValorPagar.IsEnabled = false;
                lbDicaEnter.Visibility = Visibility.Hidden;

                btConfirmar.IsEnabled = (decimal.Parse(lbFalta.Content.ToString().Replace("R$ ", string.Empty)) == 0
                    ? true
                    : false);
            }
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
            Pago = btConfirmar.IsEnabled;
            Close();
        }
    }
}
