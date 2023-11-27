using Macroeconomics.BLS.Services.EconomicIndicatorValues;
using Macroeconomics.BLS.Models.EconomicIndicatorValue;
using Microsoft.AspNetCore.Mvc;

namespace Macroeconomics.Api.Controllers
{
    [Route("api/v{version:apiVersion}/json/EconomicIndicatorField/{fieldId}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class EconomicIndicatorValueController : BaseController
    {
        private readonly IEconomicIndicatorValueService _service;

        public EconomicIndicatorValueController(IEconomicIndicatorValueService economicIndicatorValueService) 
        { 
            _service = economicIndicatorValueService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(int fieldId)
        {
            return Ok(await _service.GetAllAsync(fieldId));
        }
        [HttpGet("{valueId}")]
        public async Task<IActionResult> GetAsync(int fieldId, int valueId)
        {
            var data = await _service.GetByIdAync(fieldId, valueId);
            return Ok(data);
        }
        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Save(int fieldId, [FromBody] EconomicIndicatorValueModel data)
        {
            var res = await _service.Save(fieldId, data);
            return Created($"{baseUrl}/EconomicIndicatorField/{res}", res);
        }
        [HttpPut]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Update(int fieldId,int valueId, [FromBody] EconomicIndicatorValueModel data)
        {
            data.EconomicIndicatorValueId = valueId;
            await _service.Update(fieldId, data);
            return NoContent();
        }
        [HttpDelete]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Delete(int fieldId, int valueId)
        {
            await _service.Delete(fieldId, valueId);
            return NoContent();
        }

        [HttpPut("/Post-economicIndicatorValuesNotes")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> PostEconomicIndicatorValuesNotes(int economicIndicatorID, int economicIndicatorFieldID, [FromBody] EconomicIndicatorValuesNotesModel data)
        {
            var res = await _service.UpdateEconomicIndicatorValueNotes(economicIndicatorID, economicIndicatorFieldID, data);
            return NoContent();
        }
    }
}
