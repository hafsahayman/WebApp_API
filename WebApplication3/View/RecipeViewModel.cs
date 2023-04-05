using WebApplication3.Model;

namespace WebApplication3.View
{
    public class RecipeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImgURL { get; set; }
        public IList<IngredientViewModel> ingredients { get; set; } = new List<IngredientViewModel>();
    }
}
