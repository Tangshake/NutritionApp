namespace NutritionWebClient.Dtos.Products.Request
{
    public class ProductCreateRequestDto
    {
        public string Name { get; set; }
        
        public string Manufacturer { get; set; }
        
        public float Kcal { get; set; }
        
        public float Protein { get; set; }
        
        public float Fat { get; set; }
        
        public float Carbohydrates { get; set; }
        
        public float Roughage { get; set; }
    }
}