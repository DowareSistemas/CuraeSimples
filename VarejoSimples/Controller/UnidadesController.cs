using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class UnidadesController
    {
        private UnidadesRepository db = null;

        public UnidadesController()
        {
            db = new UnidadesRepository();
        }

        public bool Save(Unidades un)
        {
            try
            {
                if (!Valid(un))
                    return false;

                if (db.Find(un.Id) == null)
                {
                    un.Id = db.NextId(u => u.Id);
                    db.Save(un);
                }
                else
                    db.Update(un);
                db.Commit();
                BStatus.Success("Unidade salva");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool Valid(Unidades un)
        {
            if (string.IsNullOrWhiteSpace(un.Sigla))
            {
                BStatus.Alert("A sigla é obrigatória");
                return false;
            }

            if(string.IsNullOrWhiteSpace(un.Nome))
            {
                BStatus.Alert("O nome é obrigatório");
                return false;
            }

            return true;
        }

        public Unidades Find(int id)
        {
            return db.Find(id);
        }

        public bool Remove(int id)
        {
            try
            {
                Unidades un = Find(id);
                if(un.Produtos.Count > 0)
                {
                    BStatus.Alert("Não é possível excluir esta unidade. Existem um ou mais produtos relacionados a ela");
                    return false;
                }

                db.Remove(un);
                db.Commit();
                BStatus.Success("Unidade removida");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Unidades> Search(string search)
        {
            return db.Where(u => u.Sigla.Contains(search) || u.Nome.Contains(search)).ToList();
        }

        public Unidades Next(int current_id)
        {
            return db.Where(u => u.Id > current_id).FirstOrDefault();
        }

        public Unidades Prev(int current_id)
        {
            return db.Where(u => u.Id < current_id).OrderByDescending(u => u.Id).FirstOrDefault();
        }
    }
}
