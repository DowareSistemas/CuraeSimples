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
using VarejoSimples.Interfaces;

namespace VarejoSimples.Views.Movimento.LancamentoCheque
{
    /// <summary>
    /// Lógica interna para LancamentoCheque.xaml
    /// </summary>
    public partial class LancamentoCheque : Window, IRegistroCheques
    {
        public LancamentoCheque()
        {
            InitializeComponent();
        }

        public List<Cheque> Cheques { get; set; }
        
        public void Exibir(decimal valor_pagamento)
        {
            this.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
