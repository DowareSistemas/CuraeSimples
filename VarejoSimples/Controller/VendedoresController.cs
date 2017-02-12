using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class VendedoresController
    {
        private VendedoresRepository db = null;

        public VendedoresController()
        {
            db = new VendedoresRepository();
        }

        public bool Save(Vendedores v)
        {
            try
            {
                if (!Valid(v))
                    return false;

                if (db.Find(v.Id) == null)
                {
                    v.Id = db.NextId(e => e.Id);
                    db.Save(v);
                }
                else
                    db.Update(v);

                db.Commit();
                BStatus.Success("Vendedor salvo");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Vendedores Next(int current_id)
        {
            Vendedores vend = db.Where(e => e.Id > current_id).FirstOrDefault();
            return vend;
        }

        public Vendedores Prev(int current_id)
        {
            Vendedores vend = db.Where(e => e.Id < current_id).OrderByDescending(e => e.Id).FirstOrDefault();
            return vend;
        }

        public int CountByUsuario(int usuario_id)
        {
            return db.Where(e => e.Usuario_id == usuario_id).Count();
        }

        public bool Delete(int vendedor_id)
        {
            try
            {
                Vendedores vend = Find(vendedor_id);
                db.Remove(vend);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public Vendedores Find(int id)
        {
            return db.Find(id);
        }
        
        public List<Vendedores> Search(string search)
        {
            Expression<Func<Vendedores, bool>> expr = (e => 
                            e.Nome.Contains(search) ||
                            e.Apelido.Contains(search));

            int id = 0;
            if (int.TryParse(search, out id))
                expr = expr.And(e => e.Id == int.Parse(search));

            return db.Where(expr).ToList();
        }

        internal List<Vendedores> Get(Expression<Func<Vendedores, bool>> query)
        {
            return db.Where(query).ToList();
        }

        private bool Valid(Vendedores v)
        {
            if(string.IsNullOrWhiteSpace(v.Nome))
            {
                BStatus.Alert("O nome do vendedor é obrigatório");
                return false;
            }

            if(v.Loja_id == 0)
            {
                BStatus.Alert("Informe a loja do vendedor");
                return false;
            }

            if(v.Usuario_id == 0)
            {
                BStatus.Alert("Informe o usuário deste vendedor");
                return false;
            }

            return true;
        }
    }
}
