using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.DTOs.CustomerService;
using webapi.Helpers;
using webapi.Models;

namespace webapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FAQController : ControllerBase
    {
        private readonly BumpContext _context;
        private readonly IEmailHelper _emailHelper;

        public FAQController(BumpContext context, IEmailHelper emailHelper)
        {
            _context = context;
            _emailHelper = emailHelper;
        }

        [HttpGet]
        [Route("types")]
		[ResponseCache(Duration = 3600)]
		public async Task<ActionResult<IEnumerable<object>>> GetTypes()
        {
            if (_context.TrendingQuestionTypes == null)
            {
                return NotFound();
            }

            return await _context.TrendingQuestionTypes
                .Select(tqt => new
                {
                    typeId = tqt.Id,
                    name = tqt.Name,
                })
                .AsNoTracking()
                .ToListAsync();
        }

        // GET: api/FAQ
        [HttpGet]
        [ResponseCache(Duration = 3600)]
        public async Task<ActionResult<IEnumerable<FAQListDto>>> GetTrendingQuestions(int typeId = 1)
        {
          if (_context.TrendingQuestions == null)
          {
              return NotFound();
          }
            return await _context
                .TrendingQuestions
                .Include(tq => tq.QuestionType)
                .Where(tq => tq.QuestionTypeId == typeId)
                .Select(tq => new FAQListDto
                {
                    Id = tq.Id,
                    QuestionTypeId = tq.QuestionTypeId,
                    QuestionType = tq.QuestionType.Name,
                    Title = tq.Title,
                    Description = tq.Description,
                })
                .AsNoTracking()
                .ToListAsync();
        }

        [HttpGet("Test")]
        public void Test()
        {
            _emailHelper.SendEmail(new Email
            {
                Receiver = "leojudya@gmail.com",
                Subject = "Test",
                Body = @"<h1>恭喜妳中了IPHONE</h1>",
                isHtml = true
            });
        }

    }
}
