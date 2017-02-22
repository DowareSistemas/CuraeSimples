using System;
using System.Collections.Generic;
using System.Linq;
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

            if (pg.Tipo_pagamento == (int)Tipo_pagamento.CREDITO_CLIENTE)
            {
                if (pg.Tipo_intervalo == (int)Tipo_intervalo.DATA_BASE)
                    if ((pg.Dia_base > 31) || (pg.Dia_base < 1))
                    {
                        BStatus.Alert("O dia base não pode ser menor que 1 ou passar de 31 ");
                        return false;
                    }
                
                if(pg.Tipo_intervalo == (int) Tipo_intervalo.INTERVALO)
                    if(pg.Intervalo < 1)
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

            return true;
        }

        public Formas_pagamento Find(int id)
        {
            return db.Find(id);
        }

        public bool Remove(int id)
        {
            try
            {
                Formas_pagamento fp = Find(id);
                if (fp.Movimentos_caixas.Count > 0)
                {
                    BStatus.Alert("Não é possível excluir esta forma de pagamento. Ela está presente em uma ou mais movimentações de caixa");
                    return false;
                }

                if (fp.Itens_pagamento.Count > 0)
                {
                    BStatus.Alert("Não é possível excluir esta forma de pagamenti. Ela está presente em um ou mais movimentos");
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

        internal void SetContext(varejo_config context)
        {
            db.Context = context;
        }
    }
}
