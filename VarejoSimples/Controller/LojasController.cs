using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
   public  class LojasController
    {
        private LojasRepository db = null;

        public LojasController()
        {
            db = new LojasRepository();
        }

        public bool Save(Lojas loja)
        {
            try
            {
                if (!Valid(loja))
                    return false;

                if (db.Find(loja.Id) == null)
                {
                    loja.Id = db.NextId(e => e.Id);
                    db.Save(loja);
                }
                else
                    db.Update(loja);

                db.Commit();
                BStatus.Success("Loja salva");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Lojas> Search(string search)
        {
            Expression<Func<Lojas, bool>> expr = (e =>
                      e.Razao_social.Contains(search) ||
                      e.Nome_fantasia.Contains(search) ||
                      e.Cnpj.Equals(search));

            int id = 0;
            if (int.TryParse(search, out id))
                expr = expr.And(e => e.Id == int.Parse(search));

            return db.Where(expr).ToList();
        }

        internal Lojas Find(int v)
        {
            return db.Find(v);
        }

        public Lojas Next(int current_id)
        {
            Lojas loja = db.Where(e => e.Id > current_id).FirstOrDefault();
            return loja;
        }

        public Lojas Prev(int current_id)
        {
            if (current_id <= 0)
                return null;

            Lojas loja = db.Where(e => e.Id < current_id).OrderByDescending(e => e.Id).FirstOrDefault();
            return loja;
        }

        private bool Valid(Lojas loja)
        {
            if(string.IsNullOrWhiteSpace(loja.Nome_fantasia))
            {
                BStatus.Alert("O nome fantasia é obrigatório");
                return false;
            }

            if(string.IsNullOrWhiteSpace(loja.Razao_social))
            {
                BStatus.Alert("A razão social é obrigatória");
                return false;
            }

            if(string.IsNullOrWhiteSpace(loja.Logradouro))
            {
                BStatus.Alert("O logradouro é obrigatório");
                return false;
            }

            if(string.IsNullOrWhiteSpace(loja.Municipio))
            {
                BStatus.Alert("O município é obrigatório");
                return false;
            }

            if(string.IsNullOrWhiteSpace(loja.Bairro))
            {
                BStatus.Alert("O bairro é obrigatório");
                return false;
            }

            if(string.IsNullOrWhiteSpace(loja.Cnpj))
            {
                BStatus.Alert("O CNPJ é obrigatório");
                return false;
            }

            if(string.IsNullOrWhiteSpace(loja.Uf))
            {
                BStatus.Alert("A UF é obrigatória");
                return false;
            }

            return true;
        }
    }
}
