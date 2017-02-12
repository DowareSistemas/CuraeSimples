using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class Movimentos_caixasController
    {
        private Movimentos_caixasRepository db = null;

        public Movimentos_caixasController()
        {
            db = new Movimentos_caixasRepository();
        }

        public bool Save(Movimentos_caixas mc)
        {
            try
            {
                db.Save(mc);
                db.Commit();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public int CountByCaixa(int caixa_id)
        {
            return db.Where(m => m.Caixa_id == caixa_id).Count();
        }
    }
}
