namespace WebApplication3.Model
{
    public class Recipe
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImgURL { get; set; }
        // public List<Ingredients>? Ingredients { get; set; }
        public IList<Ingredients>? ingredients { get; set; } = new List<Ingredients>();


    }
}
