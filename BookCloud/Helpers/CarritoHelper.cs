using BookCloud.Models.ViewModels;
using MvcCoreSessionEmpleados.Extensions;

namespace BookCloud.Helpers
{
    public static class CarritoHelper
    {
        private const string CARRITO_SESSION_KEY = "Carrito";

        public static List<CarritoItem> GetCarrito(ISession session)
        {
            return session.GetObject<List<CarritoItem>>(CARRITO_SESSION_KEY)
                   ?? new List<CarritoItem>();
        }

        public static void SaveCarrito(ISession session, List<CarritoItem> items)
        {
            session.SetObject(CARRITO_SESSION_KEY, items);
        }

        public static void AgregarItem(ISession session, CarritoItem nuevoItem)
        {
            var carrito = GetCarrito(session);
            var itemExistente = carrito.FirstOrDefault(i => i.LibroId == nuevoItem.LibroId);

            if (itemExistente != null)
            {
                // Si ya existe, aumentar cantidad
                itemExistente.Cantidad += nuevoItem.Cantidad;

                // Validar que no exceda el stock
                if (itemExistente.Cantidad > itemExistente.StockDisponible)
                    itemExistente.Cantidad = itemExistente.StockDisponible;
            }
            else
            {
                // Agregar nuevo item
                carrito.Add(nuevoItem);
            }

            SaveCarrito(session, carrito);
        }

        public static void ActualizarCantidad(ISession session, int libroId, int nuevaCantidad)
        {
            var carrito = GetCarrito(session);
            var item = carrito.FirstOrDefault(i => i.LibroId == libroId);

            if (item != null)
            {
                if (nuevaCantidad <= 0)
                {
                    carrito.Remove(item);
                }
                else if (nuevaCantidad <= item.StockDisponible)
                {
                    item.Cantidad = nuevaCantidad;
                }
            }

            SaveCarrito(session, carrito);
        }

        public static void EliminarItem(ISession session, int libroId)
        {
            var carrito = GetCarrito(session);
            carrito.RemoveAll(i => i.LibroId == libroId);
            SaveCarrito(session, carrito);
        }

        public static void LimpiarCarrito(ISession session)
        {
            session.Remove(CARRITO_SESSION_KEY);
        }

        public static int GetCantidadTotal(ISession session)
        {
            var carrito = GetCarrito(session);
            return carrito.Sum(i => i.Cantidad);
        }

        public static decimal GetTotal(ISession session)
        {
            var carrito = GetCarrito(session);
            return carrito.Sum(i => i.Subtotal);
        }
    }
}