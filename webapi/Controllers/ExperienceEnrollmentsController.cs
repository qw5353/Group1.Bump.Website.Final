using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.DTOs.FieldService;
using webapi.Models;

namespace webapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExperienceEnrollmentsController : ControllerBase
    {
        private readonly BumpContext _context;

        public ExperienceEnrollmentsController(BumpContext context)
        {
            _context = context;
        }


        // POST: api/ExperienceEnrollments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ExperienceEnrollment>> PostExperienceEnrollment(ExperienceEnrollmentDto dto)
        {
            if (_context.ExperienceEnrollments == null)
            {
                return Problem("Entity set 'BumpContext.ExperienceEnrollments'  is null.");
            }
            dto.Ststus = false;
            dto.CreatedAt = DateTime.Now;
            dto.CoachId = null;
            if (dto.NumberOfPeople == "1") { dto.ExperienceCoursePlanId = 1; }
            else if (dto.NumberOfPeople == "2") { dto.ExperienceCoursePlanId = 2; }
            else dto.ExperienceCoursePlanId = 3;
            var experienceEnrollment = dto.ToEntity();
            _context.ExperienceEnrollments.Add(experienceEnrollment);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetExperienceEnrollment", new { id = experienceEnrollment.Id }, experienceEnrollment);
        }

        [HttpGet]

        public async Task<ActionResult<ExperienceEnrollmentDto>> GetExperienceEnrollment()
        {
            if (_context.ExperienceEnrollments == null)
            {
                return NotFound();
            }

            var latestEnrollment = await _context.ExperienceEnrollments

                                              .Include(ef => ef.Field)
                                              .Include(em => em.Member)
                                              .OrderByDescending(e => e.Id)
                                              .Select(dto => dto.ToDto())
                                              .FirstOrDefaultAsync();

            if (latestEnrollment == null)
            {
                return NotFound(); // Return appropriate response if no data is found
            }

            return Ok(latestEnrollment);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExperienceEnrollmentDto>> GetExperienceEnrollment(int id)
        {
            if (_context.ExperienceEnrollments == null)
            {
                return NotFound();
            }

            var latestEnrollment = await _context.ExperienceEnrollments
                                              .Include(ef => ef.Field)
                                              .Include(em => em.Member)
                                              .Where(s => s.MemberId == id)
                                              .OrderByDescending(e => e.Id)
                                              .Select(dto => dto.ToDto())
                                              .ToListAsync();

            if (latestEnrollment == null)
            {
                return NotFound(); // Return appropriate response if no data is found
            }

            return Ok(latestEnrollment);

        }

        private bool ExperienceEnrollmentExists(int id)
        {
            return (_context.ExperienceEnrollments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
