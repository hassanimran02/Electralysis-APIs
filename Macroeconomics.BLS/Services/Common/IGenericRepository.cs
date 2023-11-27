
using Common.BLS.Models.Commons;
using Macroeconomics.DAL.Entities;
using System.Runtime.InteropServices;

namespace Macroeconomics.BLS.Services.Common
{
    public interface IGenericRepository<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {
        Task<IEnumerable<TModel>> GetAllAsync([Optional] EFLamdaExpressions<TEntity> expressions);
        IQueryable<TEntity> GetQueryable();
        Task<TEntity> Save(TEntity entity);
        Task<bool> Update(TEntity entity);
        Task<bool> Delete(TEntity entity);
        public ArgaamNext_IndicatorContext context { get; }
    }
}
