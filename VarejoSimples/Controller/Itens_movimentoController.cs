using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class Itens_movimentoController
    {
        private Itens_movimentoRepository db = null;

        public Itens_movimentoController()
        {
            db = new Repository.Itens_movimentoRepository();
        }

        public int CountByProduto(int produto_id)
        {
            return db.Where(i => i.Produto_id == produto_id).Count();
        }
    }
}
