using Macroeconomics.BLS.Models.EconomicIndicatorField;
using Macroeconomics.BLS.Models.EconomicIndicatorSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macroeconomics.BLS.Services.EconomicIndicatorSources
{
    public interface IEconomicIndicatorSourceService
    {
        Task<IEnumerable<EconomicIndicatorSourceModel>> GetAllAsync();
        Task<EconomicIndicatorSourceModel> GetByIdAync(int sourceId);
        Task<int> Save(EconomicIndicatorSourceModel data);
        Task<bool> Update(EconomicIndicatorSourceModel data);
        Task<bool> Delete(int sourceId);
    }
}
