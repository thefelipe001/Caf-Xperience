using Services;
using System.Web.Mvc;
using UnitOfWork.Interfaces;
using UnitOfWork.SqlServer;
using Unity;
using Unity.Mvc5;

namespace CafeXperienceApp
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // Registra todos tus servicios aquí
            container.RegisterType<IUsuariosService, UsuariosService>();
            // Registro de UnitOfWork
            container.RegisterType<IUnitOfWork, UnitOfWorkSqlServer>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}