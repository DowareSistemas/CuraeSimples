using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class FabricantesController
    {
        private FabricantesRepository db = null;

        public FabricantesController()
        {
            db = new FabricantesRepository();
        }

        public bool Save(Fabricantes fab)
        {
            try
            {
                if (!Valid(fab))
                    return false;

                if (db.Find(fab.Id) == null)
                {
                    fab.Id = db.NextId(f => f.Id);
                    db.Save(fab);
                }
                else
                    db.Update(fab);
                db.Commit();
                BStatus.Success("Fabricante salvo");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool Valid(Fabricantes fab)
        {
            if (string.IsNullOrWhiteSpace(fab.Nome))
            {
                BStatus.Alert("O nome do fabricante é obrigatório");
                return false;
            }

            return true;
        }

        public bool Remove(int id)
        {
            try
            {
                if(new ProdutosController().CountByFabricante(id) > 0)
                {
                    BStatus.Alert("Não é possível excluir este fabricante. Existem um ou mais produtos relacionados a ele");
                    return false;
                }

                db.Remove(Find(id));
                db.Commit();
                BStatus.Success("Fabricante removido");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Fabricantes Find(int id)
        {
            return db.Find(id);
        }

        public Fabricantes Next(int current_id)
        {
            return db.Where(f => f.Id > current_id).FirstOrDefault();
        }

        public Fabricantes Prev(int current_id)
        {
            return db.Where(f => f.Id < current_id).OrderByDescending(f => f.Id).FirstOrDefault();
        }

        public List<Fabricantes> Search(string search)
        {
            return db.Where(f => f.Nome.Contains(search)).ToList();
        }

        public List<Fabricantes> Get(Expression<Func<Fabricantes, bool>> query)
        {
            return db.Where(query).ToList();
        }
    }
}
