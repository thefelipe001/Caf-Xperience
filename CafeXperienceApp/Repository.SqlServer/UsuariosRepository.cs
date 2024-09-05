using Models;
using Repository.Interfaces.Actions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Repository.SqlServer
{
    public class UsuariosRepository : Repository, IUsuariosRepository
    {

        public UsuariosRepository(SqlConnection context, SqlTransaction transaction)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _transaction = transaction; // Puede ser null si no se está utilizando una transacción
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

        public async Task<Usuarios> GetUsuariosAsync(string _user, string _password)
        {
            if (string.IsNullOrEmpty(_user))
                throw new ArgumentException("El Usuario no debe ser vacío.", nameof(_user));

            if (string.IsNullOrEmpty(_password))
                throw new ArgumentException("La Contraseña no debe estar vacía.", nameof(_password));

            using (var command = new SqlCommand("sp_MantenimientoUsuarios", _context, _transaction))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Correo", _user);
                command.Parameters.AddWithValue("@Contraseña", _password);
                command.Parameters.AddWithValue("@Opcion", 6);

                SqlParameter resultadoParam = new SqlParameter("@Resultado", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(resultadoParam);

                try
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Usuarios
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                Correo = reader["Correo"].ToString(),
                            };
                        }
                    }

                    bool resultado = Convert.ToBoolean(resultadoParam.Value);
                    if (!resultado)
                    {
                        throw new ApplicationException("La operación no se completó correctamente.");
                    }
                }
                catch (SqlException ex)
                {
                    throw new ApplicationException("Ocurrió un error al intentar recuperar los datos.", ex);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Ocurrió un error inesperado.", ex);
                }
            }

            return null;
        }


    }
}
