using Repository.Interfaces.Actions;
using Repository.SqlServer;
using System.Data.SqlClient;

using UnitOfWork.Interfaces;

namespace UnitOfWork.SqlServer
{
    public class UnitOfWorkSqlServerRepository : IUnitOfWorkRepository
    {

        public IUsuariosRepository UsuariosRepository { get; }

        public UnitOfWorkSqlServerRepository(SqlConnection context, SqlTransaction transaction)
        {

            UsuariosRepository = new UsuariosRepository(context, transaction);

        }
    }
}
