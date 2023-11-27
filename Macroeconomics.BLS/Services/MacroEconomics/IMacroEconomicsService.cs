using ArgaamPlus.BLS.Models.SpModel;
using Macroeconomics.BLS.Models.Charts;
using Macroeconomics.BLS.Models.EconomicIndicatorValue;
using Macroeconomics.BLS.Models.MacroEconomics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macroeconomics.BLS.Services.MacroEconomics
{
    public interface IMacroEconomicsService
    {
        Task<dynamic> GetIndicatorsFieldValue(int indicatorId, int year, int fiscalPeriodTypeId, int fiscalPeriodId);
        Task<List<Macro_Charts_GetMarketTurnOverDailyDataModel>> PP_Indicator_FieldData(int lang, int fromYear, int toYear, List<int> indicatorFieldIDs, int fiscalPeriodTypeID, string entityName);
        Task<dynamic> GetMacroComparableIndicatorsFieldData(int lang, int fromYear, int toYear, string indicatorFieldIDs, int fiscalPeriodTypeID);
        Task<dynamic> GetMacroComparableIndicatorsFieldData2(int lang, int fromYear, int toYear, string indicatorFieldIDs, int usePie, int fiscalPeriodTypeID);
        Task<dynamic> GetIndicatorFieldDataForFormula(int lang, int fromYear, int toYear, int indicatorFormulaId, int fiscalPeriodTypeID);
        Task<dynamic> GetIndicatorsFreeFloatIndex();
    }
}
