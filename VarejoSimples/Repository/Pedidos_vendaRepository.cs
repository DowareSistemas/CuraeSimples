using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Interfaces;
using VarejoSimples.Model;

namespace VarejoSimples.Repository
{
    public class Pedidos_vendaRepository : RepositoryImpl<Pedidos_venda>, IPedidos_venda
    {
        internal List<Pedidos_venda> Search(string text)
        {
            int id = 0;
            int.TryParse(text, out id);

            decimal valor = 0;
            decimal.TryParse(text, out valor);

            var q = (from pedidos in Context.Pedidos_venda.AsNoTracking()
                     join itens_pedido in Context.Itens_pedido.AsNoTracking() on pedidos.Id equals itens_pedido.Pedido_id

                     where
                     pedidos.Id == id ||
                     pedidos.Clientes.Nome.Contains(text) ||
                     itens_pedido.Produtos.Descricao.Contains(text) ||
                     pedidos.Itens_pedido.Sum(e => e.Valor_final) == valor

                     select pedidos).OrderBy(e => e.Data).Take(50).Distinct().AsEnumerable();

            return q.ToList();
        }
    }
}
