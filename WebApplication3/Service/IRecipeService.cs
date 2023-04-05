using WebApplication3.Model;
using WebApplication3.View;

namespace WebApplication3.Service
{
    public interface IRecipeService
    {
        Task<IEnumerable<RecipeViewModel>> GetAllAsync();

        Task<RecipeCreateViewModel> CreateAsync(RecipeCreateViewModel recipe);
        Task addIngredientsAsync(int id, IList<IngredientViewModel> ingredient);
        Task<RecipeViewModel> GetByIdAsync(int id);

      



    }
}
