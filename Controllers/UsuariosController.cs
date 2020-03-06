using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CasaShowAPI.Data;
using CasaShowAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Cadastrar usuários.
        /// </summary>
        [HttpPost("registro")]
        public IActionResult Registro ([FromBody] Usuario usuario) {
            if (ModelState.IsValid)
            {
                try {
                    if (usuario.Email == null || usuario.Senha == null || usuario.Email.Length < 1 || usuario.Senha.Length < 1) {
                        Response.StatusCode = 400;
                        return new ObjectResult (new {msg = "Verifique se todos os campos foram preenhidos"});                
                    }
                    if (_context.Usuarios.Any(e => e.Email.Equals(usuario.Email))) {
                        Response.StatusCode = 400;
                        return new ObjectResult (new {msg = "Verifique se todos os campos foram preenhidos"});  
                    }
                    var hash = new Hash(SHA512.Create());
                    usuario.Senha = hash.CriptografarSenha(usuario.Senha);
                    _context.Add(usuario);
                    _context.SaveChanges();                
                    Response.StatusCode = 201;
                    return new ObjectResult ("Usuário cadastrado com sucesso");                
                } catch (Exception) {
                    Response.StatusCode = 404;

                    return new ObjectResult ("Insira os campos a serem cadastrados");
                }
            }
            Response.StatusCode = 404;

            return new ObjectResult ("");
        }

        /// <summary>
        /// Logar usuário.
        /// </summary>

        [HttpPost("Login")]
        public IActionResult Login ([FromBody] Usuario credenciais) {

            try {
                Usuario usuario = _context.Usuarios.First(user => user.Email.Equals(credenciais.Email));

                if (usuario != null) {                    
                    var hash = new Hash(SHA512.Create());
                    if (hash.VerificarSenha(credenciais.Senha, usuario.Senha)){
                        string chaveDeSeguranca = "casa_chave_de_seguranca_api";
                        var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeSeguranca));
                        var credenciaisDeAcesso = new SigningCredentials(chaveSimetrica, SecurityAlgorithms.HmacSha256Signature);

                        if (usuario.Admin == true) {
                            var claims = new List<Claim>();
                            claims.Add(new Claim("id", usuario.Id.ToString()));
                            claims.Add(new Claim("email", usuario.Email));
                            claims.Add(new Claim(ClaimTypes.Role, "Admin"));

                            var JWT = new JwtSecurityToken(
                                issuer: "casaapirest",
                                expires: DateTime.Now.AddHours(1),
                                audience: "usuario",
                                signingCredentials: credenciaisDeAcesso,
                                claims: claims
                            );
                            return Ok(new JwtSecurityTokenHandler().WriteToken(JWT));
                        } else {
                            var claims = new List<Claim>();
                            claims.Add(new Claim("id", usuario.Id.ToString()));
                            claims.Add(new Claim("email", usuario.Email));

                            var JWT = new JwtSecurityToken(
                                issuer: "casaapirest",
                                expires: DateTime.Now.AddHours(1),
                                audience: "usuario",
                                signingCredentials: credenciaisDeAcesso,
                                claims: claims
                            );
                            return Ok(new JwtSecurityTokenHandler().WriteToken(JWT));
                        }

                    } else {
                        Response.StatusCode = 401;
                        return new ObjectResult("Senha incorreta");
                    }          
                } else {
                    Response.StatusCode = 401;
                    return new ObjectResult("");
                }

            } catch (Exception){
                Response.StatusCode = 401;
                return new ObjectResult("Usuário não encontrado");
            }
        }

        /// <summary>
        /// Listar usuários.
        /// </summary>
        [HttpGet]
        // GET: User
        public IActionResult Index()
        {
            if (_context.Usuarios.Count() == 0) {                
                Response.StatusCode = 404;

                return new ObjectResult ("Não há usuário cadastrado");
            } else {
                return Ok(_context.Usuarios.ToList());
            }
        }        

        /// <summary>
        /// Busca por id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> BuscaId(int id)
        {
            if (_context.Usuarios.Where(cod => cod.Id == id).Count() != 0) {
                return Ok(await _context.Usuarios.Where(cod => cod.Id == id).ToListAsync());
            } else {

                Response.StatusCode = 404;

                return new ObjectResult ("Não encontrado");
            }
        }
    }
}