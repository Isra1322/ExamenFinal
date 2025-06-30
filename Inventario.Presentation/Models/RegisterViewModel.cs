using System.ComponentModel.DataAnnotations;
using Inventario.Entities;

namespace Inventario.Presentation.Models
{
    public class RegisterViewModel
    {
        [Required]
        public required string Nombre { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Contrasena { get; set; }

        public Rol Rol { get; set; }
    }
}
