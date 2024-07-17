using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Catalog.Dtos.StatisticDtos;
using MultiShop.Catalog.Services.StatisticServices;
using System.Threading.Tasks;

namespace MultiShop.Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticService _statisticService;
        public StatisticsController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        [HttpGet("GetAllStatistics")]
        public async Task<IActionResult> GetAllStatistics()
        {
            var brandCountTask = _statisticService.GetBrandCount();
            var categoryCountTask = _statisticService.GetCategoryCount();
            var productCountTask = _statisticService.GetProductCount();
            var productAvgPriceTask = _statisticService.GetProductAvgPrice();
            var maxPriceProductNameTask = _statisticService.GetMaxPriceProductName();
            var minPriceProductNameTask = _statisticService.GetMinPriceProductName();

            await Task.WhenAll(brandCountTask, categoryCountTask, productCountTask, productAvgPriceTask, maxPriceProductNameTask, minPriceProductNameTask);

            var result = new ResultStatisticsDto
            {
                BrandCount = await brandCountTask,
                CategoryCount = await categoryCountTask ,
                ProductCount = await productCountTask, 
                ProductAvgPrice = await productAvgPriceTask ,
                MaxPriceProductName = await maxPriceProductNameTask ?? "N/A",
                MinPriceProductName = await minPriceProductNameTask ?? "N/A"
            };

            return Ok(result);
        }
    }
}
