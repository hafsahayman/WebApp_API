using AutoMapper;
using WebApplication3.Context;
using WebApplication3.Model;

namespace WebApplication3.Service
{
    public class IngredientService:IIngredientService
    {
        private readonly IIngredientService _ingredientService;
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public IngredientService( IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<IngredientViewModel> CreateAsync(IngredientViewModel ingredient)
        {
            var ingEntity = _mapper.Map<IngredientViewModel, Ingredients>(ingredient);
            await _context.Ingredients.AddAsync(ingEntity);
            await _context.SaveChangesAsync();
            return _mapper.Map<Ingredients, IngredientViewModel>(ingEntity);
        }
    }
}
