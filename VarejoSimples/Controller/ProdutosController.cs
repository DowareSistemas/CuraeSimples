﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
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

        public void SetContext(varejo_config v)
        {
            db.Context = v;
        }

        public bool Save(Produtos p)
        {
            try
            {
                db.Begin(System.Data.IsolationLevel.ReadUncommitted);

                if (!Valid(p))
                    return false;
               
                if (db.Find(p.Id) == null)
                {
                    if (!string.IsNullOrWhiteSpace(p.Referencia))
                        if (db.Where(e => e.Referencia.Equals(p.Referencia)).FirstOrDefault() != null)
                        {
                            BStatus.Alert($"Já existe um produto com a referência '{p.Referencia}'");
                            return false;
                        }

                    p.Id = db.NextId(e => e.Id);
                    db.Save(p);

                    Estoque est = new Estoque();
                    est.Produto_id = p.Id;
                    est.Loja_id = UsuariosController.LojaAtual.Id;
                    est.Data_entrada = DateTime.Now;
                    est.Quant = 0;
                    est.Lote = string.Empty;
                    est.Sublote = string.Empty;

                    EstoqueController ec = new EstoqueController();
                    ec.SetContext(db.Context);
                    ec.Save(est);
                }
                else
                    db.Update(p);

                db.Commit();
                BStatus.Success("Produto salvo");
                return true;
            }
            catch (Exception ex)
            {
                db.RollBack();
                return false;
            }
        }

        public Produtos Find(int id)
        {
            return db.Find(id);
        }

        public bool Remove(int id)
        {
            UnitOfWork unitOfWork = null;
            try
            {
                unitOfWork = new UnitOfWork();
                unitOfWork.BeginTransaction();

                db.Context = unitOfWork.Context;

                if (!ValidRemove(id))
                    return false;
               
                EstoqueController ec = new EstoqueController();
                ec.RemoveByProduto(id, unitOfWork);
                
                db.Remove(Find(id));
                unitOfWork.Commit();
                BStatus.Success("Produto removido");
                return true;
            }
            catch(Exception ex)
            {
                unitOfWork.RollBack();
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

            if (p.Produtos_fornecedores != null)
            {
                if (p.Produtos_fornecedores.Count > 0)
                {
                    BStatus.Alert("Não é possível excluir este produto. Ele está presente em uma ou mais amarrações Produto x Fornecedor");
                    return false;
                }
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

        public Estoque Get(string search)
        {
            return new EstoqueController().BuscarEstoqueProduto(search);
        }

        public List<Produtos> Search(string search)
        {
            return db.Search(search);
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
