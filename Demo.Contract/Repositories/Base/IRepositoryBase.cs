using Demo.Types.Dtos.Shared;
using System.Linq.Expressions;

namespace Demo.Contract.Repositories.Base
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        void Create(T Entity);
        void BulkCreate(IList<T> EntityList);
         void Update(T Entity);
        void BulkUpdate(IList<T> EntityList);
        void Delete(T Entity);
        void BulkDelete(IList<T> EntityList);
        T Find(params object[] Keys);
        Task SaveChangesAsync();
        void SaveChanges();
        IQueryable<T> FindAllQuerySpec(QuerySpec querySpec);
        Task<bool> Exists(Expression<Func<T, bool>> expression = null);    
    }
}
