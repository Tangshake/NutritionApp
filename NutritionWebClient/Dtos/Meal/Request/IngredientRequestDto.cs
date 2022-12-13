namespace NutritionWebClient.Dtos.Meal.Request
{
    public class IngredientRequestDto
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Manufacturer { get; set; }
        
        public int Kcal { get; set; }
        
        public float Protein { get; set; }
        
        public float Fat { get; set; }
        
        public float Carbohydrates { get; set; }
        
        public float Roughage { get; set; }

        public float Weight { get; set; }
    }
}