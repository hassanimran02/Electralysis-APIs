using Macroeconomics.BLS.Services.EconomicIndicatorGroups;
using Macroeconomics.BLS.Services.EconomicIndicatorValues;
using Macroeconomics.BLS.Services.MacroEconomics;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Macroeconomics.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/json/macro", Order = 1)]
    [ApiVersion("1.0")]
    public class MacroEconomicController : ControllerBase
    {
        private readonly IMacroEconomicsService _service;
        private readonly IEconomicIndicatorGroupService _groupService;

        private JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null
        };
        public MacroEconomicController(IMacroEconomicsService service, IEconomicIndicatorGroupService groupService, IEconomicIndicatorValueService indicatorService)
        {
            _service = service;
            _groupService = groupService;
        }

        [HttpGet("indicators-groups")]
        public async Task<dynamic> IndicatorsGroups([Optional] int? parentGroupId)
        {
            return await _groupService.GetAllAsync(parentGroupId);
        }
        [HttpGet("indicator-values/{indicatorId}/{year}/{fiscalPeriodTypeId}/{fiscalPeriodId}")]
        public async Task<dynamic> GetIndicatorsFieldValues(int indicatorId, int year, int fiscalPeriodTypeId, int fiscalPeriodId)
        {
            return await _service.GetIndicatorsFieldValue(indicatorId, year, fiscalPeriodTypeId, fiscalPeriodId);
        }

        [HttpGet("indicator-data/{lang}/{fromYear}/{toYear}/{indicatorFieldIDs}/{fiscalPeriodTypeID}")]
        public async Task<JsonResult> GetMacroComparableIndicatorsFieldData(int lang, int fromYear, int toYear, string indicatorFieldIDs, int fiscalPeriodTypeID)
        {
            return new JsonResult(await _service.GetMacroComparableIndicatorsFieldData(lang, fromYear, toYear, indicatorFieldIDs, fiscalPeriodTypeID), serializerOptions);
        }

        [HttpGet("indicator-formula-data/{lang}/{fromYear}/{toYear}/{indicatorFormulaId}/{fiscalPeriodTypeID}")]
        public async Task<JsonResult> GetIndicatorFieldDataForFormula(int lang, int fromYear, int toYear, int indicatorFormulaId, int fiscalPeriodTypeID)
        {
            return new JsonResult(await _service.GetIndicatorFieldDataForFormula(lang, fromYear, toYear, indicatorFormulaId, fiscalPeriodTypeID), serializerOptions);
        }
       
        [HttpGet("indicator-data-2/{lang}/{fromYear}/{toYear}/{indicatorFieldIDs}/{fiscalPeriodTypeID}")]
        public async Task<JsonResult> GetMacroComparableIndicatorsFieldData2(int lang, int fromYear, int toYear, string indicatorFieldIDs, int fiscalPeriodTypeID, bool usePie = false)
        {
            return new JsonResult(await _service.GetMacroComparableIndicatorsFieldData2(lang, fromYear, toYear, indicatorFieldIDs, usePie ? 1 : 0, fiscalPeriodTypeID), serializerOptions);
        }

        [HttpGet("indicators-free-float-index")]
        public async Task<JsonResult> GetIndicatorsFreeFloatIndex()
        {
            return new JsonResult(await _service.GetIndicatorsFreeFloatIndex(), serializerOptions);
        }
    }
}
