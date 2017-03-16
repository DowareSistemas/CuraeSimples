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
using VarejoSimples.Model;

namespace VarejoSimples.Views.PDV
{
    /// <summary>
    /// Interação lógica para CardPagamento.xam
    /// </summary>
    public partial class CardPagamento : UserControl
    {
        private decimal valorpago = 0;

        public int Atalho { get; set; }
        public decimal ValorPago
        {
            get
            {
                return valorpago;
            }
            set
            {
                valorpago = value;
                lbValor.Content = $"R$ {valorpago.ToString("N2")}";
            }
        }

        public CardPagamento(int atalho, string descricao_fpg, decimal valor_pago)
        {
            InitializeComponent();

            lbDescricao.Text = "F" + atalho + " - " + descricao_fpg;
            lbValor.Content = $"R$ {valor_pago.ToString("N2")}";

            Atalho = atalho;
            ValorPago = valor_pago;
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void lbDescricao_TouchDown(object sender, TouchEventArgs e)
        {

        }
    }
}
