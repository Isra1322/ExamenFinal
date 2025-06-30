using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventario.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string ContrasenaHash { get; set; }
        public Rol Rol { get; set; }

        public ICollection<Prestamo> Prestamos { get; set; }
    }
}
