using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.DTOs.Activities;
using webapi.Models;

namespace webapi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ActivitiesController : ControllerBase
	{
		private readonly BumpContext _context;

		public ActivitiesController(BumpContext context)
		{
			_context = context;
		}

		[HttpGet]
		[Route("/AllActivities")]
		public async Task<ActionResult<IEnumerable<ActivityListDto>>> GetAllActivities()
		{
			var activities = await _context.Activities
				.Include(a => a.ActivityDetails)
				.ThenInclude(ad => ad.ActivityDiscounts)
				.ThenInclude(adc => adc.PromotionProductType)
				.Include(a => a.ActivityDetails)
				.ThenInclude(ad => ad.ActivityDiscounts)
				.ThenInclude(adc => adc.ProductTags)
				.ToListAsync();
			if (activities.Count == 0)
			{
				return NotFound();
			}
			return activities.Select(a => a.ToDto()).ToList();
		}

		[HttpGet]
		[Route("/AllActivityDetails")]
		public async Task<ActionResult<IEnumerable<ActivityListDto>>> GetAllActivities(int id)
		{
			if (_context.Activities == null)
			{
				return NotFound();
			}
			var activities = await _context.Activities
				.Include(a => a.ActivityDetails)
				.ThenInclude(ad => ad.ActivityDiscounts)
				.ThenInclude(adc => adc.PromotionProductType)
				.Include(a => a.ActivityDetails)
				.ThenInclude(ad => ad.ActivityDiscounts)
				.ThenInclude(adc => adc.ProductTags)
				.Where(a => a.Id == id).ToListAsync();

			if (activities.Count == 0)
			{
				return NotFound();
			}

			return activities.Select(a => a.ToDto()).ToList();
		}

		[HttpGet]
		[Route("/OnActivities")]
		public async Task<ActionResult<IEnumerable<ActivityListDto>>> GetOnActivities()
		{
			var activities = await _context.Activities
                .Where(a => a.StartTime <= DateTime.Now.AddDays(7) && a.EndTime >= DateTime.Now)
                .Include(a => a.ActivityDetails)
				.ThenInclude(ad => ad.ActivityDiscounts)
				.ThenInclude(adc => adc.PromotionProductType)
				.Include(a => a.ActivityDetails)
				.ThenInclude(ad => ad.ActivityDiscounts)
				.ThenInclude(adc => adc.ProductTags)
				.ToListAsync();
			if (activities.Count == 0)
			{
				return NotFound();
			}

			return activities.Select(a => a.ToDto()).ToList();
		}

		[HttpGet]
		[Route("/OnActivityDetails")]
		public async Task<ActionResult<IEnumerable<ActivityListDto>>> GetOnActivities(int id)
		{
			if (_context.Activities == null)
			{
				return NotFound();
			}
			var activities = await _context.Activities
                .Where(a => a.StartTime <= DateTime.Now.AddDays(7) && a.EndTime >= DateTime.Now)
                .Include(a => a.ActivityDetails)
				.ThenInclude(ad => ad.ActivityDiscounts)
				.ThenInclude(adc => adc.PromotionProductType)
				.Include(a => a.ActivityDetails)
				.ThenInclude(ad => ad.ActivityDiscounts)
				.ThenInclude(adc => adc.ProductTags)
				.Where(a => a.Id == id).ToListAsync();

			if (activities.Count == 0)
			{
				return NotFound();
			}

			return activities.Select(a => a.ToDto()).ToList();
		}

		private bool ActivityExists(int id)
		{
			return (_context.Activities?.Any(e => e.Id == id)).GetValueOrDefault();
		}
	}
}
