using AutoMapper;
using Demo.Contract.Repositories.Base;
using Demo.Infrastructure.Configuration;
using Demo.Types.Dtos.Shared;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Demo.BLL.Services.Base
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private readonly DemoDbContext _context;
        private readonly IMapper _mapper;

        protected RepositoryBase(DemoDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }
        protected RepositoryBase(DemoDbContext context)
        {
            this._context = context;
        }

        //Code
        public void SaveChanges()
        {
            this._context.SaveChanges();
        }
        public async Task SaveChangesAsync()
        {
            await this._context.SaveChangesAsync();
        }
        public virtual void Create(T Entity)

        {
            this._context.Set<T>().Add(Entity);
        }
        public virtual void Update(T Entity)
        {
            this._context.Set<T>().Update(Entity);
        }
        public void Delete(T Entity)
        {
            this._context.Set<T>().Remove(Entity);
        }
        public IQueryable<T> FindAll()
        {
            return this._context.Set<T>().AsNoTracking();
        }
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return this._context.Set<T>().Where(expression).AsNoTracking();
        }
        public void BulkCreate(IList<T> EntityList)
        {
            this._context.Set<T>().AddRangeAsync(EntityList);
        }
        public void BulkUpdate(IList<T> EntityList)
        {
            this._context.Set<T>().UpdateRange(EntityList);
        }
        public void BulkDelete(IList<T> EntityList)
        {
            this._context.Set<T>().RemoveRange(EntityList);
        }
        public T Find(params object[] Keys)
        {
           return this._context.Set<T>().Find(Keys);
        }

        //https://code-maze.com/paging-aspnet-core-webapi/
        public IQueryable<T> FindAllQuerySpec(QuerySpec querySpec)
        {
            return this._context.Set<T>().Skip((querySpec.pagination.PageNumber - 1) * querySpec.pagination.PageSize).Take(querySpec.pagination.PageSize).AsNoTracking();
        }
        public async Task<bool> Exists(Expression<Func<T, bool>> expression = null)
        {
           if(expression == null)
            {
                return await this._context.Set<T>().AnyAsync();
            }
            return await this._context.Set<T>().Where(expression).AnyAsync();
        }
    }
}
