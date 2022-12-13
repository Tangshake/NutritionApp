using System;
using System.Threading.Tasks;
using JWTService.Entity;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace JWTService.Repository.RefreshToken
{
    public class RefreshTokenRepository : BaseRepository, IRefreshTokenRepository
    {

        public RefreshTokenRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<int> RemoveRefreshTokenAsync(int tokenId, int userId)
        {
            try
            {
                using(var connection = GetConnection())
                {
                    connection.Open();
                    
                    var sql = @"DELETE FROM refreshtoken WHERE user_id = @userId AND rid = @tokenId";

                    await using(NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userId", NpgsqlTypes.NpgsqlDbType.Integer, userId);
                        command.Parameters.AddWithValue("@tokenId", NpgsqlTypes.NpgsqlDbType.Integer, tokenId);

                        var rowsAffected = await command.ExecuteScalarAsync();

                        return Convert.ToInt32(rowsAffected);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"[RemoveRefreshTokenAsync] Something went wrong during deleting refresh token. Message: {ex.Message}");
            }

            return -1;
        }

        public async Task<int> SaveRefreshTokenAsync(JWTService.Entity.RefreshToken refreshToken)
        {
            try
            {
                using(var connection = GetConnection())
                {
                    connection.Open();
                    
                    var sql = @"INSERT INTO refreshtoken VALUES (default, @userid, @rtoken, @act, @deact) RETURNING rid";

                    await using(NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userid", NpgsqlTypes.NpgsqlDbType.Integer, refreshToken.UserId);
                        command.Parameters.AddWithValue("@rtoken", NpgsqlTypes.NpgsqlDbType.Varchar, refreshToken.Token);
                        command.Parameters.AddWithValue("@act", NpgsqlTypes.NpgsqlDbType.TimestampTz, refreshToken.ActivationDate);
                        command.Parameters.AddWithValue("@deact", NpgsqlTypes.NpgsqlDbType.TimestampTz, refreshToken.DeactivationDate);

                        var rowsAffected = await command.ExecuteScalarAsync();

                        return Convert.ToInt32(rowsAffected);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"[SaveRefreshTokenAsync] Something went wrong during saving refresh token. Message: {ex.Message}");
            }

            return -1;
        }

        public async Task<Entity.RefreshToken> GetRefreshTokenAsync(string email)
        {
            try
            {
                using(var connection = GetConnection())
                {
                    connection.Open();

                    var sql = @"SELECT rid, refreshtoken.user_id, rtoken, activation_date, deactivation_date FROM nutuser
                                INNER JOIN refreshtoken
                                ON nutuser.user_id = refreshtoken.user_id
                                WHERE nutuser.email = @email
                                ORDER BY activation_date DESC;";

                    //AND deactivation_date > now()::timestamptz
                    await using(NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", NpgsqlTypes.NpgsqlDbType.Varchar, email);

                        await using(NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                return new Entity.RefreshToken(){
                                    RId =  int.Parse(reader["rid"].ToString()),
                                    UserId =  int.Parse(reader["user_id"].ToString()),
                                    Token =  reader["rtoken"].ToString(),
                                    ActivationDate =  DateTime.Parse(reader["activation_date"].ToString()),
                                    DeactivationDate =  DateTime.Parse(reader["deactivation_date"].ToString()),
                                };
                            }
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                
                throw;
            }

            return null;
        }
    }
}