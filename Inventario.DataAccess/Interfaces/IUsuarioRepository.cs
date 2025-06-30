using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventario.Entities;

namespace Inventario.DataAccess.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorEmailAsync(string email);
        Task<Usuario?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(Usuario usuario);
        Task<List<Usuario>> ListarAsync();
        Task GuardarCambiosAsync();
        Task ActualizarAsync(Usuario usuario);
        Task EliminarAsync(Usuario usuario);

    }
}
