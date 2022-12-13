using Microsoft.Extensions.Configuration;

namespace LogService.Repositories
{
    public abstract class BaseRepository
    {
        private readonly IConfiguration _configuration;

        public string ConnectionString { get; set; } = "Logs";

        public BaseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected Npgsql.NpgsqlConnection GetConnection()
        {
            return new Npgsql.NpgsqlConnection(_configuration.GetConnectionString(ConnectionString));
        }
    }
}