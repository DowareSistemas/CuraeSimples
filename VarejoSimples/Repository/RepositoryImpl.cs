using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using VarejoSimples.Controller;
using VarejoSimples.Interfaces;
using VarejoSimples.Model;

namespace VarejoSimples.Repository
{
    public class RepositoryImpl<T> : IRepository<T> where T : class
    {
        private VarejoSimples.Model.varejo_config _context;
        private bool hasTransaction = false;
        public RepositoryImpl()
        {
            _context = new Model.varejo_config();
        }

        public int NextId(Func<T, int> query)
        {
            if (_context.Set<T>().Count() == 0)
                return 1;
            else
                return _context.Set<T>().Max(query) + 1;
        }

        public DbContextTransaction Begin(System.Data.IsolationLevel isoLevel)
        {
            hasTransaction = true;
            return _context.Database.BeginTransaction(isoLevel);
        }

        public varejo_config Context
        {
            get
            {
                return _context;
            }
            set
            {
                _context = value;
            }
        }

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        public bool ExecSQL(string sql, SqlParameter[] parameters = null)
        {
            int retorno = (parameters == null
                ? _context.Database.ExecuteSqlCommand(sql)
                : _context.Database.ExecuteSqlCommand(sql, parameters));

            return (retorno == 1);
        }

        public void RollBack()
        {
            _context.Database.CurrentTransaction.Rollback();
            hasTransaction = false;
        }

        public void Commit()
        {
            try
            {
                _context.SaveChanges();
                if (hasTransaction)
                    _context.Database.CurrentTransaction.Commit();
                hasTransaction = false;
            }
            catch (DbUpdateConcurrencyException concEx)
            {
                BStatus.Alert("A operação foi completada com erros por que o registro foi excluido por outro utilizador.");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException valEx)
            {
                BStatus.ErrorOnSave(_context.Set<T>().GetType().Name, valEx.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage);
                throw;
            }
            catch (Exception ex)
            {
                BStatus.ErrorOnSave(_context.Set<T>().GetType().Name, ex.Message);
                throw;
            }
        }

        public T First(Func<T, bool> query)
        {
            try
            {
                return _context.Set<T>().FirstOrDefault(query);
            }
            catch (Exception ex)
            {
                BStatus.ErrorOnFind(_context.Set<T>().GetType().Name, ex.Message);
            }

            return null;
        }

        public T Find(object id)
        {
            try
            {
                return _context.Set<T>().Find(id);
            }
            catch (Exception ex)
            {
                BStatus.ErrorOnFind(_context.Set<T>().GetType().Name, ex.Message);
            }

            return null;
        }

        public void Remove(T entity)
        {
            try
            {
                _context.Set<T>().Remove(entity);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException valEx)
            {
                BStatus.ErrorOnSave(_context.Set<T>().GetType().Name, valEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                BStatus.ErrorOnRemove(entity.GetType().Name, ex.Message);
                throw;
            }
        }

        public void Save(T entity)
        {
            try
            {
                _context.Set<T>().Add(entity);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException valEx)
            {
                BStatus.ErrorOnSave(_context.Set<T>().GetType().Name, valEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                BStatus.ErrorOnSave(entity.GetType().Name, ex.Message);
                throw;
            }
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> query)
        {
            try
            {
                return _context.Set<T>().Where(query).AsNoTracking();
            }
            catch (Exception ex)
            {
                BStatus.ErrorOnList(_context.Set<T>().GetType().Name, ex.Message);
            }

            return null;
        }

        public void Update(T entity)
        {
            try
            {
                _context.Entry<T>(entity).State = EntityState.Modified;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException valEx)
            {
                BStatus.ErrorOnSave(_context.Set<T>().GetType().Name, valEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                BStatus.ErrorOnSave(entity.GetType().Name, ex.Message);
                throw;
            }
        }
    }
}
