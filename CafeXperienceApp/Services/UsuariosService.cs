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
        IEnumerable<Usuarios> GetAll();
        Usuarios Get(int id);
        void Create(Usuarios model);
        void Update(Usuarios model);
        void Delete(int id);
    }
    public class UsuariosService: IUsuariosService
    {
        private IUnitOfWork _unitOfWork;

        public UsuariosService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Create(Usuarios model)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
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

        public void Update(Usuarios model)
        {
            throw new NotImplementedException();
        }
    }
}
