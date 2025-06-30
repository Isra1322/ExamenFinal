using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventario.Entities
{
    public class Prestamo
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int ArticuloId { get; set; }
        public Articulo Articulo { get; set; }

        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public string Estado { get; set; }
    }
}
