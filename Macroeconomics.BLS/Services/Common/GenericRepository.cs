
using Common.BLS.Helper;
using Macroeconomics.DAL.Entities;
using Macroeconomics.BLS.Exceptions;
using Microsoft.EntityFrameworkCore;
using Common.BLS.Models.Commons;
using System.Runtime.InteropServices;

namespace Macroeconomics.BLS.Services.Common
{

    public class GenericRepository<TEntity, TModel> : IGenericRepository<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {

        private readonly ArgaamNext_IndicatorContext _context;
        protected DbSet<TEntity> table => _context.Set<TEntity>();

        public GenericRepository(ArgaamNext_IndicatorContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TModel>> GetAllAsync([Optional] EFLamdaExpressions<TEntity> expressions)
        {
            expressions = expressions ?? new EFLamdaExpressions<TEntity>();
            var query = table.AsNoTracking();
            foreach (var expression in expressions.Includes)
            {
                query = query.Include(expression);
            }
            if (expressions.OrderBys.Any())
            {
                query = expressions.OrderBys.Aggregate(query.OrderBy(x => true), (current, order) => current.ThenBy(order));
            }
            if (expressions.OrderByDescs.Any())
            {
                query = expressions.OrderBys.Aggregate(query.OrderByDescending(x => true), (current, order) => current.ThenBy(order));
            }
            foreach (var whereStatement in expressions.Wheres)
            {
                if (whereStatement != null)
                {
                    query = query.Where(whereStatement);
                }
            }
            var res = await query.ToListAsync();
            return MapperHelper.MapList<TModel, TEntity>(res);
        }
        public IQueryable<TEntity> GetQueryable()
        {
            return table.AsQueryable();
        }
        public async Task<TEntity> Save(TEntity entity)
        {
            try
            {
                table.Add(entity);
                await SaveChangesAync();
                return entity;
            }
            catch(Exception e)
            {
                throw new DatabaseException($"Unable to save ", e.InnerException);
            }
        }
        public async Task<bool> Update(TEntity entity)
        {
            try
            {
                table.Update(entity);
                await SaveChangesAync();
                return true;
            }
            catch (Exception e)
            {
                throw new DatabaseException($"Unable to save ", e.InnerException);
            }
        }
        public async Task<bool> Delete(TEntity entity)
        {
            try
            {
                table.Remove(entity);
                await SaveChangesAync();
                return true;
            }
            catch (Exception e)
            {
                throw new DatabaseException($"Unable to delete ", e.InnerException);
            }
        }
        public Task SaveChangesAync()
        {
            return _context.SaveChangesAsync();
        }


        public ArgaamNext_IndicatorContext context
        {
            get
            {
                return _context;
            }
        }
    }
}
