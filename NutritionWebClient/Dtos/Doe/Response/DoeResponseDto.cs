using System;
using System.Collections.Generic;

namespace NutritionWebClient.Dtos.Doe.Response
{
    public class DoeResponseDto
    {
        public Guid Id { get; set; }
    
        public DateTime Date { get; set; }

        public int UserId { get; set ;}

        public List<SingleEntryDto> Does { get; set; }
    }
}