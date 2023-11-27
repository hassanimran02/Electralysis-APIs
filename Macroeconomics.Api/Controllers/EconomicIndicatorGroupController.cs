using Macroeconomics.BLS.Services.EconomicIndicatorGroups;
using Macroeconomics.BLS.Models.EconomicIndicatorGroup;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Macroeconomics.Api.Controllers
{
    [Route("api/v{version:apiVersion}/json/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class EconomicIndicatorGroupController : BaseController
    {

        private readonly IEconomicIndicatorGroupService _service;
        private JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null
        };
        public EconomicIndicatorGroupController(IEconomicIndicatorGroupService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([Optional] int? parentGroupId)
        {
            return new JsonResult(await _service.GetAllAsync(parentGroupId), serializerOptions);
        }

        [HttpGet("GetParentGroupAndSubGroups")]
        public async Task<IActionResult> GetParentGroupAndSubGroups(int? parentGroupId = null, bool? includeParentGroups = false)
        {
            return Ok(await _service.GetAllAsync(parentGroupId, includeParentGroups));
        }

        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetAsync(int groupId)
        {
            var data = await _service.GetByIdAync(groupId);
            return Ok(data);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Save([FromBody] EconomicIndicatorGroupModel data)
        {
            var res = await _service.Save(data);
            return Created($"{baseUrl}/EconomicIndicatorGroup/{res}", res);
        }

        [HttpPut("{groupId}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Update(int groupId, [FromBody] EconomicIndicatorGroupModel data)
        {
            data.GroupID = groupId;
            await _service.Update(data);
            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Delete(int groupId)
        {
            await _service.Delete(groupId);
            return NoContent();
        }

        [HttpGet("petapoco/indicatorgroups")]
        public async Task<JsonResult> GetIndicatorGroups()
        {
            var economicIndicatorGroups = await _service.GetAllIndicatorGroups();

            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            };

            return new JsonResult(economicIndicatorGroups, serializerOptions);
        }
    }
}
