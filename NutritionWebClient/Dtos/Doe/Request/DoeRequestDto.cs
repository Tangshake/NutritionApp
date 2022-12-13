using System;
using System.Collections.Generic;

namespace NutritionWebClient.Dtos.Doe.Request
{
    public class DoeRequestDto
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public int UserId { get; set ;}

        public List<SingleEntryRequestDto> Does { get; set; }
    }
}