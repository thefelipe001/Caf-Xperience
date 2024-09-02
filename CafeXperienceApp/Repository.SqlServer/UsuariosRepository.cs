using Models;
using Repository.Interfaces.Actions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.SqlServer
{
    public class UsuariosRepository : Repository, IUsuariosRepository
    {

        public UsuariosRepository(SqlConnection context, SqlTransaction transaction)
        {
            this._context = context;
            this._transaction = transaction;
        }

        public void Create(Usuarios t)
        {
            throw new NotImplementedException();
        }

        public Usuarios Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Usuarios> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Remove(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Usuarios t)
        {
            throw new NotImplementedException();
        }
    }
}
