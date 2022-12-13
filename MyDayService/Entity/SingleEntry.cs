using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace MyDayService.Entity
{
    public class SingleEntry
    {
        public string Hour { get; set; }

        public List<SingleProduct> Products { get; set; }

        public List<SingleMeal> Meals { get; set; }
    }
}