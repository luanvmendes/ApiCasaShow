using System.Linq;
using System.Threading.Tasks;
using CasaShowAPI.Data;
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
        private IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _context;

        public VendasController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Ok();
        }
        
    }
}