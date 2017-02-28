using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using VarejoSimples.Model;
using VarejoSimples.Repository;

namespace VarejoSimples.Controller
{
    public class ParcelasController
    {
        private ParcelasRepository db = null;

        public ParcelasController ()
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
            catch(Exception ex)
            {
                return false;
            }
        }
        
        public void SetContext(varejo_config v)
        {
            db.Context = v;
        }
    }
}
