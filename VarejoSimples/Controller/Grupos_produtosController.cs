using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class Grupos_produtosController
    {
        private Grupos_produtosRepository db = null;

        public Grupos_produtosController()
        {
            db = new Repository.Grupos_produtosRepository();
        }

        public List<Grupos_produtos> Search(string search)
        {
            return db.Where(e => e.Nome.Contains(search)).ToList();
        }
    }
}
