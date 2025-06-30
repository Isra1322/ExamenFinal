using Inventario.Business.Interfaces;
using Inventario.DataAccess.Interfaces;
using Inventario.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventario.Presentation.Controllers
{
    [Authorize]
    public class ArticulosController : Controller
    {
        private readonly IArticuloRepository _articuloRepository;
        private readonly IPrestamoRepository _prestamoRepository;

        public ArticulosController(IArticuloRepository articuloRepository, IPrestamoRepository prestamoRepository)
        {
            _articuloRepository = articuloRepository;
            _prestamoRepository = prestamoRepository;
        }

        // GET: /Articulos
        public async Task<IActionResult> Index(string? filtro, string? categoria)
        {
            var articulos = await _articuloRepository.ListarAsync();

            if (!string.IsNullOrEmpty(filtro))
            {
                articulos = articulos.Where(a =>
                    a.Nombre.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                    a.Codigo.Contains(filtro, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(categoria))
            {
                articulos = articulos.Where(a => a.Categoria == categoria).ToList();
            }

            ViewBag.Filtro = filtro;
            ViewBag.Categoria = categoria;
            return View(articulos);
        }

        // GET: /Articulos/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            ViewBag.Estados = new SelectList(new[] { "Disponible", "Prestado", "Mantenimiento" });
            return View();
        }

        // POST: /Articulos/Create
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create(Articulo articulo)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Estados = new SelectList(new[] { "Disponible", "Prestado", "Mantenimiento" });
                return View(articulo);
            }

            var existente = await _articuloRepository.ObtenerPorCodigoAsync(articulo.Codigo);
            if (existente != null)
            {
                ModelState.AddModelError("Codigo", "El código ya existe.");
                ViewBag.Estados = new SelectList(new[] { "Disponible", "Prestado", "Mantenimiento" });
                return View(articulo);
            }

            await _articuloRepository.AgregarAsync(articulo);
            await _articuloRepository.GuardarCambiosAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: /Articulos/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id)
        {
            var articulo = await _articuloRepository.ObtenerPorIdAsync(id);
            if (articulo == null) return NotFound();

            return View(articulo);
        }

        // POST: /Articulos/Edit/5
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, Articulo articulo)
        {
            if (id != articulo.Id) return BadRequest();

            var existente = await _articuloRepository.ObtenerPorIdAsync(id);
            if (existente == null) return NotFound();

            existente.Nombre = articulo.Nombre;
            existente.Categoria = articulo.Categoria;
            existente.Estado = articulo.Estado;
            existente.Ubicacion = articulo.Ubicacion;

            await _articuloRepository.ActualizarAsync(existente);
            await _articuloRepository.GuardarCambiosAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Articulos/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var articulo = await _articuloRepository.ObtenerPorIdAsync(id);
            if (articulo == null) return NotFound();

            var prestamos = await _prestamoRepository.ListarPorArticuloAsync(id);
            bool tienePrestamoActivo = prestamos.Any(p => p.Estado == "Pendiente" || p.Estado == "Aprobado");

            ViewBag.TienePrestamoActivo = tienePrestamoActivo;
            return View(articulo);
        }

        // POST: /Articulos/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var articulo = await _articuloRepository.ObtenerPorIdAsync(id);
            if (articulo == null) return NotFound();

            var prestamos = await _prestamoRepository.ListarPorArticuloAsync(id);
            bool tienePrestamoActivo = prestamos.Any(p => p.Estado == "Pendiente" || p.Estado == "Aprobado");
            if (tienePrestamoActivo)
            {
                TempData["Error"] = "No se puede eliminar un artículo con préstamos activos.";
                return RedirectToAction(nameof(Index));
            }

            await _articuloRepository.EliminarAsync(articulo);
            await _articuloRepository.GuardarCambiosAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
