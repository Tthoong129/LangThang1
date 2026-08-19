using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MiniMap.Services;

namespace MiniMap.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodsController : ControllerBase
    {
        private readonly IFoodService _foodService;

        public FoodsController(IFoodService foodService)
        {
            _foodService = foodService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var foods = await _foodService.GetAllFoodsAsync();
            return Ok(foods);
        }

        [HttpGet("by-province/{provinceId}")]
        public async Task<IActionResult> GetByProvince(int provinceId)
        {
            var foods = await _foodService.GetFoodsByProvinceAsync(provinceId);
            return Ok(foods);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(long id)
        {
            var food = await _foodService.GetFoodDetailAsync(id);
            if (food == null) return NotFound();
            return Ok(food);
        }
    }
}
