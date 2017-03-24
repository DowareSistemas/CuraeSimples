using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class Pedidos_vendaController
    {
        private Pedidos_vendaRepository db = null;
        private Pedidos_venda Pedido { get; set; }

        public Pedidos_vendaController()
        {
            db = new Pedidos_vendaRepository();
            Pedidos_venda pv = new Pedidos_venda();
        }

        public int FecharPedido()
        {
            List<Itens_pedido> Itens_pedido = Pedido.Itens_pedido.ToList();
            Pedido.Itens_pedido = null;

            UnitOfWork unit = null;
            try
            {
                unit = new UnitOfWork();
                unit.BeginTransaction();
                db.Context = unit.Context;

                Pedido.Id = db.NextId(e => e.Id);
                Pedido.Data = DateTime.Now;
                db.Save(Pedido);

                Itens_pedidoController itens_Controller = new Itens_pedidoController();
                itens_Controller.SetContext(unit.Context);

                foreach (Itens_pedido item in Itens_pedido)
                {
                    item.Pedido_id = Pedido.Id;
                    if (!itens_Controller.Save(item))
                    {
                        unit.RollBack();
                        return 0;
                    }
                }

                unit.Commit();
                BStatus.Success("Pedido de venda salvo");
                return Pedido.Id;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        internal List<Pedidos_venda> Search(string text)
        {
            return db.Search(text);
        }

        public Itens_pedido ConvertToItemPedido(Itens_movimento item_movimento)
        {
            Itens_pedido itemPedido = new Itens_pedido();
            itemPedido.Produto_id = item_movimento.Produto_id;
            itemPedido.Lote = item_movimento.Lote;
            itemPedido.Sublote = item_movimento.Sublote;
            itemPedido.Quant = item_movimento.Quant;
            itemPedido.Valor_unit = item_movimento.Valor_unit;
            itemPedido.Aliquota = item_movimento.Aliquota;
            itemPedido.Desconto = item_movimento.Desconto;
            itemPedido.Acrescimo = item_movimento.Acrescimo;
            itemPedido.Frete = item_movimento.Frete;
            itemPedido.Outros_valores = item_movimento.Outros_valores;
            itemPedido.Valor_final = item_movimento.Valor_final;
            itemPedido.Unidade_id = item_movimento.Unidade_id;
            itemPedido.Grade_id = item_movimento.Grade_id;
            itemPedido.Cfop = item_movimento.Cfop;

            return itemPedido;
        }

        public void AbrePedido(int cliente_id)
        {
            Pedido = new Pedidos_venda();
            Pedido.Itens_pedido = new List<Itens_pedido>();
            Pedido.Usuario_id = UsuariosController.UsuarioAtual.Id;
            Pedido.Cliente_id = cliente_id;
        }

        public void AdicionaItem(Itens_pedido item)
        {
            int id = (Pedido.Itens_pedido.OrderByDescending(e => e.Id).FirstOrDefault() == null
                ? 1
                : Pedido.Itens_pedido.OrderByDescending(e => e.Id).FirstOrDefault().Id + 1);

            item.Id = id;

            Itens_pedido itemExistente = Pedido.Itens_pedido.FirstOrDefault(e =>
            e.Produto_id == item.Produto_id &&
            (e.Lote == item.Lote && e.Sublote == item.Sublote) &&
            e.Grade_id == item.Grade_id);

            if (itemExistente != null)
            {
                decimal valor_item = (itemExistente.Valor_final / itemExistente.Quant);
                itemExistente.Quant += item.Quant;
                itemExistente.Valor_final += (valor_item * item.Quant);
            }
            else
                Pedido.Itens_pedido.Add(item);
        }

        public void RemoveItem(int item_id)
        {
            Itens_pedido imv = Pedido.Itens_pedido.Where(e => e.Id == item_id).FirstOrDefault();
            if (imv == null)
                return;

            Pedido.Itens_pedido.Remove(imv);

            int id = 0;
            foreach (Itens_pedido item in Pedido.Itens_pedido)
                item.Id = (id += 1);
        }

        internal void SetContext(varejo_config context)
        {
            db.Context = context;
        }

        internal void RemovePedido(int pedido_venda_id)
        {
            Pedidos_venda pedido = Find(pedido_venda_id);

            Itens_pedidoController itensPedidoController = new Itens_pedidoController();
            itensPedidoController.SetContext(db.Context);
            itensPedidoController.RemoveByPedido(pedido_venda_id);

            db.Remove(pedido);
            db.Commit();
        }

        private Pedidos_venda Find(int pedido_venda_id)
        {
            return db.Find(pedido_venda_id);
        }
    }
}
