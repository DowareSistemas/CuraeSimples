using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using VarejoSimples.Enums;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class Operadoras_cartaoController
    {
        private Operadoras_cartaoRepository db = null;

        public Operadoras_cartaoController()
        {
            db = new Operadoras_cartaoRepository();
        }

        public bool Save(Operadoras_cartao oc)
        {
            try
            {
                if (!Valid(oc))
                    return false;

                if (db.Find(oc.Id) == null)
                {
                    oc.Id = db.NextId(o => o.Id);
                    db.Save(oc);
                }
                else
                    db.Update(oc);
                db.Commit();
                BStatus.Success("Operadora de cartão salva");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool Valid(Operadoras_cartao op)
        {
            if (string.IsNullOrWhiteSpace(op.Nome))
            {
                BStatus.Alert("O nome da operadora é obrigatório");
                return false;
            }

            if (op.Tipo_recebimento == (int)Tipo_recebimento.DIAS)
            {
                if (op.Prazo_recebimento > 31)
                {
                    BStatus.Alert("O prazo de recebimento não pode passar de 31 dias");
                    return false;
                }
            }

            if (op.Prazo_recebimento == (int)Tipo_recebimento.HORAS)
            {
                if (op.Tipo_recebimento > 72)
                {
                    BStatus.Alert("O prazo de recebimento não pode passar de 72 horas");
                    return false;
                }
            }

            return true;
        }

        public Operadoras_cartao Find(int id)
        {
            return db.Find(id);
        }

        public bool Remove(int id)
        {
            try
            {
                db.Remove(Find(id));
                db.Commit();
                BStatus.Success("Operadora removida");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Operadoras_cartao Next(int current_id)
        {
            return db.Where(e => e.Id > current_id).FirstOrDefault();
        }

        public Operadoras_cartao Prev(int current_id)
        {
            return db.Where(e => e.Id < current_id).OrderByDescending(e => e.Id).FirstOrDefault();
        }

        public List<Operadoras_cartao> Search(string search, bool inativo = false)
        {
            Expression<Func<Operadoras_cartao, bool>> query = (e => e.Nome.Contains(search));
            if (!inativo)
                query = query.And(e => e.Inativo == false);

            return db.Where(query).ToList();
        }
    }
}
