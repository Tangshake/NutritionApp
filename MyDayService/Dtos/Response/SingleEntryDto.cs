using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace MyDayService.Dtos.Response
{
    public class SingleEntryDto
    {
        public string Hour { get; set; }

        public List<SingleProductDto> Products { get; set; }

        public List<SingleMealDto> Meals { get; set; }
    }
}