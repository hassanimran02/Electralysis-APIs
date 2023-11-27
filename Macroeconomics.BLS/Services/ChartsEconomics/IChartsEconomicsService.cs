using Macroeconomics.BLS.Models.EconomicIndicatorValue;
using Macroeconomics.BLS.Models.MacroEconomics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macroeconomics.BLS.Services.ChartsEconomics
{
    public interface IChartsEconomicsService
    {
        Task<dynamic> MacroEconomicData(string indicatorFieldIDs, int fiscalPeriodTypeID, int fromYear, int toYear, int lang, int companyID);
        Task<dynamic> MargintoDailyTradingValueData(short fromYear, short toYear, int fiscalPeriodType, int lang);
        Task<dynamic> MacroChartsGetMarketTradingValue(short fromYear, short toYear, int lang, string indicatorFieldIDs);
        Task<dynamic> GetBankAssetsByGDPCurrent(int lang, int fromYear, int toYear, int fiscalPeriodTypeID, string attributeIds, string indicatorFieldIds);
        Task<dynamic> ChartsImportPerCapita(int lang, int fromYear, int toYear, int fiscalPeriodTypeID, string indicatorFieldIds);
        Task<dynamic> SamaAttributeFieldPrices(string attributesIds, short fromYear, short toYear, int fiscalPeriodType, int lang, string field, string source);
        Task<dynamic> TradingData(string attributesIds, short fromYear, short toYear, int lang);
        Task Charts_INSERT_DATA_INTO_MarketTradingDataTools();
    }
}
