using Macroeconomics.BLS.Models.EconomicIndicatorValue;
using Macroeconomics.BLS.Models.MacroEconomics;
using Macroeconomics.BLS.Models.ToolsEconomics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macroeconomics.BLS.Services.ChartsEconomics
{
    public interface ITooslsEconomicsService
    {
        Task<dynamic> ArgaamToolsPageQuery(ArgaamToolsPageQueryEnum indicatorsType);

        Task<dynamic> ArgaamToolsGovtBudgetPieChart();
        Task<dynamic> ArgaamToolsGovtBudget();
        Task<dynamic> ArgaamToolsGetMarketQuarterlyAndYearlyTradingDataTransactions(string fiscalPeriodTypeIDs);
        Task<dynamic> ArgaamToolsTradingInformationGetEconomicIndicatorData();
        Task<dynamic> ArgaamToolsOperatingExpendituresStack();
        Task<dynamic> ArgaamToolsEstimatedExpenditureAreasGovtBudgetPieChart();
        Task<dynamic> ArgaamToolsMargintoDailyTradingValue();
    }
}
