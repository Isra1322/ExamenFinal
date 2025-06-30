using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventario.Entities;

namespace Inventario.Business.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario> RegistrarAsync(string nombre, string email, string contraseña, Rol rol);
        Task<Usuario?> LoginAsync(string email, string contraseña);
    }
}
