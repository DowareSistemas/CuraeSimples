using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Interfaces;
using VarejoSimples.Model;

namespace VarejoSimples.Repository
{
    public class ParametrosRepository : RepositoryImpl<Parametros>, IParametros
    {
        public List<Parametros> ListParametrosPagamentosPDV(varejo_config context)
        {
            try
            {
                string sql = "select * from Parametros where nome like 'PDV_F%' order by valor";
                return context.Database.SqlQuery<Parametros>(sql).ToList();
            }
            catch(Exception ex)
            {
                return new List<Parametros>();
            }
        }
    }
}
