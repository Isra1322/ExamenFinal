using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Inventario.Business.Interfaces;
using Inventario.Entities;
using Inventario.DataAccess.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Inventario.Business.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Usuario> RegistrarAsync(string nombre, string email, string contraseña, Rol rol)
        {
            var existe = await _usuarioRepository.ObtenerPorEmailAsync(email);
            if (existe != null)
                throw new Exception("El correo ya está registrado.");

            var usuario = new Usuario
            {
                Nombre = nombre,
                Email = email,
                ContrasenaHash = HashPassword(contraseña),
                Rol = rol
            };

            await _usuarioRepository.AgregarAsync(usuario);
            await _usuarioRepository.GuardarCambiosAsync();

            return usuario;
        }

        public async Task<Usuario?> LoginAsync(string email, string contraseña)
        {
            var usuario = await _usuarioRepository.ObtenerPorEmailAsync(email);
            if (usuario == null || !VerificarPassword(contraseña, usuario.ContrasenaHash))
                return null;

            return usuario;
        }

        // Puedes reemplazar esto por ASP.NET Identity más adelante
        private string HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return $"{Convert.ToBase64String(salt)}.{hash}";
        }

        private bool VerificarPassword(string password, string hashCompleto)
        {
            var partes = hashCompleto.Split('.');
            if (partes.Length != 2) return false;

            var salt = Convert.FromBase64String(partes[0]);
            var hashEsperado = partes[1];

            var hashReal = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashReal == hashEsperado;
        }
    }
}
