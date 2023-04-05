using WebApplication3.Model;

namespace WebApplication3.Service
{
    public interface IIngredientService
    {
        Task<IngredientViewModel> CreateAsync(IngredientViewModel ingredient);

    }
}
