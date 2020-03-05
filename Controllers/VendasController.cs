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
            return Ok();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(int id, [FromBody] Venda venda)
        {
            if (ModelState.IsValid)
            {
                var evento = _context.Eventos.First(x => x.Id == id);
                venda.User = _context.Usuarios.First(user => user.Id == venda.User.Id);
                venda.Evento = evento;
                venda.Total *= venda.Quantidade;
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