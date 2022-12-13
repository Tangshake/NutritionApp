using System;

namespace LogService.Dtos
{
    public class LogRequestDto
    {
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public string ServiceName { get; set; }
        public string Method { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
    }
}