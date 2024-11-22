using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Catalog.Dtos.DealsOfDayDtos;
using MultiShop.Catalog.Services.DealsOfDayServices;

using System.Threading.Tasks;

namespace MultiShop.Catalog.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DealsOfDaysController : ControllerBase
    {
        private readonly IDealsOfDayService _dealsService;

        public DealsOfDaysController(IDealsOfDayService dealsService)
        {
            _dealsService = dealsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDealsOfDay()
        {
            var deals = await _dealsService.GetAllDealsOfDayAsync();
            return Ok(deals);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDealsOfDayById(string id)
        {
            var deal = await _dealsService.GetDealsOfDayByIdAsync(id);
            return Ok(deal);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDealsOfDay(CreateDealsOfDayDto createDealDto)
        {
            await _dealsService.CreateDealsOfDayAsync(createDealDto);
            return Ok("Deal created successfully");
        }
        [HttpGet("{id}/UpdateGetByIdDealAsync")]
        public async Task<IActionResult> UpdateGetByIdDealAsync(string id)
        {
            var deal = await _dealsService.UpdateGetByIdDealAsync(id);
            return Ok(deal);
        }

        [HttpPut("UpdateDealsOfDay/{id}")]
        public async Task<IActionResult> UpdateDealsOfDay(string id, [FromBody] UpdateDealsOfDayDto updateDealDto)
        {
            if (id != updateDealDto.DealsOfDayId)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                await _dealsService.UpdateDealsOfDayAsync(updateDealDto);
                return Ok(new { success = true, message = "Deal updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDealsOfDay(string id)
        {
            await _dealsService.DeleteDealsOfDayAsync(id);
            return Ok("Deal deleted successfully");
        }

        [HttpPost("{id}/ChangeStatus")]
        public async Task<IActionResult> ChangeDealStatus(string id, [FromBody] bool isActive)
        {
            await _dealsService.ChangeDealStatusAsync(id, isActive);
            return Ok($"Deal status updated to {(isActive ? "Active" : "Inactive")}");
        }

    }
}
