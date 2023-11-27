using Macroeconomics.BLS.Models.EconomicIndicatorValue;

namespace Macroeconomics.BLS.Services.EconomicIndicatorValues
{
    public interface IEconomicIndicatorValueService
    {
        Task<IEnumerable<EconomicIndicatorValueModel>> GetAllAsync(int fieldId);
        Task<EconomicIndicatorValueModel> GetByIdAync(int fieldId, int EconomicIndicatorValueId);
        Task<int> Save(int fieldId, EconomicIndicatorValueModel data);
        Task<bool> Update(int fiedlId, EconomicIndicatorValueModel data);
        Task<bool> Delete(int fieldId, int EconomicIndicatorValueId);
        Task<IEnumerable<IndicatorsFieldValue>> GetIndicatorsFieldValues(int indicatorId, int year, int fiscalPeriodTypeId, int fiscalPeriodId);
        Task<bool> UpdateEconomicIndicatorValueNotes(int economicIndicatorID, int economicIndicatorFieldID, EconomicIndicatorValuesNotesModel data);
    }
}
