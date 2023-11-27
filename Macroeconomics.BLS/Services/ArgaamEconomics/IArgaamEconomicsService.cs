
namespace Macroeconomics.BLS.Services.ArgaamEconomics
{
    public interface IArgaamEconomicsService
    {
        Task<EconomicIndicatorApiModel> GetAllDataForIndexPage(int countryId, int? parentGroupId, int? subGroupId, int? fiscalPeriodTypeId, int? year, bool? isVertical = false);
        Task<List<EconomicIndicatorChartEntity>> GetChartDataForEconomicIndicator(int fieldId, int subGroupId, int fromYear);
        Task<List<FiscalFeriodTypeModel>> GetGroupFiscalPeriodTypes(int subGroupId);
        Task<List<int>> GetGroupYears(int countryId, int? subGroupId, int? fiscalPeriodTypeId = null);
        Task<List<IndicatorGroupModel>> GetGroupsBySector(int sectorId, string tourismKey);
    }
}
