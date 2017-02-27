using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class CaixasController
    {
        private CaixasRepository db = null;

        public varejo_config Context
        {
            get
            {
                return db.Context;
            }
        }

        public CaixasController()
        {
            db = new CaixasRepository();
           
        }

        public DbContextTransaction BeginTx()
        {
            return db.Begin(System.Data.IsolationLevel.ReadUncommitted);
        }

        public bool Save(Caixas caixa)
        {
            try
            {
                if (!Valid(caixa))
                    return false;

                if (db.Find(caixa.Id) == null)
                {
                    caixa.Id = db.NextId(c => c.Id);
                    db.Save(caixa);
                }
                else
                    db.Update(caixa);

                db.Commit();
                BStatus.Success("Caixa salvo");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Caixas Find(int id)
        {
            return db.Find(id);
        }

        public bool Remove(int id)
        {
            try
            {
                if (!ValidRemove(id))
                    return false;

                db.Remove(Find(id));
                db.Commit();
                BStatus.Success("Caixa removido");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool ValidRemove(int caixa_id)
        {
            if(new Movimentos_caixasController().CountByCaixa(caixa_id) > 0)
            {
                BStatus.Alert("Não é possível excluir este caixa. Já foram realizadas uma ou mais movimentações.");
                return false;
            }

            return true;
        }

        private bool Valid(Caixas caixa)
        {
            if(string.IsNullOrWhiteSpace(caixa.Nome))
            {
                BStatus.Alert("O nome do caixa é obrigatório");
                return false;
            }

            return true;
        }

        internal void SetContext(varejo_config c)
        {
            db.Context = c;
        }

        public List<Caixas> Search(string search)
        {
            return db.Where(c => c.Nome.Contains(search)).ToList();
        }

        public Caixas Next(int current_id)
        {
            Caixas caixa = db.Where(c => c.Id > current_id).FirstOrDefault();
            return caixa;
        }

        public Caixas Prev(int current_id)
        {
            Caixas caixa = db.Where(c => c.Id < current_id).OrderByDescending(c => c.Id).FirstOrDefault();
            return caixa;
        }
    }
}
