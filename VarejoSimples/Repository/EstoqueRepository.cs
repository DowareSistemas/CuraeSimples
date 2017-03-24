using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using VarejoSimples.Controller;
using VarejoSimples.Enums;
using VarejoSimples.Interfaces;
using VarejoSimples.Model;

namespace VarejoSimples.Repository
{
    public class EstoqueRepository : RepositoryImpl<Estoque>, IEstoque
    {
        internal List<Estoque> ListarEstoqueProdutos(string desc_cod_ref, string marca, string fabricante)
        {
            int id = 0;
            int.TryParse(desc_cod_ref, out id);

            var q = (from estoque in Context.Estoque
                     join produtos in Context.Produtos on estoque.Produto_id equals produtos.Id
                     join marcas in Context.Marcas on produtos.Marca_id equals marcas.Id
                     join fabricantes in Context.Fabricantes on produtos.Fabricante_id equals fabricantes.Id

                     where
                     (produtos.Descricao.Contains(desc_cod_ref) ||
                      produtos.Referencia.Contains(desc_cod_ref) ||
                      produtos.Ean.Contains(desc_cod_ref) ||
                      produtos.Id == id) &&
                      marcas.Nome.Contains(marca) &&
                      fabricantes.Nome.Contains(fabricante)

                     select estoque).Take(100).AsEnumerable();

            return q.ToList();
        }

        internal Estoque BuscarEstoqueProduto(string search)
        {
            Expression<Func<Estoque, bool>> expr = (p =>
            (p.Produto_id.ToString().Equals(search) ||
             p.Produtos.Ean.Equals(search) ||
             p.Produtos.Referencia.Equals(search)) &&
            (p.Lote.Equals("")));

            return Where(expr).OrderBy(p => p.Lote).FirstOrDefault();
        }

        internal List<Estoque> ProdutosVencendo(int diasApartirDaDataAtual, Tipo_produto_filtro_validade tipo)
        {
            DateTime data = DateTime.Now.AddDays(diasApartirDaDataAtual);
            Expression<Func<Estoque, bool>> query = (e => e.Data_validade <= data);
            query = query.And(e => e.Loja_id == UsuariosController.LojaAtual.Id);
            query = query.And(e => e.Quant > 0);

            switch (tipo)
            {
                case Tipo_produto_filtro_validade.APENAS_COM_LOTE:
                    query = query.And(e => e.Lote != "");
                    break;

                case Tipo_produto_filtro_validade.APENAS_SEM_LOTE:
                    query = query.And(e => e.Lote == "");
                    break;
            }

            return Where(query).ToList();
        }

        internal List<Estoque> Search(string search, bool considera_lote)
        {
            Expression<Func<Estoque, bool>> query = (
                 e => e.Produtos.Descricao.Contains(search));

            if (considera_lote)
            {
                query = query.Or(e => e.Lote.Equals(search));
                query = query.And(e => e.Lote != "");
            }
            else
                query = query.And(e => e.Lote == string.Empty);

            query = query.And(e => (e.Loja_id == UsuariosController.LojaAtual.Id));
            return Where(query).ToList();
        }
    }
}
