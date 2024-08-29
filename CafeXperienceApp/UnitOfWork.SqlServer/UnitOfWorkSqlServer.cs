using Common;
using Microsoft.Extensions.Configuration;
using UnitOfWork.Interfaces;

namespace UnitOfWork.SqlServer
{
    public class UnitOfWorkSqlServer : IUnitOfWork
    {
        private readonly IConfiguration _configuration;

        public UnitOfWorkSqlServer(IConfiguration configuration = null)
        {
            _configuration = configuration;
        }

        public IUnitOfWorkAdapter Create()
        {
            var connectionString = Parameters.ConnectionString; // Utilizando la cadena de Parameters directamente
            return new UnitOfWorkSqlServerAdapter(connectionString);
        }
    }
}
