using Microsoft.Extensions.Configuration;

namespace UserLogin.Repository
{
    public abstract class BaseRepository
    {
        private readonly IConfiguration _configuration;
        public string ConnectionString { get; set; } = "Users";

        protected BaseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected Npgsql.NpgsqlConnection GetConnection()
        {
            return new Npgsql.NpgsqlConnection(_configuration.GetConnectionString(ConnectionString));
        }
    }
}