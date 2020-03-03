using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CasaShowAPI.Data;
using CasaShowAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CasaShowAPI.Controllers
{
    [Route("eventos")]
    [ApiController]
    public class EventoController : ControllerBase
    {
        private IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _context;

        public EventoController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categorias = _context.Categorias.ToList();
            var casa = _context.CasaShow.ToList();
            return Ok(await _context.Eventos.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Evento evento)
        {
            if (ModelState.IsValid)
            {
                evento.CasaShow = _context.CasaShow.First(cs => cs.Id == evento.CasaShow.Id);
                evento.Categoria = _context.Categorias.First(ctg => ctg.Id == evento.Categoria.Id);
                _context.Add(evento);
                await _context.SaveChangesAsync();                
                Response.StatusCode = 201;
                return new ObjectResult ("");
            }
            Response.StatusCode = 404;

            return new ObjectResult ("");
        }

        [HttpPatch]
        public async Task<IActionResult> Edit([FromBody] Evento evento)
        {
            if (evento.Id == 0)
            {
                return NotFound("Id inválido");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    evento.CasaShow = _context.CasaShow.First(cs => cs.Id == evento.CasaShow.Id);
                    evento.Categoria = _context.Categorias.First(ctg => ctg.Id == evento.Categoria.Id);
                    if (evento.Imagem != null) {
                        evento.Imagem = evento.Imagem;
                    }                    
                    _context.Update(evento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventoExists(evento.Id))
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
            try{
                var evento = await _context.Eventos.FindAsync(id);
                _context.Eventos.Remove(evento);
                await _context.SaveChangesAsync();
                return Ok();
            } catch (Exception e) {

                Response.StatusCode = 404;

                return new ObjectResult ("");
            }
        }

        private bool EventoExists(int id)
        {
            return _context.Eventos.Any(e => e.Id == id);
        }
    }
}