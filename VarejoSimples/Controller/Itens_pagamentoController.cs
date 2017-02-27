using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class Itens_pagamentoController
    {
        private Itens_pagamentoRepository db = null;

        public Itens_pagamentoController()
        {
            db = new Itens_pagamentoRepository();
        }

        bool auto_commit = true;

        public  bool Save(Itens_pagamento ip)
        {
            try
            {
                ip.Id = db.NextId(e => e.Id);
                db.Save(ip);
                db.Commit();

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        internal void SetContext(varejo_config v)
        {
            db.Context = v;
        }
    }
}
