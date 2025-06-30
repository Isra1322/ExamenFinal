using Inventario.Business.Interfaces;
using Inventario.DataAccess.Interfaces;
using Inventario.DataAccess.Repositories;
using Inventario.Entities;
using Inventario.Presentation.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Inventario.Presentation.Controllers
{
    public class UsuariosController(IUsuarioService usuarioService, IUsuarioRepository usuarioRepository) : Controller
    {
        private readonly IUsuarioService _usuarioService = usuarioService;
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

        // GET: /Usuarios/Login
        [HttpGet]
        public IActionResult Login() => View();

        // POST: /Usuarios/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var usuario = await _usuarioService.LoginAsync(model.Email, model.Contrasena);
            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
                return View(model);
            }

            // Crear Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (usuario.Rol == Rol.Administrador)
            {
                return RedirectToAction("Panel", "Admin");
            }
            else if (usuario.Rol == Rol.Operador)
            {
                return RedirectToAction("Panel", "Operador");
            }

            return RedirectToAction("Index", "Home"); // Fallback

        }

        // GET: /Usuarios/Register
        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Roles = new SelectList(Enum.GetValues(typeof(Rol)));
            return View();
        }

        // POST: /Usuarios/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var usuario = await _usuarioService.RegistrarAsync(model.Nombre, model.Email, model.Contrasena, model.Rol);

                // Login inmediato
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.Rol.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                if (usuario.Rol == Rol.Administrador)
                {
                    return RedirectToAction("Panel", "Admin");
                }
                else if (usuario.Rol == Rol.Operador)
                {
                    return RedirectToAction("Panel", "Operador");
                }

                return RedirectToAction("Usuario", "Login"); // fallback por si acaso

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // /Usuarios/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Usuarios");
        }

        // GET: /Usuarios
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioRepository.ListarAsync();
            return View(usuarios);
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(id);
            if (usuario == null) return NotFound();

            var model = new RegisterViewModel
            {
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol,
                Contrasena = ""
            };

            ViewBag.Roles = new SelectList(Enum.GetValues(typeof(Rol)));
            ViewBag.IdUsuario = id;

            return View(model);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var usuario = await _usuarioRepository.ObtenerPorIdAsync(id);
            if (usuario == null) return NotFound();

            usuario.Nombre = model.Nombre;
            usuario.Email = model.Email;
            usuario.Rol = model.Rol;

            await _usuarioRepository.ActualizarAsync(usuario);
            await _usuarioRepository.GuardarCambiosAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(id);
            if (usuario == null) return NotFound();

            await _usuarioRepository.EliminarAsync(usuario);
            await _usuarioRepository.GuardarCambiosAsync();

            return RedirectToAction("Index");
        }

    }
}
