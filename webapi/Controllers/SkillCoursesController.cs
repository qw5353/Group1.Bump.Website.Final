using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using webapi.DTOs.FieldService;
using webapi.Models;

namespace webapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SkillCoursesController : ControllerBase
    {
        private readonly BumpContext _context;

        public SkillCoursesController(BumpContext context)
        {
            _context = context;
        }

        // GET: api/SkillCourses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<skillcourseDto>>> GetSkillCourses()
        {
          if (_context.SkillCourses == null)
          {
              return NotFound();
          }
           var skillcourses =await _context.SkillCourses
                .Select(sk=>sk.ToDto())
                .ToListAsync();
            return Ok(skillcourses);
        }

        

        private bool SkillCourseExists(int id)
        {
            return (_context.SkillCourses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
