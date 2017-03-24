using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using VarejoSimples.Interfaces;
using VarejoSimples.Model;

namespace VarejoSimples.Repository
{
    public class ProdutosRepository : RepositoryImpl<Produtos>, IProdutos
    {
        internal List<Produtos> Search(string search)
        {
            Expression<Func<Produtos, bool>> expr = (p =>
                        p.Id.ToString().Equals(search) ||
                        p.Descricao.Contains(search) ||
                        p.Ean.Contains(search) ||
                        p.Referencia.Contains(search));

            return Where(expr).ToList();
        }
    }
}
