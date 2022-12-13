//dotnet add package Npgsql --version 6.0.5
using Microsoft.Extensions.Configuration;


namespace MyDayService.Repository
{
    public abstract class BaseRepository
    {
        private readonly IConfiguration _configuration;
        public string ConnectionString { get; set; } = "MongoDbConnection";

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