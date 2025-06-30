using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventario.Entities;

namespace Inventario.DataAccess.Interfaces
{
    public interface IArticuloRepository
    {
        Task<Articulo?> ObtenerPorIdAsync(int id);
        Task<Articulo?> ObtenerPorCodigoAsync(string codigo);
        Task<List<Articulo>> ListarAsync();
        Task AgregarAsync(Articulo articulo);
        Task ActualizarAsync(Articulo articulo);
        Task EliminarAsync(Articulo articulo);
        Task GuardarCambiosAsync();
    }
}
