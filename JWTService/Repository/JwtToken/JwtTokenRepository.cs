using System;
using System.Threading.Tasks;
using JWTService.Entity;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace JWTService.Repository.JwtToken
{
    public class JwtTokenRepository : BaseRepository, IJwtTokenRepository
    {

        public JwtTokenRepository(IConfiguration configuration) : base(configuration)
        {
        }
        
        public async Task<JwtTokenEntity> GetJwtTokenAsync()
        {
            try
            {
                using(var connection = GetConnection())
                {
                    connection.Open();
                    
                    var sql = @"SELECT * FROM jwttoken WHERE active = @active_token";

                    await using(NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@active_token",NpgsqlTypes.NpgsqlDbType.Boolean, true);

                        await using(NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while(await reader.ReadAsync())
                            {
                                return new JwtTokenEntity(){
                                    Id = int.Parse(reader["secret_id"].ToString()),
                                    SecretKey = reader["secret_key"].ToString(),
                                    Active = bool.Parse(reader["active"].ToString()),
                                    ActivationDate = DateTime.Parse(reader["activation_date"].ToString()),
                                    DeactivationDate = DateTime.Parse(reader["deactivation_date"].ToString()),
                                };
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"[GetSecretKey] Something went wrong during retrieving secrets. Message: {ex.Message}");
            }

            return null;
        }
    }
}