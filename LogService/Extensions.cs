using System;
using Google.Protobuf.WellKnownTypes;
using LogService.Entity;

namespace LogService
{
    public static class Extensions
    {
        public static LogModel AsModel(this Log log)
        {
            var logModel = new LogModel()
            {
                Id = log.Id,
                UserId = log.UserId,
                Date = Timestamp.FromDateTime(DateTime.SpecifyKind(log.Date, DateTimeKind.Utc)),
                ServiceName = log.ServiceName,
                Message = log.Message,
                Error = log.Error,
                Method = log.Method
            };

            return logModel;
        }
    }
}