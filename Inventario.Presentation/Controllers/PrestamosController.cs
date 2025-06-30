using Inventario.Business.Interfaces;
using Inventario.DataAccess.Interfaces;
using Inventario.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Inventario.Presentation.Controllers
{
    [Authorize]
    public class PrestamosController : Controller
    {
        private readonly IPrestamoService _prestamoService;
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IArticuloRepository _articuloRepository;

        public PrestamosController(
            IPrestamoService prestamoService,
            IPrestamoRepository prestamoRepository,
            IArticuloRepository articuloRepository)
        {
            _prestamoService = prestamoService;
            _prestamoRepository = prestamoRepository;
            _articuloRepository = articuloRepository;
        }

        // GET: /Prestamos/MisSolicitudes
        public async Task<IActionResult> MisSolicitudes()
        {
            int usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var prestamos = await _prestamoRepository.ListarPorUsuarioAsync(usuarioId);
            return View(prestamos);
        }

        // GET: /Prestamos/Solicitar
        public async Task<IActionResult> Solicitar()
        {
            var disponibles = await _articuloRepository.ListarAsync();
            var articulos = disponibles.Where(a => a.Estado == "Disponible").ToList();
            return View(articulos);
        }

        // POST: /Prestamos/Solicitar
        [HttpPost]
        public async Task<IActionResult> Solicitar(int articuloId, DateTime fechaEntrega)
        {
            int usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                await _prestamoService.SolicitarPrestamoAsync(usuarioId, articuloId, fechaEntrega);
                TempData["Exito"] = "Solicitud registrada correctamente.";
                return RedirectToAction(nameof(MisSolicitudes));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Solicitar));
            }
        }

        // GET: /Prestamos/Pendientes
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Pendientes()
        {
            var todos = await _prestamoRepository.ListarAsync();
            var pendientes = todos.Where(p => p.Estado == "Pendiente").ToList();
            return View(pendientes);
        }

        // POST: /Prestamos/Aprobar
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Aprobar(int id)
        {
            await _prestamoService.AprobarPrestamoAsync(id);
            return RedirectToAction(nameof(Pendientes));
        }

        // POST: /Prestamos/Rechazar
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Rechazar(int id)
        {
            await _prestamoService.RechazarPrestamoAsync(id);
            return RedirectToAction(nameof(Pendientes));
        }

        // GET: /Prestamos/RegistrarDevolucion
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> RegistrarDevolucion()
        {
            var prestamos = await _prestamoRepository.ListarAsync();
            var enCurso = prestamos.Where(p => p.Estado == "Aprobado").ToList();
            return View(enCurso);
        }

        // POST: /Prestamos/Devolver
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Devolver(int id)
        {
            await _prestamoService.RegistrarDevolucionAsync(id, DateTime.Now);
            return RedirectToAction(nameof(RegistrarDevolucion));
        }

        // GET: /Prestamos
        public IActionResult Index()
        {
            return RedirectToAction(User.IsInRole("Administrador") ? "Pendientes" : "MisSolicitudes");
        }

    }
}
