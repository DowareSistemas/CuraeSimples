using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class FornecedoresController
    {
        private FornecedoresRepository db = null;

        public FornecedoresController()
        {
            db = new FornecedoresRepository();
        }

        public bool Save(Fornecedores f)
        {
            try
            {
                if (!Valid(f))
                    return false;

                if (db.Find(f.Id) == null)
                {
                    f.Id = db.NextId(e => e.Id);
                    db.Save(f);
                }
                else
                    db.Update(f);
                db.Commit();
                BStatus.Success("Fornecedor salvo");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool Valid(Fornecedores f)
        {
            if (string.IsNullOrWhiteSpace(f.Nome))
            {
                BStatus.Alert("O nome do fornecedor é obrigatório");
                return false;
            }

            if (string.IsNullOrWhiteSpace(f.Cnpj))
            {
                BStatus.Alert("O CNPJ do fornecedor é obrigatório");
                return false;
            }

            if (string.IsNullOrWhiteSpace(f.Uf))
            {
                BStatus.Alert("A UF do fornecedor é obrigatória");
                return false;
            }

            return true;
        }

        public Fornecedores Find(int id)
        {
            return db.Find(id);
        }

        public Fornecedores Next(int current_id)
        {
            return db.Where(f => f.Id > current_id).FirstOrDefault();
        }

        public Fornecedores Prev(int current_id)
        {
            return db.Where(f => f.Id < current_id).OrderByDescending(f => f.Id).FirstOrDefault();
        }

        public List<Fornecedores> Search(string search)
        {
            return db.Where(f =>
                       f.Nome.Contains(search) ||
                       f.Uf.Contains(search) ||
                       f.Municipio.Contains(search) ||
                       f.Cnpj.Contains(search)).ToList();
        }
        
        public bool Remove(int id)
        {
            Fornecedores f = Find(id);
            if (f.Produtos_fornecedores.Count > 0)
            {
                BStatus.Alert("Não é possível excluir este fornecedor. Ele está presente em uma ou mais amarrações Produto X Fornecedor");
                return false;
            }

            db.Remove(f);
            db.Commit();
            BStatus.Success("Fornecedor removido");

            return true;
        }

        internal bool Existe(string cnpj)
        {
            return db.Where(e => e.Cnpj.Equals(cnpj)).Count() > 0;
        }
    }
}
