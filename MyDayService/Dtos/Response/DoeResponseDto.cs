using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace MyDayService.Dtos.Response
{
    public class DoeResponseDto
    {
        public Guid Id { get; set; }
    
        public DateTime Date { get; set; }

        public int UserId { get; set ;}

        public List<SingleEntryDto> Does { get; set; }
    }
}