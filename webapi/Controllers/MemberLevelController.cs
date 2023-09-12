using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.DTOs.MemberLevelInfo;
using webapi.Models;

namespace webapi.Controllers
{
	[Route("[controller]")]
	[ApiController]
	[Authorize]
	public class MemberLevelController : ControllerBase
	{
		private readonly BumpContext _context;

		public MemberLevelController(BumpContext context)
		{
			_context = context;
		}

		[HttpGet("MemberCards")]
		public async Task<ActionResult<IEnumerable<MemberLevelCards>>> MemberLevelCards()
		{
			if (_context.MemberLevels == null) return NotFound();
			var levelCards = await _context.MemberLevels.ToListAsync();

			return levelCards.Select(c => c.ToLevelCardsDto()).ToList();
		}

		[HttpGet("TheMemberCard")]
		public async Task<ActionResult<MemberLevelCard>> TheMemberCard()
		{
			if (_context.MemberLevels == null) return NotFound();
			var currentMemberId = int.Parse(User.FindFirstValue("memberId"));

			var theCard = await _context.MemberLevelDetails
				  .Include(l => l.MemberLevel)
				  .Include(m => m.Member)
				  .Where(d => d.MemberId == currentMemberId)
				  .FirstOrDefaultAsync();

			if (theCard == null) return BadRequest("查無此人");

			return theCard.ToMemberLevelCardDto();
		}

		[HttpGet("TheNextCard")]
		public async Task<ActionResult<MemberLevelCards>> NextCard()
		{
			if (_context.MemberLevels == null) return NotFound();
			var currentMemberId = int.Parse(User.FindFirstValue("memberId"));
			var maxCardId = _context.MemberLevels.Max(m => m.LevelOrder);
			var currentCard = await _context.MemberLevelDetails
				  .Include(l => l.MemberLevel)
				  .Include(m => m.Member)
				  .Where(d => d.MemberId == currentMemberId)
				  .FirstOrDefaultAsync();

			int currentCardId = currentCard.MemberLevel.LevelOrder;
			int nextCardId = currentCardId + 1 > maxCardId ? maxCardId : currentCardId + 1;

			return _context.MemberLevels.Where(m => m.LevelOrder == nextCardId).Select(c => c.ToLevelCardsDto()).FirstOrDefault();
		}

		[HttpGet("CurrentOrderSum")]
		public async Task<ActionResult<decimal>> CurrentOrderSum()
		{
			var currentMemberId = int.Parse(User.FindFirstValue("memberId"));
			var memberOrder = await _context.Orders
				.Where(m => m.MemberId == currentMemberId)
				.ToListAsync();

			decimal theOrderSum = 0;
			foreach (var order in memberOrder)
			{
				theOrderSum += order.TotalProductsPrice;
			}
			return theOrderSum;
		}

		[HttpGet("CurrentOrderCount")]
		public async Task<ActionResult<int>> CurrentOrderCount()
		{
			var currentMemberId = int.Parse(User.FindFirstValue("memberId"));
			var memberOrder = await _context.Orders
				.Where(m => m.MemberId == currentMemberId)
				.ToListAsync();

			return memberOrder.Count();
		}
	}
}
