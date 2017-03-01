using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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

        public void SetContext(varejo_config v)
        {
            db.Context = v;
        }

        internal List<Parcelas> ListByItens_pagamento(List<Itens_pagamento> itens_pagamento)
        {
            if (itens_pagamento == null)
                return new List<Parcelas>();
            if (itens_pagamento.Count == 0)
                return new List<Parcelas>();

            int item_id = itens_pagamento[0].Id;
            Expression<Func<Parcelas, bool>> query = (e => e.Item_pagamento_id == item_id);

            for (int i = 1; i < itens_pagamento.Count; i++)
            {
                item_id = itens_pagamento[i].Id;
                query = query.Or(e => e.Item_pagamento_id == item_id);
            }
            return db.Where(query).ToList();
        }
    }
}
