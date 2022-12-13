using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogService.Dtos;
using LogService.Entity;

namespace LogService.Repositories
{
    public interface ILogRepository
    {
        Task<int> CreateLogAsync(LogRequestDto log);

        Task<List<Log>> GetLogByUserId(int userId);

        Task<List<Log>> GetLogByDateAndUserId(int userId, DateTime date);
    }
}