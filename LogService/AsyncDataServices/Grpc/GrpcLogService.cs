using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using LogService.Repositories;

namespace LogService.AsyncDataServices.Grpc
{
    public class GrpcLogService : GrpcLogs.GrpcLogsBase
    {
        private readonly ILogRepository _repository;
        private readonly IMapper _mapper;

        public GrpcLogService(ILogRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public override async Task<GrpcResponseLogsDto> GetLogsByDateAndUserId(GrpcRequestLogDto request, ServerCallContext context)
        {
            if(request is not null)
            {
                var date = request.Date.ToDateTimeOffset();
                Console.WriteLine($"[GrpcServerCall] DateTimeOffset: {date}");
                Console.WriteLine($"[GrpcServerCall] GetLogsByUserId id: {request.UserId} date: {request.Date.ToDateTime()}.");
                
                var result = await _repository.GetLogByDateAndUserId(request.UserId, request.Date.ToDateTime());
                
                var lGrpcModel = result.Select(x => x.AsModel());
                var response = new GrpcResponseLogsDto() { Logs = { lGrpcModel }};

                Console.WriteLine($"[GrpcServerCall] Returning: {response.Logs.Count} items from service.");
                return response;
            }
            
            return null;
        }
    }
}