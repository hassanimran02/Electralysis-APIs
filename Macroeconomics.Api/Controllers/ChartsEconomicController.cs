using Macroeconomics.BLS.Services.ChartsEconomics;
using Macroeconomics.BLS.Services.EconomicIndicatorGroups;
using Macroeconomics.BLS.Services.EconomicIndicatorValues;
using Macroeconomics.BLS.Services.MacroEconomics;
using Macroeconomics.BLS.Models.EconomicIndicatorValue;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text.Json;

namespace Macroeconomics.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/json/charts", Order = 2)]
    [ApiVersion("1.0")]
    public class ChartsEconomicController : ControllerBase
    {
        private readonly IChartsEconomicsService _service;

        private JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null
        };
        public ChartsEconomicController(IChartsEconomicsService service)
        {
            _service = service;
        }

        [HttpGet("ui/macro-economic-data/{indicatorFieldIDs}/{fiscalPeriodTypeID}/{fromYear}/{toYear}/{lang}/{companyid}")]
        public async Task<JsonResult> MacroEconomicData(string indicatorFieldIDs, int fiscalPeriodTypeID, int fromYear, int toYear, int lang, int companyid)
        {
            var res = await _service.MacroEconomicData(indicatorFieldIDs, fiscalPeriodTypeID, fromYear, toYear, lang, companyid);
            return new JsonResult(res, serializerOptions);
        }

        [HttpGet("macro-margin-daily-trading-volume/{fromYear}/{toYear}/{lang}/{fiscalPeriodType}")]
        public async Task<JsonResult> MargintoDailyTradingValueData(short fromYear, short toYear, int fiscalPeriodType, int lang)
        {
            var res = await _service.MargintoDailyTradingValueData(fromYear, toYear, fiscalPeriodType, lang);
            return new JsonResult(res, serializerOptions);
        }

        [HttpGet("macro-trading-value/{fromYear}/{toYear}/{lang}/{indicatorFieldIDs}")]
        public async Task<JsonResult> Macro_Charts_GetMarketTradingValue(short fromYear, short toYear, int lang, string indicatorFieldIDs)
        {
            var res = await _service.MacroChartsGetMarketTradingValue(fromYear, toYear, lang, indicatorFieldIDs);
            return new JsonResult(res, serializerOptions);
        }

        [HttpGet("ui/bank-assets-by-gdp-current/{lang}/{fromYear}/{toYear}/{fiscalPeriodTypeID}/{attributeIds}/{indicatorFieldIds}")]
        public async Task<JsonResult> GetBankAssetsByGDPCurrent(int lang, int fromYear, int toYear, int fiscalPeriodTypeID, string attributeIds, string indicatorFieldIds)
        {
            var res = await _service.GetBankAssetsByGDPCurrent(lang, fromYear, toYear, fiscalPeriodTypeID, attributeIds, indicatorFieldIds);
            return new JsonResult(res, serializerOptions);
        }

        [HttpGet("ui/charts-import-per-capita/{lang}/{fromYear}/{toYear}/{fiscalPeriodTypeID}/{indicatorFieldIds}")]
        public async Task<JsonResult> ChartsImportPerCapita(int lang, int fromYear, int toYear, int fiscalPeriodTypeID, string indicatorFieldIds)
        {
            var res = await _service.ChartsImportPerCapita(lang, fromYear, toYear, fiscalPeriodTypeID, indicatorFieldIds);
            return new JsonResult(res, serializerOptions);
        }
        
        [HttpGet("sama-prices/{attributesIds}/{fromYear}/{toYear}/{lang}/{fiscalPeriodType}/{field}/{source}")]
        public async Task<JsonResult> SamaPrices(string attributesIds, short fromYear, short toYear, int lang, int fiscalPeriodType, string field = "", string source = "")
        {
            var res = await _service.SamaAttributeFieldPrices(attributesIds, fromYear, toYear, fiscalPeriodType, lang, field, source);
            return new JsonResult(res, serializerOptions);
        }
        [HttpGet("trading-data/{attributesIds}/{fromYear}/{toYear}/{lang}")]
        public async Task<JsonResult> TradingData(string attributesIds, short fromYear, short toYear, int lang)
        {
            var res = await _service.TradingData(attributesIds, fromYear, toYear, lang);
            return new JsonResult(res, serializerOptions);
        }

        [HttpPost("set-trading-data")]
        public async Task<IActionResult> TradingData()
        {
            await _service.Charts_INSERT_DATA_INTO_MarketTradingDataTools();
            return NoContent();
        }
    }
}
