using CafeXperienceApp.Models;
using Newtonsoft.Json;

namespace CafeXperienceApp.Repositorio
{
    public class CarritoService
    {
        private readonly ISession _session;
        private const string CarritoSessionKey = "Carrito";

        public CarritoService(IHttpContextAccessor httpContextAccessor)
        {
            _session = httpContextAccessor.HttpContext.Session;
        }

        public List<CarritoItem> ObtenerCarrito()
        {
            var carritoJson = _session.GetString(CarritoSessionKey);
            return carritoJson == null ? new List<CarritoItem>() : JsonConvert.DeserializeObject<List<CarritoItem>>(carritoJson);
        }

        public void AgregarAlCarrito(CarritoItem item)
        {
            var carrito = ObtenerCarrito();
            var itemExistente = carrito.FirstOrDefault(i => i.ProductId == item.ProductId);

            if (itemExistente != null)
            {
                itemExistente.Cantidad += item.Cantidad;
            }
            else
            {
                carrito.Add(item);
            }

            _session.SetString(CarritoSessionKey, JsonConvert.SerializeObject(carrito));
        }
    }

}
