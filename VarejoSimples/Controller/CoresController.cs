using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class CoresController
    {
        private CoresRepository db = null;

        public CoresController()
        {
            db = new CoresRepository();
        }

        public bool Save(Cores cor)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cor.Descricao))
                {
                    BStatus.Alert("A descrição da cor é obrigatória");
                    return false;
                }

                if (db.Find(cor.Id) == null)
                {
                    cor.Id = db.NextId(e => e.Id);
                    db.Save(cor);
                }
                else
                    db.Update(cor);

                db.Commit();
                BStatus.Success("Cor salva com sucesso");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Cores Find(int id)
        {
            return db.Find(id);
        }

        public bool Remove(int id)
        { 
            try
            {
                Cores cor = Find(id);

                if (cor.Grades_produtos.Count > 0)
                {
                    BStatus.Alert("Não é possível remover esta cor. Ela está presente em uma ou mais grades de produtos.");
                    return false;
                }

                db.Remove(cor);
                db.Commit();
                BStatus.Success("Cor removida");

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Cores> Search(string searchTerm)
        {
            return db.Where(c => c.Descricao.Contains(searchTerm)).ToList();
        }

        public Cores Next(int current_id)
        {
            return db.Where(e => e.Id > current_id).FirstOrDefault();
        }

        public Cores Prev(int current_id)
        {
            return db.Where(e => e.Id < current_id).OrderByDescending(e => e.Id).FirstOrDefault();
        }
    }
}
