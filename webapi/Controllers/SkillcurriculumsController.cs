using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Models;
using webapi.DTOs.FieldService;
using System.ComponentModel.DataAnnotations;

namespace webapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SkillcurriculumsController : ControllerBase
    {
        private readonly BumpContext _context;

        public SkillcurriculumsController(BumpContext context)
        {
            _context = context;
        }


        // GET: api/Skillcurriculums
        [HttpGet]
        public async Task<ActionResult<Skillcurriculum>> GetSkillcurriculum([Required] int fieldId)
        {
            if (_context.Skillcurriculums == null)
            {
                return NotFound();
            }
            var skillcurriculum = await _context.Skillcurriculums
                .Include(sf => sf.Field)
                .Include(ss=>ss.SkillCourses)
                .Where(sf => sf.FieldId== fieldId)
                .Select(dto => dto.ToDto())
                .ToListAsync();

            if (skillcurriculum == null)
            {
                return NotFound();
            }

            return Ok(skillcurriculum);
        }


        // GET: api/Skillcurriculums/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Skillcurriculum>> GetOneSkillcurriculum([Required] int id)
        {
            if (_context.Skillcurriculums == null)
            {
                return NotFound();
            }
            var skillcurriculum = await _context.Skillcurriculums
                .Include(ss => ss.SkillCourses)
                .Include(sc=>sc.Coach)
                .Where (sc => sc.Id== id)
                .Select(dto => dto.ToGetSkillcurriculumDto())
                .FirstOrDefaultAsync()
                ;
            if (skillcurriculum == null)
            {
                return NotFound();
            }
            return Ok(skillcurriculum);
        }





        // PUT: api/Skillcurriculums/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSkillcurriculum(int id, Skillcurriculum skillcurriculum)
        {
            if (id != skillcurriculum.Id)
            {
                return BadRequest();
            }

            _context.Entry(skillcurriculum).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SkillcurriculumExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Skillcurriculums
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Skillcurriculum>> PostSkillcurriculum(Skillcurriculum skillcurriculum)
        {
          if (_context.Skillcurriculums == null)
          {
              return Problem("Entity set 'BumpContext.Skillcurriculums'  is null.");
          }
            _context.Skillcurriculums.Add(skillcurriculum);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSkillcurriculum", new { id = skillcurriculum.Id }, skillcurriculum);
        }

        // DELETE: api/Skillcurriculums/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkillcurriculum(int id)
        {
            if (_context.Skillcurriculums == null)
            {
                return NotFound();
            }
            var skillcurriculum = await _context.Skillcurriculums.FindAsync(id);
            if (skillcurriculum == null)
            {
                return NotFound();
            }

            _context.Skillcurriculums.Remove(skillcurriculum);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SkillcurriculumExists(int id)
        {
            return (_context.Skillcurriculums?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
