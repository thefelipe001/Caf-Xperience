using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork.Interfaces;

namespace Services
{
  
    public interface IUsuariosService
    {
        Task<IEnumerable<Usuarios>> GetAllAsync(); 
        Task<Usuarios> GetAsync(int id); 
        Task CreateAsync(Usuarios model);
        Task UpdateAsync(Usuarios model);
        Task DeleteAsync(int id); 
        Task<Usuarios> GetUsuariosAsync(string _user, string _password); 

    }

    public class UsuariosService: IUsuariosService
    {
        private IUnitOfWork _unitOfWork;

        public UsuariosService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task CreateAsync(Usuarios model)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Usuarios>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Usuarios> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Usuarios> GetUsuariosAsync(string _user, string _password)
        {
            using (var context = _unitOfWork.Create())
            {
                var usuarios = await context.Repositories.UsuariosRepository.GetUsuariosAsync(_user, _password);

                return usuarios;
            }
        }


        public Task UpdateAsync(Usuarios model)
        {
            throw new NotImplementedException();
        }
    }
}
 