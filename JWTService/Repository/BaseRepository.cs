using Microsoft.Extensions.Configuration;

namespace JWTService.Repository
{
    public abstract class BaseRepository
    {
        private readonly IConfiguration _configuration;

        public string ConnectionString {get;set;} = "Secrets";

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