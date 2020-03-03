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
            return Ok(_context.CasaShow.ToList());
        }
        /// <summary>
        /// Cadastrar casa de show.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CasaDeShow casaDeShow)
        {
            if (ModelState.IsValid)
            {
                if (casaDeShow.Nome == null || casaDeShow.Endereco == null || casaDeShow.Nome.Length < 1 || casaDeShow.Endereco.Length < 1) {
                Response.StatusCode = 400;
                return new ObjectResult (new {msg = "Verifique se todos os campos foram preenchidos"});                
                }
                _context.Add(casaDeShow);
                await _context.SaveChangesAsync();                
                Response.StatusCode = 201;
                return new ObjectResult ("");
                //return RedirectToAction(nameof(Index));
            }
            Response.StatusCode = 404;

            return new ObjectResult ("");
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

                return new ObjectResult ("");
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
                    var casa = _context.CasaShow.First(c => c.Id == casaDeShow.Id); 
                    if (casa != null) {
                        casa.Nome = casaDeShow.Nome != null && casaDeShow.Nome.Length > 1 ? casaDeShow.Nome : casa.Nome;
                        casa.Endereco = casaDeShow.Endereco != null && casaDeShow.Endereco.Length > 1 ? casaDeShow.Endereco : casa.Endereco;
                    }
                    await _context.SaveChangesAsync();
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
            } catch (Exception e) {

                Response.StatusCode = 404;

                return new ObjectResult ("");
            }
        }
        
        /// <summary>
        /// Listar em ordem alfabética crescente.
        /// </summary>
        [HttpGet("asc")]
        public async Task<IActionResult> NomeAsc()
        {
            return Ok(await _context.Eventos.OrderBy(nome => nome.Nome).ToListAsync());
        }
        
        /// <summary>
        /// Listar em ordem alfabética decrescente.
        /// </summary>
        [HttpGet("desc")]
        public async Task<IActionResult> NomeDesc()
        {
            return Ok(await _context.Eventos.OrderByDescending(nome => nome.Nome).ToListAsync());
        }

        /// <summary>
        /// Busca por nome.
        /// </summary>
        [HttpGet("nome/{nome}")]
        public async Task<IActionResult> Busca(string nome)
        {
            if (_context.Eventos.Where(n => n.Nome == nome).Count() != 0) {
                return Ok(await _context.Eventos.Where(n => n.Nome == nome).ToListAsync());
            } else {

                Response.StatusCode = 404;

                return new ObjectResult ("");
            }
        }


        private bool CasaDeShowExists(int id)
        {
            return _context.CasaShow.Any(e => e.Id == id);
        }
    }
}