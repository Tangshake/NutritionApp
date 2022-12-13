using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogService.Dtos;
using LogService.Entity;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace LogService.Repositories
{
    public class LogRepository : BaseRepository, ILogRepository
    {
        public LogRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<int> CreateLogAsync(LogRequestDto log)
        {
            try
            {
                using(var connection = GetConnection())
                {
                    connection.Open();               

                    var sql = @"INSERT INTO log VALUES (default, @userId, @date, @serviceName, @methodName, @message, @error)";

                    await using(NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userId", NpgsqlTypes.NpgsqlDbType.Integer, log.UserId);
                        command.Parameters.AddWithValue("@date", NpgsqlTypes.NpgsqlDbType.Timestamp, log.Date);
                        command.Parameters.AddWithValue("@serviceName", NpgsqlTypes.NpgsqlDbType.Varchar, log.ServiceName);
                        command.Parameters.AddWithValue("@methodName", NpgsqlTypes.NpgsqlDbType.Varchar, log.Method);
                        command.Parameters.AddWithValue("@message", NpgsqlTypes.NpgsqlDbType.Varchar, log.Message);
                        command.Parameters.AddWithValue("@error", NpgsqlTypes.NpgsqlDbType.Varchar, log.Error);

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        return Convert.ToInt32(rowsAffected);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"--> Ups something went wrong. Message: {ex.Message}");
                return 0;
            }
        }

        public async Task<List<Log>> GetLogByDateAndUserId(int userId, DateTime date)
        {
            try
            {
                using(var connection = GetConnection())
                {
                    connection.Open();

                    var sql = @"SELECT * FROM log WHERE user_id = @userId AND CAST(event_date as date) = @datetime";

                    await using(NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        command.Parameters.AddWithValue("@datetime", date.Date);

                        await using(NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            var result = new List<Log>();

                            while(await reader.ReadAsync())
                            {
                                result.Add(new Log(){
                                    Id = int.Parse(reader["id"].ToString()),
                                    UserId = int.Parse(reader["user_id"].ToString()),
                                    Date = DateTime.Parse(reader["event_date"].ToString()),
                                    Message = reader["message"].ToString(),
                                    Error = reader["error"].ToString(),
                                    ServiceName = reader["service_name"].ToString(),
                                    Method = reader["method_name"].ToString()
                                });
                            }

                            Console.WriteLine($"[GetLogByDateAndUserId] Results contains: {result.Count} items.");
                            return result;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Ups something went wrong: {ex.Message}");
            }

            return null;
        }

        public async Task<List<Log>> GetLogByUserId(int userId)
        {
            try
            {
                using(var connection = GetConnection())
                {
                    connection.Open();

                    var sql = @"SELECT * FROM log WHERE user_id = @userId";

                    await using(NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        await using(NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            var result = new List<Log>();

                            while(await reader.ReadAsync())
                            {
                                result.Add(new Log(){
                                    Id = int.Parse(reader["id"].ToString()),
                                    UserId = int.Parse(reader["user_id"].ToString()),
                                    Date = DateTime.Parse(reader["event_date"].ToString()),
                                    Message = reader["message"].ToString(),
                                    Error = reader["error"].ToString(),
                                    ServiceName = reader["service_name"].ToString(),
                                    Method = reader["method_name"].ToString()
                                });
                            }

                            return result;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Ups something went wrong: {ex.Message}");
            }

            return null;
        }

    }
}