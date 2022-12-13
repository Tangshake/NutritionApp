using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using UserRegister.Entity;

namespace UserRegister.Repository
{
    public class ActivationRepository : BaseRepository, IActivationRepository
    {
        public ActivationRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<Activation> GetUserActivation(int userId, string token)
        {
            try
            {
                using(var connection = GetConnection())
                {
                    connection.Open();               

                    var sql = @"SELECT * FROM nutactivation WHERE nutuser_id = @userid AND email_token = @token";

                    await using(NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userid", userId);
                        command.Parameters.AddWithValue("@token", token);

                        await using(NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                return new Activation()
                                {
                                    Id = int.Parse(reader["id"].ToString()),
                                    UserId = int.Parse(reader["nutuser_id"].ToString()),
                                    Token = reader["email_token"].ToString(),
                                    ActivationStart = DateTime.Parse(reader["activation_start"].ToString()),
                                    ActivationExpiry = DateTime.Parse(reader["activation_expiry"].ToString()),
                                };
                            }

                            return null;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"-->[GetUserActivation] {ex.Message}");
                return null;
            }
        }

        
    }
}