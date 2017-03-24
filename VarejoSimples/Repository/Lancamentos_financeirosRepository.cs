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
    public class Lancamentos_financeirosRepository : RepositoryImpl<Lancamentos_financeiros>, ILancamentos_financeiros
    {
        internal List<Lancamentos_financeiros> BuscaSimples(int pagina_atual, int numero_registros, int mes, int conta_id)
        {
            Expression<Func<Lancamentos_financeiros, bool>> query = (e => e.Conta_id == conta_id);

            DateTime mes_inicio = new DateTime(DateTime.Now.Year, mes, 1);
            DateTime mes_fim = new DateTime(DateTime.Now.Year, mes, DateTime.DaysInMonth(DateTime.Now.Year, mes));

            query = query.And(e => e.Data >= mes_inicio && e.Data <= mes_fim);

            List<Lancamentos_financeiros> result = Where(query).OrderBy(e => e.Id).Skip(pagina_atual).Take(numero_registros).ToList();
            return result;
        }

        internal int CountBusca(int mes, int conta_id)
        {
            Expression<Func<Lancamentos_financeiros, bool>> query = (e => e.Conta_id == conta_id);

            DateTime mes_inicio = new DateTime(DateTime.Now.Year, mes, 1);
            DateTime mes_fim = new DateTime(DateTime.Now.Year, mes, DateTime.DaysInMonth(DateTime.Now.Year, mes));

            query = query.And(e => e.Data >= mes_inicio && e.Data <= mes_fim);

            int result = Where(query).OrderBy(e => e.Id).Count();
            return result;
        }
    }
}
