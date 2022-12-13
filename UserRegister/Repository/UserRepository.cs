using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using UserRegister.Entity;

namespace UserRegister.Repository
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<int> UserExistsAsync(string name, string email)
        {
            try
            {
                using(var connection = GetConnection())
                {
                    connection.Open();               

                    var sql = @"SELECT * FROM nutuser WHERE user_name = @username OR email = @useremail";

                    await using(NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.Add("@username", NpgsqlTypes.NpgsqlDbType.Varchar).Value = name;
                        command.Parameters.AddWithValue("@useremail",NpgsqlTypes.NpgsqlDbType.Varchar).Value = email;

                        await using(NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if(reader.HasRows)
                            {
                                await connection.CloseAsync();
                                return 1;
                            }
                            else
                            {
                                await connection.CloseAsync();
                                return 0;
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"--> {ex.Message}");
                return int.Parse("-1");
            }
        }

        public async Task<int> CreateUserAsync(string login, string role, string email, string password)
        {
            try
            {
                using(var connection = GetConnection())
                {
                    connection.Open();               

                    var sql = @"INSERT INTO nutuser VALUES (DEFAULT, @username, @userrole, @userpassword, @useremail) RETURNING user_id";

                    await using(NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@username", login);
                        command.Parameters.AddWithValue("@userrole", role);
                        command.Parameters.AddWithValue("@userpassword", password);
                        command.Parameters.AddWithValue("@useremail", email);

                        var user_id = await command.ExecuteScalarAsync();  //returns user_id
                        await connection.CloseAsync();

                        return Convert.ToInt32(user_id);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"--> [CreateUser] {ex.Message}");
                return int.Parse("-1");
            }
        }

        public async Task<User> GetUserByLoginAsync(string name, string email)
        {
            Console.WriteLine($"[GetUserByLoginAsync] Name: {name}, email:{email}");
            using(var connection = GetConnection())
            {
                try
                {
                    connection.Open();               
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"--> Cannot open connection to user database: {ex.Message}");
                    return null;
                }

                var sql = @"SELECT * FROM nutuser WHERE user_name = @username OR email = @useremail";

                await using(NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@username", name);
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
                                Role = reader["role"].ToString()
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<int> CreateEmailVerificationEntryAsync(int user_id, string token, DateTime tokenGenerationDate, DateTime tokenExpirationDate)
        {
            try
            {
                using(var connection = GetConnection())
                {
                    connection.Open();               

                    var sql = @"INSERT INTO nutactivation VALUES (DEFAULT, @userid, @token, @activationstart, @activationend)";

                    await using(NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userid", user_id);
                        command.Parameters.AddWithValue("@token", token);
                        command.Parameters.AddWithValue("@activationstart", tokenGenerationDate);
                        command.Parameters.AddWithValue("@activationend", tokenExpirationDate);

                        var result = await command.ExecuteNonQueryAsync();  //returns number of rows affected
                        await connection.CloseAsync();

                        return result;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"--> [CreateEmailVerificationEntry] {ex.Message}");
                return int.Parse("-1");
            }
        }

        public async Task<int> SetEmailAccountAsVerifiedAsync(int userId)
        {
            try
            {
                using(var connection = GetConnection())
                {
                    connection.Open();               

                    var sql = @"UPDATE nutuser SET activated = true WHERE user_id = @userid";

                    await using(NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userid", userId);

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        await connection.CloseAsync();

                        return rowsAffected;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"--> [SetEmailAccountAsVerified] {ex.Message}");
                return int.Parse("-1");
            }
        }

        public async Task<bool?> UserActivatedAsync(int userId)
        {
            try
            {
                using(var connection = GetConnection())
                {
                    connection.Open();

                    var sql = @"SELECT activated FROM nutuser WHERE user_id = @userid";

                    await using(NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userid", userId);

                        var isActivated = await command.ExecuteScalarAsync();
                        await connection.CloseAsync();

                        return Convert.ToBoolean(isActivated);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"--> [UserActivated] {ex.Message}");
                return null;
            }
        }
    }
}