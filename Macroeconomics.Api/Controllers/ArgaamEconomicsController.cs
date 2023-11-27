//using Macroeconomics.BLS.Services.ArgaamEconomics;
using Macroeconomics.BLS.Services.ArgaamEconomics;
using Macroeconomics.BLS.Services.ChartsEconomics;
using Macroeconomics.BLS.Services.EconomicIndicatorFields;
using Macroeconomics.BLS.Services.EconomicIndicatorValues;
using Macroeconomics.BLS.Services.MacroEconomics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Macroeconomics.Api.Controllers
{
    [Route("api/v{version:apiVersion}/json/argaam")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ArgaamEconomicsController : BaseController
    {
        private readonly IArgaamEconomicsService _service;
        private JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null
        };

        #region Argaam EconomicIndicatorData Old Endpoints
       
        public ArgaamEconomicsController(IArgaamEconomicsService service)
        {
            _service = service;
        }

        [HttpGet("ui/argaam-index")]
        public async Task<IActionResult> GetAllDataForIndexPage(int countryId, int? parentGroupId, int? subGroupId, int? fiscalPeriodTypeId, int? year, bool? isVertical = false)
        {
            return new JsonResult(await _service.GetAllDataForIndexPage(countryId, parentGroupId, subGroupId, fiscalPeriodTypeId, year, isVertical));
        }
        [HttpGet("ui/chart-data-indicator-values")]
        public async Task<IActionResult> ChartDataForEconomicIndicator(int fieldId, int subGroupId,int fromYear)
        {
            return new JsonResult(await _service.GetChartDataForEconomicIndicator(fieldId, subGroupId, fromYear));
        }
        [HttpGet("ui/group-fiscalperiodtypes")]
        public async Task<IActionResult> GroupFiscalPeriodTypes(int subGroupId)
        {
            return new JsonResult(await _service.GetGroupFiscalPeriodTypes(subGroupId));
        }
        [HttpGet("ui/group-available-years")]
        public async Task<IActionResult> GroupYears(int countryId, int? subGroupId = null, int? fiscalPeriodTypeId = null)
        {
            return new JsonResult(await _service.GetGroupYears(countryId, subGroupId, fiscalPeriodTypeId));
        }
        [HttpGet("ui/sector-groups")]
        public async Task<IActionResult> GetGroupsBySector(int sectorId, string tourismKey)
        {
            return new JsonResult(await _service.GetGroupsBySector(sectorId, tourismKey));
        }
        #endregion
    }
}
