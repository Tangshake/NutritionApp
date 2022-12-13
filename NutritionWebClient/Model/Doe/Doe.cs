using System;
using System.Collections.Generic;

namespace NutritionWebClient.Model.Doe
{
    public class Doe
    {
        public Guid Id { get; set; }
    
        public DateTime Date { get; set; }

        public int UserId { get; set ;}

        public List<SingleEntry> Does { get; set; }
    }
}