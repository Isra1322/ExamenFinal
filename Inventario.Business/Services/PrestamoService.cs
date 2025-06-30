using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventario.Business.Interfaces;
using Inventario.DataAccess.Interfaces;
using Inventario.Entities;

namespace Inventario.Business.Services
{
    public class PrestamoService : IPrestamoService
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IArticuloRepository _articuloRepository;

        public PrestamoService(IPrestamoRepository prestamoRepository, IArticuloRepository articuloRepository)
        {
            _prestamoRepository = prestamoRepository;
            _articuloRepository = articuloRepository;
        }

        public async Task SolicitarPrestamoAsync(int usuarioId, int articuloId, DateTime fechaEntrega)
        {
            var articulo = await _articuloRepository.ObtenerPorIdAsync(articuloId);
            if (articulo == null || articulo.Estado != "Disponible")
                throw new Exception("Artículo no disponible.");

            var prestamo = new Prestamo
            {
                UsuarioId = usuarioId,
                ArticuloId = articuloId,
                FechaSolicitud = DateTime.Now,
                FechaEntrega = fechaEntrega,
                Estado = "Pendiente"
            };

            await _prestamoRepository.AgregarAsync(prestamo);
            articulo.Estado = "Prestado";
            await _articuloRepository.ActualizarAsync(articulo);

            await _prestamoRepository.GuardarCambiosAsync();
            await _articuloRepository.GuardarCambiosAsync();
        }

        public async Task AprobarPrestamoAsync(int prestamoId)
        {
            var prestamo = await _prestamoRepository.ObtenerPorIdAsync(prestamoId);
            if (prestamo == null) throw new Exception("Préstamo no encontrado.");

            prestamo.Estado = "Aprobado";
            await _prestamoRepository.ActualizarAsync(prestamo);
            await _prestamoRepository.GuardarCambiosAsync();
        }

        public async Task RechazarPrestamoAsync(int prestamoId)
        {
            var prestamo = await _prestamoRepository.ObtenerPorIdAsync(prestamoId);
            if (prestamo == null) throw new Exception("Préstamo no encontrado.");

            prestamo.Estado = "Rechazado";

            var articulo = await _articuloRepository.ObtenerPorIdAsync(prestamo.ArticuloId);
            articulo.Estado = "Disponible";

            await _prestamoRepository.ActualizarAsync(prestamo);
            await _articuloRepository.ActualizarAsync(articulo);

            await _prestamoRepository.GuardarCambiosAsync();
            await _articuloRepository.GuardarCambiosAsync();
        }

        public async Task RegistrarDevolucionAsync(int prestamoId, DateTime fechaDevolucion)
        {
            var prestamo = await _prestamoRepository.ObtenerPorIdAsync(prestamoId);
            if (prestamo == null) throw new Exception("Préstamo no encontrado.");

            prestamo.FechaDevolucion = fechaDevolucion;
            prestamo.Estado = "Devuelto";

            var articulo = await _articuloRepository.ObtenerPorIdAsync(prestamo.ArticuloId);
            articulo.Estado = "Disponible";

            await _prestamoRepository.ActualizarAsync(prestamo);
            await _articuloRepository.ActualizarAsync(articulo);

            await _prestamoRepository.GuardarCambiosAsync();
            await _articuloRepository.GuardarCambiosAsync();
        }
    }
}
