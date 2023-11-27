using Macroeconomics.BLS.Models.EconomicIndicatorGroup;
using Macroeconomics.DAL.Entities;
using System.Linq.Expressions;
using Macroeconomics.DAL.Petapoco;

namespace Macroeconomics.BLS.Services.EconomicIndicatorGroups
{
    public interface IEconomicIndicatorGroupService
    {
        Task<IEnumerable<EconomicIndicatorGroupModel>> GetAllAsync(int? parentGroupId = null, bool? includeParentGroups = false);
        Task<EconomicIndicatorGroupModel> GetByIdAync(int id);
        Task<int> Save(EconomicIndicatorGroupModel data);
        Task<bool> Update(EconomicIndicatorGroupModel data);
        Task<bool> Delete(int id);
        Task<IEnumerable<Macroeconomics.DAL.Petapoco.TableModels.EconomicIndicatorGroup>> GetAllIndicatorGroups();
    }

}
