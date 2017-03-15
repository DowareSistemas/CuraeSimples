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
    /// Interação lógica para ItemVendaPdv.xam
    /// </summary>
    public partial class ItemVendaPdv : UserControl
    {
        public ItemVendaPdv()
        {
            InitializeComponent();
        }

        public ItemVendaPdv(Itens_movimento item)
        {
            InitializeComponent();

            lbItem.Content = item.Id.ToString();
            lbDescricao.Content = item.Produtos.Descricao;
            lbCod_prod.Content = item.Produto_id.ToString();
            lbValor_unit.Content = $"{item.Quant} {item.Produtos.Unidades.Sigla} x R${item.Valor_unit.ToString("N2")}";
            lbTotal_item.Content = $"R$ {item.Valor_final.ToString("N2")}";

            if (item.Desconto > 0)
                lbValor_unit.Content += " - " + "R$ " + item.Desconto.ToString("N2");
        }
    }
}
