using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class TamanhosController
    {
        private TamanhosRepository db = null;

        public TamanhosController()
        {
            db = new TamanhosRepository();
        }

        public bool Save(Tamanhos tamanho)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tamanho.Descricao))
                {
                    BStatus.Alert("A descrição é obrigatória");
                    return false;
                }

                if (db.Find(tamanho.Id) == null)
                {
                    tamanho.Id = db.NextId(e => e.Id);
                    db.Save(tamanho);
                }
                else
                    db.Update(tamanho);

                db.Commit();
                BStatus.Success("Tamanho salvo");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Tamanhos Find(int id)
        {
            return db.Find(id);
        }

        internal Tamanhos Next(int current_id)
        {
            return db.Where(e => e.Id > current_id).FirstOrDefault();
        }

        public Tamanhos Prev(int current_id)
        {
            return db.Where(e => e.Id < current_id).OrderByDescending(e => e.Id).FirstOrDefault();
        }

        public bool Remove(int id)
        {
            try
            {
                Tamanhos tamanho = Find(id);
                if (tamanho.Grades_produtos.Count > 0)
                {
                    BStatus.Alert("Não é possível remover este tamanho. Ele está relacionado em uma ou mais grades de produtos.");
                    return false;
                }

                db.Remove(tamanho);
                db.Commit();
                BStatus.Success("Tamanho removido");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Tamanhos> Search(string searchTerm)
        {
            return db.Where(e => e.Descricao.Contains(searchTerm)).ToList();
        }
    }
}
