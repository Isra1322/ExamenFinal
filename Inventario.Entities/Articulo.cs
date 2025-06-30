using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventario.Entities
{
    public class Articulo
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }

        [Required(ErrorMessage = "El campo Estado es obligatorio.")]
        public string Estado { get; set; }
        public string Ubicacion { get; set; }

        public ICollection<Prestamo> Prestamos { get; set; }
    }
}
