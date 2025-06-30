using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.Presentation.Controllers
{
    [Authorize(Roles = "Operador")]
    public class OperadorController : Controller
    {
        public IActionResult Panel()
        {
            return View();
        }
    }
}
