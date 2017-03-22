using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class MarcasController
    {
        private MarcasRepository db = null;

        public MarcasController()
        {
            db = new MarcasRepository();
        }

        public bool Save(Marcas m)
        {
            try
            {
                if (!Valid(m))
                    return false;

                if (db.Find(m.Id) == null)
                {
                    m.Id = db.NextId(mar => mar.Id);
                    db.Save(m);
                }
                else
                    db.Update(m);

                db.Commit();
                BStatus.Success("Marca salva");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SetContext(varejo_config c)
        {
            db.Context = c;
        }

        private bool ValidDelete(int id)
        {
            if(new ProdutosController().CountByMarca(id) > 0)
            {
                BStatus.Alert("Não é possível excluir esta marca. Existem um ou mais produtos relacionados a ela");
                return false;
            }

            return true;
        }

        private bool Valid(Marcas m)
        {
            if (string.IsNullOrWhiteSpace(m.Nome))
            {
                BStatus.Alert("O nome de marca é obrigatório");
                return false;
            }

            return true;
        }

        public Marcas Find(int id)
        {
            return db.Find(id);
        }

        public bool Remove(int id)
        {
            try
            {
                if (!ValidDelete(id))
                    return false;

                db.Remove(Find(id));
                db.Commit();
                BStatus.Success("Marca removida");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Marcas Next(int current_id)
        {
            return db.Where(m => m.Id > current_id).FirstOrDefault();
        }

        public Marcas Prev(int current_id)
        {
            return db.Where(m => m.Id < current_id).OrderByDescending(m => m.Id).FirstOrDefault();
        }

        public List<Marcas> Search(string search)
        {
            return db.Where(m => m.Nome.Contains(search)).ToList();
        }
    }
}
