using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using UserLogin.Entity;

namespace UserLogin.Repository
{
    public class UserRepository : BaseRepository, IUserRepository
    {

        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<int> CreateSecurityStampForUserAsync(int userId, string guid)
        {
            try
            {
                using(var connection = GetConnection())
                {
                    connection.Open();               

                    var sql = @"INSERT INTO usersecuritystamp VALUES (default, @userId, @guid) 
                                ON CONFLICT (user_id) 
                                DO UPDATE SET sec_key = @guid";

                    await using(NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userId", NpgsqlTypes.NpgsqlDbType.Integer, userId);
                        command.Parameters.AddWithValue("@guid", NpgsqlTypes.NpgsqlDbType.Varchar, guid);

                        var rowsAffected = await command.ExecuteScalarAsync();

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

        public async Task<UserSecurityStamp> GetUserSecurityStampAsync(int userId)
        {
            try
            {
                using(var connection = GetConnection())
                {
                    connection.Open();               

                    var sql = @"SELECT * FROM usersecuritystamp WHERE user_id = @userId";

                    await using(NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        await using(NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                return new UserSecurityStamp()
                                {
                                    SecId = int.Parse(reader["sec_id"].ToString()),
                                    UserId = int.Parse(reader["user_id"].ToString()),
                                    SecKey = reader["sec_key"].ToString()
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"--> Ups something went wrong. Message: {ex.Message}");
                return null;
            }
        }

        public async Task<User> GetUserAsync(string email)
        {
            try
            {
                using(var connection = GetConnection())
                {
                    connection.Open();               

                    var sql = @"SELECT * FROM nutuser WHERE email = @useremail";

                    await using(NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@useremail", email);

                        await using(NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                return new User()
                                {
                                    Id = int.Parse(reader["user_id"].ToString()),
                                    Name = reader["user_name"].ToString(),
                                    Email = reader["email"].ToString(),
                                    PasswordHash = reader["passhash"].ToString(),
                                    Role = reader["user_role"].ToString(),
                                    Activated = bool.Parse(reader["activated"].ToString())
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"--> Ups something went wrong. Message: {ex.Message}");
                return null;
            }
        }

        public async Task UpdateUserSecurityTimestampAsync(int userId, string guid)
        {
            try
            {
                using(var connection = GetConnection())
                {
                    connection.Open();               

                    var sql = @"INSERT INTO usersecuritystamp VALUES (default, @userId, @guid) 
                                ON CONFLICT (user_id) 
                                DO UPDATE SET sec_key = @guid";

                    await using(NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userId", NpgsqlTypes.NpgsqlDbType.Integer, userId);
                        command.Parameters.AddWithValue("@guid", NpgsqlTypes.NpgsqlDbType.Varchar, guid);

                        var rowsAffected = await command.ExecuteScalarAsync();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"--> Ups something went wrong. Message: {ex.Message}");
            }
        }
    }
}