using Macroeconomics.BLS.Services.ChartsEconomics;
using Macroeconomics.BLS.Services.EconomicIndicatorFields;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Macroeconomics.Api.Controllers
{
    [Route("api/v{version:apiVersion}/json/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ToolsEconomicsController : BaseController
    {
        private readonly ITooslsEconomicsService _service;
        private JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null
        };
        public ToolsEconomicsController(ITooslsEconomicsService service)
        {
            _service = service;
        }
        [HttpGet("ui/government-budget-piechart")]
        public async Task<IActionResult> ArgaamToolsGovtBudgetPieChart()
        {
            return new JsonResult(await _service.ArgaamToolsGovtBudgetPieChart(), serializerOptions);
        }
        [HttpGet("ui/government-budget")]
        public async Task<IActionResult> ArgaamToolsGovtBudget()
        {
            return new JsonResult(await _service.ArgaamToolsGovtBudget(), serializerOptions);
        }
        [HttpGet("ui/GetMarketQuarterlyAndYearlyTradingDataTransactions/{fiscalPeriodTypeIDs}")]
        public async Task<dynamic> ArgaamToolsGetMarketQuarterlyAndYearlyTradingDataTransactions(string fiscalPeriodTypeIDs)
        {
            return new JsonResult(await _service.ArgaamToolsGetMarketQuarterlyAndYearlyTradingDataTransactions(fiscalPeriodTypeIDs), serializerOptions);
        }
        [HttpGet("ui/TradingInformationGetEconomicIndicatorData")]
        public async Task<dynamic> ArgaamToolsTradingInformationGetEconomicIndicatorData()
        {
            return new JsonResult(await _service.ArgaamToolsTradingInformationGetEconomicIndicatorData(), serializerOptions);
        }
        [HttpGet("ui/OperatingExpendituresStack")]
        public async Task<dynamic> ArgaamToolsOperatingExpendituresStack()
        {
            return new JsonResult(await _service.ArgaamToolsOperatingExpendituresStack(), serializerOptions);
        }
        [HttpGet("ui/EstimatedExpenditureAreasGovtBudgetPieChart")]
        public async Task<dynamic> ArgaamToolsEstimatedExpenditureAreasGovtBudgetPieChart()
        {
            return new JsonResult(await _service.ArgaamToolsEstimatedExpenditureAreasGovtBudgetPieChart(), serializerOptions);
        }
        [HttpGet("ui/MargintoDailyTradingValue")]
        public async Task<dynamic> ArgaamToolsMargintoDailyTradingValue()
        {
            return new JsonResult(await _service.ArgaamToolsMargintoDailyTradingValue(), serializerOptions);
        }
    }
}
