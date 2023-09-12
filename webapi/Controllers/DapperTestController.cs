using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Context;
using webapi.Models;
using webapi.Services;

namespace webapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DapperTestController : ControllerBase
    {
        private readonly BumpDapperContext _context;
        private readonly BumpContext _bumpContext;
        private readonly PayService _payService;
        public DapperTestController(BumpDapperContext context, BumpContext bumpContext, PayService payService)
        {
            _context = context;
            _bumpContext = bumpContext;
            _payService = payService;
        }

        [HttpGet]
        [Route("products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            using (var conn = _context.CreateConnection())
            {
                string sql = "SELECT TOP 10 * FROM Products";

                return Ok(await conn.QueryAsync<Product>(sql));
            }
        }

        [HttpGet]
        [Route("/test")]
        public async Task<ActionResult<IEnumerable<Skillcurriculum>>> GetCurriculum()
        {
            return await _bumpContext.Skillcurriculums.Take(1).ToArrayAsync();
        }
    }
}
