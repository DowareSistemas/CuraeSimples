using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
   public  class Planos_contasController
    {
        private Planos_contasRepository db = null;

        public Planos_contasController()
        {
            db = new Planos_contasRepository();
        }

        public bool Save(Planos_contas pc)
        {
            try
            {
                if (db.Find(pc.Id) == null)
                {
                    pc.Id = db.NextId(e => e.Id);
                    db.Save(pc);
                }
                else
                    db.Update(pc);

                db.Commit();
                BStatus.Success("Plano de contas salvo.");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Planos_contas Find(int id)
        {
            return db.Find(id);
        }

        public bool Remove(int id)
        {
            Tipos_movimentoController tmc = new Tipos_movimentoController();
            int countTiposMov = tmc.CountByPlano_conta(id);

            if(countTiposMov > 0)
            {
                BStatus.Alert("Não é possível excluir este plano de contas. Existem um ou mais tipos de movimento relacionado a ele.");
                return false;
            }

            db.Remove(Find(id));
            db.Commit();
            BStatus.Success("Plano de contas removido.");
            return true;
        }

        public bool TemFilhos(int plano_pai)
        {
            return db.Where(e => e.Conta_pai == plano_pai).Count() > 0;
        }

        public List<Planos_contas> Search(string search)
        {
            return db.Where(p => p.Descricao.Contains(search)).ToList();
        }

        public List<Planos_contas> GetFilhos(int id)
        {
            return db.Where(p => p.Conta_pai == id).ToList();
        }

        internal Planos_contas Prev(int id)
        {
            return db.Where(p => p.Id < id).OrderByDescending(p => p.Id).FirstOrDefault();
        }

        public Planos_contas Next(int id)
        {
            return db.Where(p => p.Id > id).FirstOrDefault();
        }
    }
}
