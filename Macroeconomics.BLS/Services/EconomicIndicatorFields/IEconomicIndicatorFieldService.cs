using Macroeconomics.BLS.Models.EconomicIndicatorField;
using Macroeconomics.DAL.Entities;
using System.Linq.Expressions;

namespace Macroeconomics.BLS.Services.EconomicIndicatorFields
{
    public interface IEconomicIndicatorFieldService
    {
        Task<IEnumerable<EconomicIndicatorFieldModel>> GetAllAsync(int? groupId);
        Task<EconomicIndicatorFieldModel> GetByIdAync(int groupId, short id);
        Task<short> Save(int groupId, EconomicIndicatorFieldModel data);
        Task<bool> Update(int groupId, EconomicIndicatorFieldModel data);
        Task<bool> Delete(int groupId, int id);
    }

}
