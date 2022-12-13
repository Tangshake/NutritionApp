namespace NutritionWebClient.Model.Doe.DoeNutritionSummary
{
    public class Summary
    {
        public float Weight { get; set; }
        public float Kcal { get; set; }
        public float Protein { get; set; }
        public float Carbohydrates { get; set; }
        public float Fat { get; set; }
        public float Roughage { get; set; }
    

        public void ClearAll()
        {
            Weight = 0;
            Kcal = 0;
            Protein = 0;
            Carbohydrates = 0;
            Fat = 0;
            Roughage = 0;
        }
    }
}