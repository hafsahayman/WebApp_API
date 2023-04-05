using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Context;
using WebApplication3.Model;
using WebApplication3.View;

namespace WebApplication3.Service
{
    public class RecipeService : IRecipeService

    {
      
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        // private Ingredients ingDb;



        public RecipeService(DataContext context, IMapper mapper)
        {

            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RecipeViewModel>> GetAllAsync()
        {
            return await _context
                .Recipe
                .Select(c => _mapper.Map<Recipe, RecipeViewModel>(c))
                .ToListAsync();


        }
        public async Task<RecipeCreateViewModel> CreateAsync(RecipeCreateViewModel recipe)
        {
            var recipeEntity = _mapper.Map<RecipeCreateViewModel, Recipe>(recipe);
            await _context.Recipe.AddAsync(recipeEntity);
           // var id = recipeEntity.Id;
           // await addIngredientsAsync(id, recipe.ingredients);
            await _context.SaveChangesAsync();


            return _mapper.Map<Recipe, RecipeCreateViewModel>(recipeEntity);
        }

        public async Task<RecipeViewModel> GetByIdAsync(int id)
        {
            var recipeDb = await GetRecipe(id);
            var viewModel = _mapper.Map<Recipe, RecipeViewModel>(recipeDb);

            var ingredients = recipeDb
                .ingredients
                .Where(a => a.recipeId == id)
                .Select(a => _mapper.Map<Ingredients, IngredientViewModel>(a))
                .ToList();

            viewModel.ingredients = ingredients;

            return viewModel;
        }

        private async Task<Recipe> GetRecipe(int id, bool withIngredients = true)
        {
            IQueryable<Recipe> query = _context.Recipe;

            if (withIngredients)
            {
                query = query.Include(c => c.ingredients);
            }

            var recipeDb = await query.FirstOrDefaultAsync(c => c.Id == id);

            if (recipeDb == null)
            {
                Console.WriteLine($"Could not find the customer with id: {id}");
            }

            return recipeDb;
        }
        public async Task addIngredientsAsync(int id, IList<IngredientViewModel> ingredient)
        {
            Ingredients ingDb = new Ingredients();



            if (ingredient != null)
            {
                foreach (var ing in ingredient)
                {

                    ingDb.Name = ing.Name;
                    ingDb.Amount = ing.Amount;
                    ingDb.recipeId = id;

                    Console.WriteLine(ing.Name);
                    Console.Write(ing.Amount);
                }

                await _context.Ingredients.AddAsync(ingDb);
                //await _context.SaveChangesAsync();

                //for (int i = 0; i < ingredient.Count-1; i++)
                //{
                //    var ingDb = new Ingredients
                //    {
                //        Name = ingredient[i].Name,
                //        Amount = ingredient[i].Amount,
                //        recipeId = id

                //    };
                //    await _context.Ingredients.AddAsync(ingDb);
                //    await _context.SaveChangesAsync();
                //    Console.WriteLine(ingredient.Count);

                //    //ingDb.Name = ingredient[i].Name;
                //    //ingDb.Amount = ingredient[i].Amount;
                //    //ingDb.recipeId = id;
                //    //await _context.Ingredients.AddAsync(ingDb);
                //    //await _context.SaveChangesAsync();

                //}
            }
            


        }

       
    }
}



    

