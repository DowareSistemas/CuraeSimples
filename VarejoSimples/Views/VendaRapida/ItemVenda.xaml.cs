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
    /// Interação lógica para ItemVenda.xam
    /// </summary>
    public partial class ItemVenda : UserControl
    {
        public ItemVenda(int numero_item, Produtos produto, decimal quant)
        {
            InitializeComponent();

            lbNumero_item.Content = numero_item.ToString();
            lbDescricao.Content = produto.Descricao;
            lbUnidade.Content = produto.Unidades.Sigla;
            lbQuantidade.Content = quant.ToString("N2");
            lbValor_unitario.Content = produto.Valor_unit.ToString("N2");
            lbTotal.Content = (produto.Valor_unit * quant).ToString("N2");
        }
    }
}
