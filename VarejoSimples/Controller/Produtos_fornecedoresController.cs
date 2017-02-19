using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Enums;
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
                if (!Valid(pf))
                    return false;

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

        private bool Valid(Produtos_fornecedores pf)
        {
            if (pf.Consignado)
            {
                if (pf.Movimento_devolucao == 0)
                {
                    BStatus.Alert("Informe o Tipo de Movimento para devoluções consignadas.");
                    return false;
                }

                if (pf.Movimento_entrada == 0)
                {
                    BStatus.Alert("Informe o Tipo de Movimento para entradas consignadas.");
                    return false;
                }

                Tipos_movimentoController tmc = new Tipos_movimentoController();

                Tipos_movimento tmEntrada = tmc.Find(pf.Movimento_entrada);
                Tipos_movimento tmDevol = tmc.Find(pf.Movimento_devolucao);

                if(tmEntrada.Movimentacao_itens != (int)Tipo_movimentacao.ENTRADA)
                {
                    BStatus.Alert("O Tipo de Movimento para realizar a entrada consignada deve possuir 'Movimentação de Itens = ENTRADA'.");
                    return false;
                }

                if(tmEntrada.Movimentacao_valores != (int) Tipo_movimentacao.NENHUM)
                {
                    BStatus.Alert("O Tipo de Movimento para realizar entrada consignada deve possuir 'Movimentação de Valores = NENHUM'.");
                    return false;
                }

                if(tmDevol.Movimentacao_itens != (int) Tipo_movimentacao.SAIDA)
                {
                    BStatus.Alert("O Tipo de Movimento para realizar devolução consignada deve possuir 'Movimentação de Itens = SAIDA'.");
                    return false;
                }

                if(tmDevol.Movimentacao_valores != (int)Tipo_movimentacao.SAIDA)
                {
                    BStatus.Alert("O Tipo de Movimento para realizar devolução consignada deve possuir 'Movimentação de Valores = SAIDA'.");
                    return false;
                }

                if(pf.Comissao == 0)
                {
                    BStatus.Alert("A comissão deve ser superior a 0");
                }
            }

            if(pf.Fornecedor_id == 0)
            {
                BStatus.Alert("Informe o fornecedor");
                return false;
            }

            if(pf.Produto_id == 0)
            {
                BStatus.Alert("Informe o produto");
                return false;
            }
            
            if(pf.Unidade_id == 0)
            {
                BStatus.Alert("Informe a unidade");
                return false;
            }

            if (pf.Fator_conversao == 0)
                pf.Fator_conversao = 1;

            return true;
        }

        public List<Produtos_fornecedores> Search(string search)
        {
            return db.Where(p =>
                              p.Produtos.Descricao.Contains(search) || 
                              p.Fornecedores.Nome.Contains(search)
                          ).ToList();
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
