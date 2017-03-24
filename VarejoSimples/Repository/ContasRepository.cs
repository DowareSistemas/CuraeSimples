using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using VarejoSimples.Controller;
using VarejoSimples.Interfaces;
using VarejoSimples.Model;

namespace VarejoSimples.Repository
{
    public class ContasRepository : RepositoryImpl<Contas>, IContas
    {
        internal List<Contas> Search(string search, bool inativos)
        {
            Expression<Func<Contas, bool>> query = (e =>
                            e.Nome.Contains(search) ||
                            e.Nome_banco.Contains(search) ||
                            e.Conta.Equals(search) ||
                            e.Id.ToString().Equals(search) ||
                            e.Titular.Contains(search));

            if (!inativos)
                query = query.And(e => e.Inativa == false);

            return Where(query).ToList();
        }
    }
}
