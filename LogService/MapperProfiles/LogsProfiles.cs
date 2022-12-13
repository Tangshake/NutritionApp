using System.Collections.Generic;
using AutoMapper;
using LogService;
using LogService.Entity;

namespace LogService.MapperProfiles
{
    public class LogsProfiles : Profile
    {
        public LogsProfiles()
        {
            CreateMap<Log, GrpcResponseLogsDto>();
            // Source --> Target
            CreateMap<Log, LogModel>();
        }
    }
}