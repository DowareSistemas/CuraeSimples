using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class ProdutosController
    {
        private ProdutosRepository db = null;

        public ProdutosController()
        {
            db = new ProdutosRepository();
        }

        public bool Save(Produtos p)
        {
            try
            {
                if (!Valid(p))
                    return false;

                if (db.Find(p.Id) == null)
                {
                    if (!string.IsNullOrWhiteSpace(p.Referencia))
                        if (db.Where(e => e.Referencia.Equals(p.Referencia)) != null)
                        {
                            BStatus.Alert($"Já existe um produto com a referência '{p.Referencia}'");
                            return false;
                        }

                    p.Id = db.NextId(e => e.Id);
                    db.Save(p);
                }
                else
                    db.Update(p);

                db.Commit();
                BStatus.Success("Produto salvo");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Produtos Find(int id)
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
                BStatus.Success("Produto removido");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool ValidRemove(int id)
        {
            Produtos p = Find(id);

            if (new Itens_movimentoController().CountByProduto(id) > 0)
            {
                BStatus.Alert("Não é possível excluir este produto. Ele ja foi utilizado em um ou mais movimentos");
                return false;
            }

            if (p.Produtos_fornecedores.Count > 0)
            {
                BStatus.Alert("Não é possível excluir este produto. Ele está presente em uma ou mais amarrações Produto x Fornecedor");
                return false;
            }

            return true;
        }

        public Produtos Next(int current_id)
        {
            return db.Where(p => p.Id > current_id).FirstOrDefault();
        }

        public Produtos Prev(int current_id)
        {
            return db.Where(p => p.Id < current_id).OrderByDescending(p => p.Id).FirstOrDefault();
        }

        public Produtos Get(string search)
        {
            Expression<Func<Produtos, bool>> expr = (p =>
            p.Id.ToString().Equals(search) ||
            p.Ean.Equals(search) ||
            p.Referencia.Equals(search));

            return db.Where(expr).FirstOrDefault();
        }

        public List<Produtos> Search(string search)
        {
            Expression<Func<Produtos, bool>> expr = (p =>
                        p.Id.ToString().Equals(search) ||
                        p.Descricao.Contains(search) ||
                        p.Ean.Contains(search) ||
                        p.Referencia.Contains(search));

            return db.Where(expr).ToList();
        }

        public List<Produtos> Where(Expression<Func<Produtos, bool>> query)
        {
            return db.Where(query).ToList();
        }

        public int CountByFabricante(int fabricante_id)
        {
            return db.Where(p => p.Fabricante_id == fabricante_id).Count();
        }

        public int CountByMarca(int marca_id)
        {
            return db.Where(p => p.Marca_id == marca_id).Count();
        }

        private bool Valid(Produtos p)
        {
            if (string.IsNullOrWhiteSpace(p.Descricao))
            {
                BStatus.Alert("A descrição do produto é obrigatória");
                return false;
            }

            if (string.IsNullOrWhiteSpace(p.Ean))
            {
                BStatus.Alert("O EAN é obrigatório");
                return false;
            }

            if (p.Ean.Length > 13)
            {
                BStatus.Alert("O EAN não pode conter mais de 13 caracteres");
                return false;
            }

            if (p.Unidade_id == 0)
            {
                BStatus.Alert("A unidade é obrigatória");
                return false;
            }

            if (string.IsNullOrEmpty(p.Ncm))
            {
                BStatus.Alert("Atenção: o NCM do produto não é obrigatório neste cadastro, porém, este produto não poderá ser incluido em uma NFC-e");
                return true;
            }

            return true;
        }
    }
}
