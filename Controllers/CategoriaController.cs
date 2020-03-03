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
    [Route("categorias")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CategoriaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        // GET: Categoria
        public async Task<IActionResult> Index()
        {
            return Ok(await _context.Categorias.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                if (categoria.Nome == null || categoria.Nome.Length < 1) {
                Response.StatusCode = 400;
                return new ObjectResult (new {msg = "Insira o nome da categoria"});                
                }
                _context.Add(categoria);
                await _context.SaveChangesAsync();                
                Response.StatusCode = 201;
                return new ObjectResult ("");
            }
            Response.StatusCode = 404;

            return new ObjectResult ("");
        }

        [HttpPatch]
        public async Task<IActionResult> Edit([FromBody] Categoria categoria)
        {
            if (categoria.Id == 0)
            {
                return NotFound("Id inválido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoria);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaExists(categoria.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try {
                var categoria = await _context.Categorias.FindAsync(id);
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();return Ok();
            } catch (Exception e) {

                Response.StatusCode = 404;

                return new ObjectResult ("");
            }
        }

        private bool CategoriaExists(int id)
        {
            return _context.Categorias.Any(e => e.Id == id);
        }
    }
}