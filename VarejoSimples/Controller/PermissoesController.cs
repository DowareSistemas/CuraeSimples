using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class PermissoesController
    {
        private PermissoesRepository db = null;

        public PermissoesController()
        {
            db = new PermissoesRepository();
        }

        public bool Save(Permissoes p)
        {
            try
            {
                if (db.Find(p.Id) == null)
                {
                    p.Id = db.NextId(e => e.Id);
                    db.Save(p);
                }
                else
                    db.Update(p);

                db.Commit();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void DeleteByUsuario(int usuario_id, varejo_config context)
        {
            db.Context = context;
            List<Permissoes> list = db.Where(e => e.Usuario_id == usuario_id).ToList();
            list.ForEach(e => db.Remove(e));
        }

        public Permissoes Find(int id)
        {
            return db.Find(id);
        }
    }
}
