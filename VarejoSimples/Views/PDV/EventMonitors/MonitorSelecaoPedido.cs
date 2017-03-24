using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;

namespace VarejoSimples.Views.PDV.EventMonitors
{
    public class MonitorSelecaoPedido
    {
        public delegate void SelecaoPedido(Pedidos_venda pedido);
        public event SelecaoPedido PedidoSelecionado;

        private static MonitorSelecaoPedido instance = null;

        public static MonitorSelecaoPedido Instance
        {
            get
            {
                if (instance == null)
                    instance = new MonitorSelecaoPedido();

                return instance;
            }
        }

        public void AcionarSelecao(Pedidos_venda pedido)
        {
            if (PedidoSelecionado != null)
                PedidoSelecionado(pedido);
        }
    }
}
