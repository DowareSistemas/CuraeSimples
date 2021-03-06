﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using VarejoSimples.Enums;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class Formas_pagamentoController
    {
        private Formas_pagamentoRepository db = null;

        public Formas_pagamentoController()
        {
            db = new Repository.Formas_pagamentoRepository();
        }

        public bool Save(Formas_pagamento pg)
        {
            try
            {
                if (!Valid(pg))
                    return false;

                if (db.Find(pg.Id) == null)
                {
                    pg.Id = db.NextId(e => e.Id);
                    db.Save(pg);
                }
                else
                    db.Update(pg);

                db.Commit();
                BStatus.Success("Forma de pagamento salva");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool Valid(Formas_pagamento pg)
        {
            if (string.IsNullOrWhiteSpace(pg.Descricao))
            {
                BStatus.Alert("A descrição da forma de pagamento é obrigatória");
                return false;
            }

            if (pg.Tipo_pagamento == (int)Tipo_pagamento.CREDITO)
            {
                if (pg.Tipo_intervalo == (int)Tipo_intervalo.DATA_BASE)
                    if ((pg.Dia_base > 31) || (pg.Dia_base < 1))
                    {
                        BStatus.Alert("O dia base não pode ser menor que 1 ou passar de 31 ");
                        return false;
                    }

                if (pg.Tipo_intervalo == (int)Tipo_intervalo.INTERVALO)
                    if (pg.Intervalo < 1)
                    {
                        BStatus.Alert("O intervalo não pode ser inferior a 1");
                        return false;
                    }
            }

            if (pg.Tipo_pagamento == (int)Tipo_pagamento.CARTAO)
            {
                if (pg.Operadora_cartao_id == 0)
                {
                    BStatus.Alert("Informe a operadora de cartão");
                    return false;
                }
            }

            if (pg.Tipo_pagamento == (int)Tipo_pagamento.CHEQUE)
            {
                if (pg.Conta_id == 0)
                {
                    BStatus.Alert("Uma conta do tipo BANCÁRIA é necessária para o tipo pagamento CHEQUE");
                    return false;
                }

                Contas conta = new ContasController().Find(pg.Conta_id);
                if (conta.Tipo != (int)Tipo_conta.CONTA_BANCARIA)
                {
                    BStatus.Alert("Uma conta do tipo BANCÁRIA é necessária para o tipo pagamento CHEQUE");
                    return false;
                }
            }

            return true;
        }

        internal Formas_pagamento Get(Expression<Func<Formas_pagamento, bool>> query)
        {
            return db.Where(query).FirstOrDefault();
        }

        public Formas_pagamento Find(int id)
        {
            return db.Find(id);
        }

        public bool ExisteCondicaoPagamentoTipo(Tipo_pagamento tipo_pg)
        {
            int tipo_pag = (int)tipo_pg;
            return (db.Where(e => e.Tipo_pagamento == tipo_pag).Count() > 0);
        }

        public bool Remove(int id)
        {
            try
            {
                Formas_pagamento fp = Find(id);
                if (fp.Movimentos_caixas.Count > 0)
                {
                    BStatus.Alert("Não é possível excluir esta condição de pagamento. Ela está presente em uma ou mais movimentações de caixa");
                    return false;
                }

                if (fp.Itens_pagamento.Count > 0)
                {
                    BStatus.Alert("Não é possível excluir esta condição de pagamento. Ela está presente em um ou mais movimentos");
                    return false;
                }

                if (fp.Pagamentos_lancamentos.Count > 0)
                {
                    BStatus.Alert("Não é possível excluir esta condição de pagamento. Ela está presente em um ou mais lançamentos financeiros");
                    return false;
                }

                db.Remove(fp);
                db.Commit();
                BStatus.Success("Forma de pagamento removida");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Formas_pagamento> Search(string search)
        {
            return db.Where(f => f.Descricao.Contains(search)).ToList();
        }

        public Formas_pagamento Next(int current_id)
        {
            return db.Where(f => f.Id > current_id).FirstOrDefault();
        }

        public Formas_pagamento Prev(int current_id)
        {
            return db.Where(f => f.Id < current_id).OrderByDescending(f => f.Id).FirstOrDefault();
        }

        internal void SetContext(varejo_config v)
        {
            db.Context = v;
        }
    }
}
