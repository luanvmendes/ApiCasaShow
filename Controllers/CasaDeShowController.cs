using System;
using System.Linq;
using System.Threading.Tasks;
using CasaShowAPI.Data;
using CasaShowAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CasaShowAPI.Controllers
{
    [Route("casadeshow")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CasaDeShowController: ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CasaDeShowController (ApplicationDbContext context) 
        {
            _context = context;
        }
        
        /// <summary>
        /// Listar casas de show cadastradas.
        /// </summary>
        [HttpGet]
        // GET: CasaDeShow
        public IActionResult Index()
        {
            if (_context.CasaShow.Count() == 0) {                
                Response.StatusCode = 404;

                return new ObjectResult ("Não há casa de show cadastrada");
            } else {
                return Ok(_context.CasaShow.ToList());
            }
        }
        /// <summary>
        /// Cadastrar casa de show.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CasaDeShow casaDeShow)
        {
            if (ModelState.IsValid)
            {
                try {
                    if (casaDeShow.Nome == null || casaDeShow.Endereco == null || casaDeShow.Nome.Length < 1 || casaDeShow.Endereco.Length < 1) {
                        Response.StatusCode = 400;
                        return new ObjectResult (new {msg = "Verifique se todos os campos foram preenchidos"});                
                    }
                    _context.Add(casaDeShow);
                    await _context.SaveChangesAsync();                
                    Response.StatusCode = 201;
                    return new ObjectResult ("");
                    //return RedirectToAction(nameof(Index));
                } catch (Exception) {
                    Response.StatusCode = 404;

                    return new ObjectResult ("Insira os campos a serem cadastrados");
                }
            } else {
                return BadRequest();
            }
        }

        /// <summary>
        /// Editar casa de show.
        /// </summary>
        [HttpPatch]
        public async Task<IActionResult> Edit([FromBody] CasaDeShow casaDeShow)
        {
            if (casaDeShow.Id == 0)
            {
                return NotFound("Id inválido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (CasaDeShowExists(casaDeShow.Id)) {
                        var casa = _context.CasaShow.First(c => c.Id == casaDeShow.Id); 
                        if (casa != null) {
                            casa.Nome = casaDeShow.Nome != null && casaDeShow.Nome.Length > 1 ? casaDeShow.Nome : casa.Nome;
                            casa.Endereco = casaDeShow.Endereco != null && casaDeShow.Endereco.Length > 1 ? casaDeShow.Endereco : casa.Endereco;
                        }
                        await _context.SaveChangesAsync();
                    } else {
                        return NotFound();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CasaDeShowExists(casaDeShow.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok();
                //return RedirectToAction(nameof(Index));
            }
            return BadRequest();
        }

        /// <summary>
        /// Remove uma casa de show.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try {
                var casaDeShow = await _context.CasaShow.FindAsync(id);
                _context.CasaShow.Remove(casaDeShow);
                await _context.SaveChangesAsync();
                return Ok();
            } catch (Exception) {

                Response.StatusCode = 404;

                return new ObjectResult ("Id inválido");
            }
        }

        /// <summary>
        /// Busca por id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> BuscaId(int id)
        {
            if (_context.CasaShow.Where(cod => cod.Id == id).Count() != 0) {
                return Ok(await _context.CasaShow.Where(cod => cod.Id == id).ToListAsync());
            } else {

                Response.StatusCode = 404;

                return new ObjectResult ("Não encontrado");
            }
        }
        
        /// <summary>
        /// Listar em ordem alfabética crescente.
        /// </summary>
        [HttpGet("asc")]
        public async Task<IActionResult> NomeAsc()
        {
            return Ok(await _context.CasaShow.OrderBy(nome => nome.Nome).ToListAsync());
        }
        
        /// <summary>
        /// Listar em ordem alfabética decrescente.
        /// </summary>
        [HttpGet("desc")]
        public async Task<IActionResult> NomeDesc()
        {
            return Ok(await _context.CasaShow.OrderByDescending(nome => nome.Nome).ToListAsync());
        }

        /// <summary>
        /// Busca por nome.
        /// </summary>
        [HttpGet("nome/{nome}")]
        public async Task<IActionResult> Busca(string nome)
        {
            //Busca ignorando case sensitive
            if (_context.CasaShow.Where(n => n.Nome.StartsWith(nome, StringComparison.InvariantCultureIgnoreCase)).Count() != 0) {
                return Ok(await _context.CasaShow.Where(n => n.Nome.StartsWith(nome, StringComparison.InvariantCultureIgnoreCase)).ToListAsync());
            } else {

                Response.StatusCode = 404;

                return new ObjectResult ("Não encontrado");
            }
        }


        private bool CasaDeShowExists(int id)
        {
            return _context.CasaShow.Any(e => e.Id == id);
        }
    }
}