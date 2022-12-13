using System;

namespace LogService.Entity
{
    public class Log
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public string ServiceName { get; set; }
        public string Method { get; set; }
        public string Message { get; set; }
        public string Error { get; set; } = "";
    }
}