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

namespace VarejoSimples.Views.VendaRapida
{
    /// <summary>
    /// Interação lógica para ItensVenda.xam
    /// </summary>
    public partial class ItensVenda : UserControl
    {
        public ItensVenda()
        {
            InitializeComponent();
        }

        public void AdicionaItem(Produtos produto, decimal quant)
        {
            sp_produtos.Children.Add(new ItemVenda(1, produto, quant));
        }
    }
}
