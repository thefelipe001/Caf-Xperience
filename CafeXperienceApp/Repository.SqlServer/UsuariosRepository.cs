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

        public Usuarios GetUsuarios(string _user, string _password)
        {
            // Validar el parámetro de entrada
            if (_user == string.Empty)
            {
                throw new ArgumentException("El Usuario no debe ser vacios.", nameof(_user));
            }
            if (_password == string.Empty)
            {
                throw new ArgumentException("La Contraseña no debe ser vacios.", nameof(_password));
            }


            // Crear el comando para llamar al procedimiento almacenado
            using (var command = new SqlCommand("sp_GetClientById", _context, _transaction))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Agregar el parámetro con validación para evitar inyecciones SQL
                command.Parameters.Add(new SqlParameter("@clientId", SqlDbType.Int)
                {
                    Value = _user,
                   
                });
                command.Parameters.Add(new SqlParameter("@clientId", SqlDbType.Int)
                {
                    Value = _password,

                });

                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Usuarios
                            {
                              
                            };
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Manejo de errores específicos de SQL
                    throw new ApplicationException("Ocurrió un error al intentar recuperar los datos.", ex);
                }
                catch (Exception ex)
                {
                    // Manejo de cualquier otro tipo de excepción
                    throw new ApplicationException("Ocurrió un error inesperado.", ex);
                }
            }

            // Devolver null si no se encontró ningún cliente
            return null;
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
