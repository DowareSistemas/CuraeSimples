using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using VarejoSimples.Model;
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

        public void SetContext(varejo_config v)
        {
            db.Context = v;
        }

        public bool Save(Itens_movimento item)
        {
            try
            {
                db.Save(item);
                db.Commit();
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public int CountByProduto(int produto_id)
        {
            return db.Where(i => i.Produto_id == produto_id).Count();
        }

        internal string GetLastLote(bool cod_loja)
        {
            string lote = "";
            Itens_movimento est = db.Where(e => e.Lote != "" && e.Movimentos.Loja_id == UsuariosController.LojaAtual.Id).OrderByDescending(e => e.Lote).FirstOrDefault();

            lote = (est == null
                ? "A00000"
                : est.Lote);

            if (!cod_loja)
                if (lote.StartsWith(UsuariosController.LojaAtual.Id.ToString()))
                    lote = lote.Substring(1);

            return lote;

        }
    }
}
