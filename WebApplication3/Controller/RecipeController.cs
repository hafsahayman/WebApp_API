using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Context;
using WebApplication3.Service;
using WebApplication3.View;



namespace WebApplication3.Controller
{
    [Route("api/v1/[controller]")]
    [ApiController]
   [Authorize]
    public class RecipeController:ControllerBase
    {
        private readonly IRecipeService _recipeService;
        private readonly DataContext _context;

        public RecipeController(IRecipeService recipeService, DataContext context)
        {
            _recipeService = recipeService;
           
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<RecipeCreateViewModel>> CreateRecipeAsync(RecipeCreateViewModel viewModel)
        {
            return Ok(await _recipeService.CreateAsync(viewModel));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeCreateViewModel>>> GetAllAsync()
        {
            return Ok(await _recipeService.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RecipeViewModel>> GetByIdAsync(int id)
        {
            return Ok(await _recipeService.GetByIdAsync(id));
        }
    }
}
