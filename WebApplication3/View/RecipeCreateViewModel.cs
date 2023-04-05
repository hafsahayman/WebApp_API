using WebApplication3.Model;

namespace WebApplication3.View
{
    public class RecipeCreateViewModel
    {

        public string name { get; set; }
        public string description { get; set; }
        public string imgURL { get; set; }
        public IList<IngredientViewModel> ingredients { get; set; } = new List<IngredientViewModel>();

    }
}
