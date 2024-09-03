using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces.Actions
{
    public interface IUsuariosRepository: IReadRepository<Usuarios, int>, ICreateRepository<Usuarios>, IUpdateRepository<Usuarios>, IRemoveRepository<int>
    {
        Task<Usuarios> GetUsuariosAsync(string _user, string _password);

    }
}
