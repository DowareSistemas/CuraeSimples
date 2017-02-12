using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace VarejoSimples.Interfaces
{
    public interface IRepository<T> where T : class
    {
        void Save(T entity);
        void Update(T entity);
        void Remove(T entity);
        void Commit();
        T Find(int id);
        int NextId(Func<T, int> query);
        IQueryable<T> Where(Expression<Func<T, bool>> query);
        DbContextTransaction Begin(System.Data.IsolationLevel isoLevel);
    }
}
