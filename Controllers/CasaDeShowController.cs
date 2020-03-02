using System;
using System.Linq;
using System.Threading.Tasks;
using CasaShowAPI.Data;
using CasaShowAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CasaShowAPI.Controllers
{
    [Route("")]
    [ApiController]
    public class CasaDeShowController: ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CasaDeShowController (ApplicationDbContext context) 
        {
            _context = context;
        }
        
        [HttpGet]
        // GET: CasaDeShow
        public IActionResult Index()
        {
            return Ok(_context.CasaShow.ToList());
        }

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
                    _context.Update(casaDeShow);
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


        private bool CasaDeShowExists(int id)
        {
            return _context.CasaShow.Any(e => e.Id == id);
        }
    }
}