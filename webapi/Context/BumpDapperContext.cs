using Microsoft.Data.SqlClient;
using System.Data;

namespace webapi.Context
{
    public class BumpDapperContext
    {
        private readonly IConfiguration _configuration;

        public BumpDapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    }
}
