using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.Category;
using Services.Services.CategoryService;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categorySerivce;
        public CategoryController(ICategoryService categorySerivce)
        {
            _categorySerivce = categorySerivce;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById([FromRoute]string id)
        {
            var res = await _categorySerivce.GetCategoryById(id);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllCategories()
        {
            var res = await _categorySerivce.GetAllCategories();
            return StatusCode(res.StatusCode, res);
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory([FromForm] CategoryRequest category)
        {
            var res = await _categorySerivce.CreateCategory(category);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory([FromRoute] string id, [FromBody] CategoryRequest category)
        {
            var res = await _categorySerivce.UpdateCategory(id, category);
            return StatusCode(res.StatusCode, res);
        }
    }
}
