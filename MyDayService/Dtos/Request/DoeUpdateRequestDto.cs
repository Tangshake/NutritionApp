using System;
using System.Collections.Generic;

namespace MyDayService.Dtos.Request
{
    public class DoeUpdateRequestDto
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public int UserId { get; set ;}

        public List<SingleEntryRequestDto> Does { get; set; }
    }
}