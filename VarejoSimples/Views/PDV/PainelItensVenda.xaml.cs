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
using VarejoSimples.Interfaces;
using VarejoSimples.Model;

namespace VarejoSimples.Views.PDV
{
    /// <summary>
    /// Interação lógica para PainelItensVenda.xam
    /// </summary>
    public partial class PainelItensVenda : UserControl, IPainelVenda
    {
        private MovimentosController MovimentosController = null;
        private int Movimento_atual { get; set; }
        public PainelItensVenda()
        {
            InitializeComponent();

            MovimentosController = new MovimentosController();
            lbTotalParcial.Content = "R$ 0,00";
        }

        public Clientes GetCliente()
        {
            return new ClientesController().Find(MovimentosController.GetClienteId());
        }

        private void RefreshItens()
        {
            sp_produtos.Children.Clear();
            MovimentosController.Itens_movimento.ForEach(e =>
                sp_produtos.Children.Add(new ItemVendaPdv(e)));

            lbTotalParcial.Content = "R$ " + MovimentosController.GetTotalParcial().ToString("N2");
        }

        public void AbreVenda(int tipo_movimento)
        {
            MovimentosController.AbreMovimento(0, tipo_movimento);
        }

        public decimal GetTotalParcial()
        {
            decimal total = MovimentosController.Itens_movimento.Sum(e => e.Valor_final);
            return total;
        }

        public void VendeItem(Itens_movimento item)
        {
            MovimentosController.AdicionaItem(item);
            RefreshItens();
        }

        public void RemoveItem(int item_id)
        {
            MovimentosController.RemoveItem(item_id);
            RefreshItens();
        }
        
        public void InformaCliente(int cliente_id)
        {
            MovimentosController.InformarCliente(cliente_id);
        }
        
        public void IncrementaItem(int item_id)
        {
            MovimentosController.IncrementaItem(item_id);
            RefreshItens();
        }

        public void DecrementaItem(Estoque estoque)
        {
            MovimentosController.DecrementaItem(estoque);
            RefreshItens();
        }

        public void DecrementaItem(int item_id)
        {
            MovimentosController.DecrementaItem(item_id);
            RefreshItens();
        }

        public void AplicarDescontoItemReais(int item_id, decimal valor)
        {
            MovimentosController.AplicarDescontoReais(item_id, valor);
            RefreshItens();
        }

        public void AplicarDescontoItemPercent(int item_id, decimal valor)
        {
            MovimentosController.AplicarDescontoPerc(item_id, valor);
            RefreshItens();
        }

        public void EfetuarPagamento(int forma_pagamento_id, decimal valor)
        {
            MovimentosController.EfetuaPagamento(forma_pagamento_id, valor);
        }

        public bool Encerrar(decimal troco)
        {
           Movimento_atual =  MovimentosController.FechaMovimento(troco);
            return (Movimento_atual > 0);
        }

        public decimal GetValorParcial()
        {
            return MovimentosController.GetTotalParcial();
        }

        public void NFCe()
        {
            MovimentosController.NFCe();
        }

        public int TransformarEmPedido()
        {
            return MovimentosController.MovimentoParaPedido();
        }

        public void TransformarEmMovimento(Pedidos_venda pedido, int tipo_mov)
        {
            MovimentosController.PedidoParaMovimento(pedido, tipo_mov);
            RefreshItens();
        }

        public bool ClienteInformado
        {
            get
            {
                return MovimentosController.GetClienteId() > 0;
            }
        }

        public UserControl CurrentUserControl
        {
            get
            {
                return this;
            }
        }
    }
}
