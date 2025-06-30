using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventario.Entities;

namespace Inventario.DataAccess.Interfaces
{
    public interface IPrestamoRepository
    {
        Task<Prestamo?> ObtenerPorIdAsync(int id);
        Task<List<Prestamo>> ListarAsync();
        Task<List<Prestamo>> ListarPorUsuarioAsync(int usuarioId);
        Task<List<Prestamo>> ListarPorArticuloAsync(int articuloId);
        Task AgregarAsync(Prestamo prestamo);
        Task ActualizarAsync(Prestamo prestamo);
        Task GuardarCambiosAsync();
    }
}
