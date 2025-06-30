using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventario.DataAccess.Interfaces;
using Inventario.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventario.DataAccess.Repositories
{
    public class PrestamoRepository : IPrestamoRepository
    {
        private readonly AppDbContext _context;

        public PrestamoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Prestamo?> ObtenerPorIdAsync(int id)
        {
            return await _context.Prestamos
                .Include(p => p.Usuario)
                .Include(p => p.Articulo)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Prestamo>> ListarAsync()
        {
            return await _context.Prestamos
                .Include(p => p.Usuario)
                .Include(p => p.Articulo)
                .ToListAsync();
        }

        public async Task<List<Prestamo>> ListarPorUsuarioAsync(int usuarioId)
        {
            return await _context.Prestamos
                .Include(p => p.Articulo)
                .Where(p => p.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task<List<Prestamo>> ListarPorArticuloAsync(int articuloId)
        {
            return await _context.Prestamos
                .Include(p => p.Usuario)
                .Where(p => p.ArticuloId == articuloId)
                .ToListAsync();
        }

        public async Task AgregarAsync(Prestamo prestamo)
        {
            await _context.Prestamos.AddAsync(prestamo);
        }

        public async Task ActualizarAsync(Prestamo prestamo)
        {
            _context.Prestamos.Update(prestamo);
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}