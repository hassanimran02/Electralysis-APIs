using Macroeconomics.BLS.Models.EconomicIndicatorField;
using Microsoft.AspNetCore.Mvc;
using Macroeconomics.BLS.Services.EconomicIndicatorFields;
using System.Runtime.InteropServices;

namespace Macroeconomics.Api.Controllers
{
    [Route("api/v{version:apiVersion}/json/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class EconomicIndicatorFieldController : BaseController
    {

        private readonly IEconomicIndicatorFieldService _service;
        public EconomicIndicatorFieldController(IEconomicIndicatorFieldService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([Optional] int? groupId)
        {
            return Ok(await _service.GetAllAsync(groupId));
        }

        [HttpGet("{groupId}/{fieldId}")]
        public async Task<IActionResult> GetAsync(int groupId, short fieldId)
        {
            var data = await _service.GetByIdAync(groupId, fieldId);
            return Ok(data);
        }

        [HttpPost("{groupId}")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Save(int groupId, [FromBody] EconomicIndicatorFieldModel data)
        {
            var res = await _service.Save(groupId, data);
            return Created($"{baseUrl}/EconomicIndicatorField/{res}", res);
        }

        [HttpPut("{groupId}/{fieldId}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Update(int groupId, short fieldId, [FromBody] EconomicIndicatorFieldModel data)
        {
            data.EconomicIndicatorFieldId = fieldId;
            await _service.Update(groupId, data);
            return NoContent();
        }

        [HttpDelete("{groupId}/{fieldId}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Delete(int groupId, int id)
        {
            await _service.Delete(groupId, id);
            return NoContent();
        }
    }
}
