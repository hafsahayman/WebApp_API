namespace WebApplication3.Model
{
    public class Ingredients
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Amount { get; set; }
        public Recipe? Recipe { get; set; }
        public int? recipeId { get; set; }
        //public int Id { get; set; }
        //public string? name { get; set; }
        //public int  amount { get; set; }
        //public Recipe? recipe { get; set; }
    }
}
