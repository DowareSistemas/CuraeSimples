using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VarejoSimples.Model;

namespace VarejoSimples.Controller
{
    public class UnitOfWork
    {
        public varejo_config Context { get; set; }

        public UnitOfWork()
        {
            Context = new varejo_config();
        }

        public void BeginTransaction(System.Data.IsolationLevel? isoLevel = null)
        {
            if (isoLevel == null)
                Context.Database.BeginTransaction();
            else
                Context.Database.BeginTransaction((System.Data.IsolationLevel)isoLevel);
        }

        public void RollBack()
        {
            Context.Database.CurrentTransaction.Rollback();
        }

        public void Commit()
        {
            Context.Database.CurrentTransaction.Commit();
            Context.SaveChanges();
        }
    }
}
