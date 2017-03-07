using System;
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
    public class ParcelasController
    {
        private ParcelasRepository db = null;

        public ParcelasController()
        {
            db = new ParcelasRepository();
        }

        private bool auto_commit = true;

        public bool Save(Parcelas p)
        {
            try
            {
                if (db.Find(p.Id) == null)
                {
                    p.Id = db.NextId(e => e.Id);
                    db.Save(p);
                }
                else
                    db.Update(p);

                if (auto_commit)
                    db.Commit();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Parcelas Find(int parcela_id)
        {
            return db.Find(parcela_id);
        }

        public void SetContext(varejo_config v)
        {
            db.Context = v;
        }

        public varejo_config GetContext()
        {
            return db.Context;
        }

        public bool ExisteParcelaAnterior(int parcela_atual)
        {
            return (db.Where(e => e.Parcela_anterior == parcela_atual).Count() > 0);
        }

        public List<Parcelas> ListByPagamentosLancamento(List<Pagamentos_lancamentos> itens)
        {
            if (itens == null)
                return new List<Parcelas>();
            if (itens.Count == 0)
                return new List<Parcelas>();

            var itens_pg = new int[itens.Count];
            for (int i = 0; i < itens.Count; i++)
                itens_pg[i] = itens[i].Id;

            return db.Where(e => itens_pg.Contains(e.Pagamento_lancamento_id)).ToList();

        }

        internal List<Parcelas> ListByItens_pagamento(List<Itens_pagamento> itens_pagamento)
        {
            if (itens_pagamento == null)
                return new List<Parcelas>();
            if (itens_pagamento.Count == 0)
                return new List<Parcelas>();

            var itens_pg = new int[itens_pagamento.Count];
            for (int i = 0; i < itens_pagamento.Count; i++)
                itens_pg[i] = itens_pagamento[i].Id;

            return db.Where(e => itens_pg.Contains(e.Item_pagamento_id)).ToList();
        }

        public List<Parcelas> BuscaBasica(Tipo_parcela tipo,
            int pagina_atual,
            int numero_registros, int mes)
        {
            DateTime mes_inicio = new DateTime(DateTime.Now.Year, mes, 1);
            DateTime mes_fim = new DateTime(DateTime.Now.Year, mes, DateTime.DaysInMonth(DateTime.Now.Year, mes));

            int tipo_parcela = (int)tipo;
            return db.Where(e =>
                    e.Tipo_parcela == tipo_parcela &&
                    e.Data_vencimento >= mes_inicio &&
                    e.Data_vencimento <= mes_fim)
               .OrderBy(e => e.Data_vencimento)
               .OrderBy(e => e.Situacao)
               .Skip(pagina_atual).Take(numero_registros).ToList();
        }

        public int CountBusca(Tipo_parcela tipo, int mes)
        {
            DateTime mes_inicio = new DateTime(DateTime.Now.Year, mes, 1);
            DateTime mes_fim = new DateTime(DateTime.Now.Year, mes, DateTime.DaysInMonth(DateTime.Now.Year, mes));

            int tipo_parcela = (int)tipo;
            int retorno = db.Where(e =>
                    e.Tipo_parcela == tipo_parcela &&
                    e.Data_vencimento >= mes_inicio &&
                    e.Data_vencimento <= mes_fim).Count();

            return retorno;
        }
    }
}
