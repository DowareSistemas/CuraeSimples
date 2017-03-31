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
using VarejoSimples.Controller;
using VarejoSimples.Model;

namespace VarejoSimples.Views.PDV
{
    /// <summary>
    /// Interaction logic for ItemFpgFechamentoCaixa.xaml
    /// </summary>
    public partial class ItemFpgFechamentoCaixa : UserControl
    {
        public Formas_pagamento Forma_pagamento { get; set; }
        public decimal Valor_esperado { get; set; }

        public decimal Valor_informado
        {
            get
            {
                return decimal.Parse(txValor.Text);
            }
        }

        public ItemFpgFechamentoCaixa(Formas_pagamento fpg, decimal valor)
        {
            InitializeComponent();

            lbForma_pagamento.Content = fpg.Descricao;
            Valor_esperado = valor;
            Forma_pagamento = fpg;

            txValor.ToMoney();
        }
    }
}
