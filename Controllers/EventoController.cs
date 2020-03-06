using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CasaShowAPI.Data;
using CasaShowAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CasaShowAPI.Controllers
{
    [Route("eventos")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class EventoController : ControllerBase
    {
        private IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _context;

        public EventoController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _context = context;
        }

        /// <summary>
        /// Listar evento.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (_context.Eventos.Count() == 0) {                
                Response.StatusCode = 404;

                return new ObjectResult ("Não há evento cadastrado");
            } else {
               // var categorias = _context.Categorias.ToList();
               // var casa = _context.CasaShow.ToList();
                return Ok(await _context.Eventos.Select(dados => new {
                    dados.Id, dados.Nome, dados.Capacidade, dados.Data, dados.ValorIngresso, CasadeShow = dados.CasaShow.Nome, Gênero = dados.Categoria.Nome, dados.Imagem
                }).ToListAsync());
            }
        }

        /// <summary>
        /// Cadastrar evento.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Evento evento)
        {
            if (ModelState.IsValid)
            {
                if (_context.CasaShow.Count() == 0) {
                    Response.StatusCode = 404;

                    return new ObjectResult ("Cadastre uma casa de show antes de criar um evento");

                } else if (_context.Categorias.Count() == 0) {                    
                    Response.StatusCode = 404;

                    return new ObjectResult ("Cadastre uma categoria antes de criar um evento");
                }
                try{
                    if (evento.Nome == null || evento.CasaShow == null || evento.Categoria == null || evento.Data == null || evento.Imagem == null 
                    || evento.Nome.Length < 1 || evento.Imagem.Length < 1) {
                        Response.StatusCode = 400;
                        return new ObjectResult (new {msg = "Verifique se todos os campos foram preenchidos"});                
                    }
                    if (!_context.CasaShow.Any(id => id.Id == evento.CasaShow.Id)) {
                        Response.StatusCode = 400;
                        return new ObjectResult (new {msg = "Verifique o id da casa de show"});  
                    }
                    if (!_context.Categorias.Any(id => id.Id == evento.Categoria.Id)) {
                        Response.StatusCode = 400;
                        return new ObjectResult (new {msg = "Verifique o id da categoria"});  
                    }
                    evento.CasaShow = _context.CasaShow.First(cs => cs.Id == evento.CasaShow.Id);
                    evento.Categoria = _context.Categorias.First(ctg => ctg.Id == evento.Categoria.Id);
                    _context.Add(evento);
                    await _context.SaveChangesAsync();                
                    Response.StatusCode = 201;
                    return new ObjectResult ("");
                } catch (Exception) {

                    Response.StatusCode = 404;

                    return new ObjectResult ("Insira os campos a serem cadastrados");
                }
            }
            Response.StatusCode = 404;

            return new ObjectResult ("");
        }

        /// <summary>
        /// Editar evento.
        /// </summary>
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

        /// <summary>
        /// Remover evento.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try{
                var evento = await _context.Eventos.FindAsync(id);
                _context.Eventos.Remove(evento);
                await _context.SaveChangesAsync();
                return Ok();
            } catch (Exception) {

                Response.StatusCode = 404;

                return new ObjectResult ("");
            }
        }

        /// <summary>
        /// Busca por id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> BuscaId(int id)
        {
            if (_context.Eventos.Where(cod => cod.Id == id).Count() != 0) {
                return Ok(await _context.Eventos.Where(cod => cod.Id == id).Select(dados => new {
                    dados.Id, dados.Nome, dados.Capacidade, dados.Data, dados.ValorIngresso, CasadeShow = dados.CasaShow.Nome, Gênero = dados.Categoria.Nome, dados.Imagem
                }).ToListAsync());
            } else {

                Response.StatusCode = 404;

                return new ObjectResult ("Não encontrado");
            }
        }
        
        /// <summary>
        /// Listar em ordem alfabética crescente.
        /// </summary>
        [HttpGet("nome/asc")]
        public async Task<IActionResult> NomeAsc()
        {
            return Ok(await _context.Eventos.OrderBy(nome => nome.Nome).ToListAsync());
        }
        
        /// <summary>
        /// Listar em ordem alfabética decrescente.
        /// </summary>
        [HttpGet("nome/desc")]
        public async Task<IActionResult> NomeDesc()
        {
            return Ok(await _context.Eventos.OrderByDescending(nome => nome.Nome).ToListAsync());
        }
        
        /// <summary>
        /// Listar por data crescente.
        /// </summary>
        [HttpGet("data/asc")]
        public async Task<IActionResult> DataAsc()
        {
            return Ok(await _context.Eventos.OrderBy(d => d.Data).ToListAsync());
        }
        
        /// <summary>
        /// Listar por data decrescente.
        /// </summary>
        [HttpGet("data/desc")]
        public async Task<IActionResult> DataDesc()
        {
            return Ok(await _context.Eventos.OrderByDescending(d => d.Data).ToListAsync());
        }
        
        /// <summary>
        /// Listar por capacidade crescente.
        /// </summary>
        [HttpGet("capacidade/asc")]
        public async Task<IActionResult> CapacidadeAsc()
        {
            return Ok(await _context.Eventos.OrderBy(c => c.Capacidade).ToListAsync());
        }
        
        /// <summary>
        /// Listar por capacidade decrescente.
        /// </summary>
        [HttpGet("capacidade/desc")]
        public async Task<IActionResult> CapacidadeDesc()
        {
            return Ok(await _context.Eventos.OrderByDescending(c => c.Capacidade).ToListAsync());
        }
        
        /// <summary>
        /// Listar por preço crescente.
        /// </summary>
        [HttpGet("preco/asc")]
        public async Task<IActionResult> PrecoAsc()
        {
            return Ok(await _context.Eventos.OrderBy(p => p.ValorIngresso).ToListAsync());
        }
        
        /// <summary>
        /// Listar por preço decrescente.
        /// </summary>
        [HttpGet("preco/desc")]
        public async Task<IActionResult> PrecoDesc()
        {
            return Ok(await _context.Eventos.OrderByDescending(p => p.ValorIngresso).ToListAsync());
        }

        private bool EventoExists(int id)
        {
            return _context.Eventos.Any(e => e.Id == id);
        }
    }
}