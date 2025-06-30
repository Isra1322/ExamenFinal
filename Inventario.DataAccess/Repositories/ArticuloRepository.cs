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
    public class ArticuloRepository : IArticuloRepository
    {
        private readonly AppDbContext _context;

        public ArticuloRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Articulo?> ObtenerPorIdAsync(int id) =>
            await _context.Articulos.FindAsync(id);

        public async Task<Articulo?> ObtenerPorCodigoAsync(string codigo) =>
            await _context.Articulos.FirstOrDefaultAsync(a => a.Codigo == codigo);

        public async Task<List<Articulo>> ListarAsync() =>
            await _context.Articulos.ToListAsync();

        public async Task AgregarAsync(Articulo articulo) =>
            await _context.Articulos.AddAsync(articulo);

        public async Task ActualizarAsync(Articulo articulo) =>
            _context.Articulos.Update(articulo);

        public async Task EliminarAsync(Articulo articulo) =>
            _context.Articulos.Remove(articulo);

        public async Task GuardarCambiosAsync() =>
            await _context.SaveChangesAsync();
    }

}
