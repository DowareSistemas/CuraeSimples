using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class Pagamentos_lancamentosController
    {
        private Pagamentos_lancamentosRepository db;

        public Pagamentos_lancamentosController()
        {
            db = new Pagamentos_lancamentosRepository();
        }

        public bool Save(Pagamentos_lancamentos pl)
        {
            try
            {
                pl.Id = db.NextId(e => e.Id);
                db.Save(pl);
                db.Commit();

                BStatus.Success("Pagamento do lançamnento efetuado");
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public void SetContext(varejo_config context)
        {
            db.Context = context;
        }
    }
}
