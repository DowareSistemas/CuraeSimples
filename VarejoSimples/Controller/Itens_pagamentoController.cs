using System;
using System.Collections.Generic;
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

        public  bool Save(Itens_pagamento ip, varejo_config context)
        {
            try
            {
                db.Context = context;
                ip.Id = db.NextId(e => e.Id);
                db.Save(ip);

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
