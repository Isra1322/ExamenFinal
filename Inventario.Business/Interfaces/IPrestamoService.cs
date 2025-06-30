using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventario.Entities;

namespace Inventario.Business.Interfaces
{
    public interface IPrestamoService
    {
        Task SolicitarPrestamoAsync(int usuarioId, int articuloId, DateTime fechaEntrega);
        Task AprobarPrestamoAsync(int prestamoId);
        Task RechazarPrestamoAsync(int prestamoId);
        Task RegistrarDevolucionAsync(int prestamoId, DateTime fechaDevolucion);
    }
}
