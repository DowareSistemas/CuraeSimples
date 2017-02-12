using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class Produtos_fornecedoresController
    {
        private Produtos_fornecedoresRepository db = null;

        public Produtos_fornecedoresController()
        {
            db = new Produtos_fornecedoresRepository();
        }

        public bool Save(Produtos_fornecedores pf)
        {
            try
            {
                if (db.Find(pf.Id) == null)
                {
                    pf.Id = db.NextId(p => p.Id);
                    db.Save(pf);
                }
                else
                    db.Update(pf);

                db.Commit();
                BStatus.Success("Amarração Produto x Fornecedor salva com sucesso");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Produtos_fornecedores> Search(string search)
        {
            return db.Where(p =>
                          p.Produtos.Descricao.Contains(search) || 
                          p.Fornecedores.Nome.Contains(search))
                          .ToList();
        }

        public Produtos_fornecedores Next(int current_id)
        {
            return db.Where(p => p.Id > current_id).FirstOrDefault();
        }

        public Produtos_fornecedores Prev(int current_id)
        {
            return db.Where(p => p.Id < current_id).OrderByDescending(p => p.Id).FirstOrDefault();
        }

        public Produtos_fornecedores Find(int id)
        {
            return db.Find(id);
        }

        public int CountByProduto(int produto_id)
        {
            return db.Where(p => p.Produto_id == produto_id).Count();
        }

        public int CountByFornecedor(int fornecedor_id)
        {
            return db.Where(p => p.Fornecedor_id == fornecedor_id).Count();
        }

        public bool Remove(int id)
        {
            try
            {
                db.Remove(Find(id));
                db.Commit();
                BStatus.Success("Amaração removida");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
