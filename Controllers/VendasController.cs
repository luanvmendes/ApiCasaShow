using System;
using System.Linq;
using System.Threading.Tasks;
using CasaShowAPI.Data;
using CasaShowAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CasaShowAPI.Controllers
{
    [Route("vendas")]
    [ApiController]
    [Authorize]
    public class VendasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VendasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.User.Claims.First(claim => claim.Type.ToString().Equals("id", StringComparison.InvariantCultureIgnoreCase)).Value;
            if (!_context.Vendas.Any(x => x.User.Id == int.Parse(userId))) {                
                Response.StatusCode = 404;

                return new ObjectResult ("Você ainda não fez nenhuma compra");
            } else {
                var casa = _context.CasaShow.ToList();
                var ctg = _context.Categorias.ToList();
                var user = _context.Usuarios.ToList();
                return Ok(await _context.Vendas.Include(x => x.Evento).Where(x => x.User.Id == int.Parse(userId)).ToListAsync());
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Venda venda)
        {
            if (ModelState.IsValid)
            {
                if (venda.Data == null || venda.Evento == null) {                    
                        Response.StatusCode = 400;
                        return new ObjectResult (new {msg = "Verifique se todos os campos foram preenchidos"});  
                } else if (venda.Quantidade <= 0) {
                        Response.StatusCode = 400;
                        return new ObjectResult (new {msg = "Insira a quantidade"});  
                } else if (!_context.Eventos.Any(id => id.Id == venda.Evento.Id)) {
                        Response.StatusCode = 400;
                        return new ObjectResult (new {msg = "Verifique o id do evento"});  
                }                
                var userId = HttpContext.User.Claims.First(claim => claim.Type.ToString().Equals("id", StringComparison.InvariantCultureIgnoreCase)).Value;
                var evento = _context.Eventos.First(x => x.Id == venda.Evento.Id);
                venda.User = _context.Usuarios.First(user => user.Id == int.Parse(userId));
                venda.Evento = evento;
                venda.Total = venda.Quantidade * evento.ValorIngresso;
                evento.Capacidade -= venda.Quantidade;
                _context.Update(evento);
                _context.Add(venda);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return Ok(venda);
        }
    }
}