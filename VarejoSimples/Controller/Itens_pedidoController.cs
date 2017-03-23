using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class Itens_pedidoController
    {
        private Itens_pedidoRepository db = null;

        public Itens_pedidoController()
        {
            db = new Itens_pedidoRepository();
        }

        public void SetContext(varejo_config context)
        {
            db.Context = context;
        }

        public bool Save(Itens_pedido item)
        {
            try
            {
                db.Save(item);
                db.Commit();
                BStatus.Success("Item adicionado ao pedido");
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
