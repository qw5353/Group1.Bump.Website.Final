using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using webapi.DTOs.MemberInfo;
using webapi.Infra;
using webapi.Models;

namespace webapi.Controllers
{
	[Route("[controller]")]
	[ApiController]
	[Authorize]
	public class MemberController : ControllerBase
	{
		private readonly BumpContext _context;
		private readonly HashUtility _hashUtility;

		public MemberController(BumpContext context, HashUtility hashUtility)
		{
			_context = context;
			_hashUtility = hashUtility;

		}

		//把所有原始密碼雜湊
		[HttpPost("HashingPwds")]
		public async Task<IActionResult> HashingPwds()
		{
			var membersInDb = _context.Members;
			foreach (var member in membersInDb)
			{
				_context.Entry(member).State = EntityState.Modified;
				member.Password = _hashUtility.ToBCrypt(member.Password);
			}
			await _context.SaveChangesAsync();
			return NoContent();
		}

		[HttpGet]
		public async Task<ActionResult<MemberIndexDto>> MemberInfo()
		{
			if (_context.Members == null) return NotFound();

			if (!User.Identity.IsAuthenticated) return Unauthorized();

			var currentMemberId = Int32.Parse(User.FindFirstValue("memberId"));
			var memberInDb = await _context.Members
				  .Include(l => l.MemberLevel)
				  .Where(m => m.Id == currentMemberId)
				  .FirstOrDefaultAsync();
			if (memberInDb == null) return NotFound();

			return memberInDb.ToDto();
		}

		[HttpGet("EditProfile")]
		public async Task<ActionResult<MemberInfoEditDto>> EditProfile()
		{
			var currentMemberId = Int32.Parse(User.FindFirstValue("memberId"));
			var profile = GetProfile(currentMemberId);

			return profile;
		}

		private MemberInfoEditDto GetProfile(int currentMemberId)
		{
			var memberInDb = _context.Members.FirstOrDefault(m => m.Id == currentMemberId);
			return memberInDb == null ? null : memberInDb.ToEditDto();
		}

		[HttpPost("EditProfile")]
		public async Task<IActionResult> EditProfile(MemberInfoEditDto dto)
		{
			var currentMemberId = Int32.Parse(User.FindFirstValue("memberId"));
			if (currentMemberId != dto.Id) return BadRequest("Id無法預期的錯誤");

			var memberInDb = _context.Members?.FirstOrDefault(e => e.Id == currentMemberId);
			if (memberInDb == null) return NoContent();

			dto.UpdateEntity(memberInDb);

			await _context.SaveChangesAsync();

			return Ok("修改成功!");
		}

		[HttpPost("EditPwd")]
		public async Task<ActionResult> EditPwd(EditPwdDto dto)
		{
			var currentMemberId = Int32.Parse(User.FindFirstValue("memberId"));
			Result result = ChangePwd(currentMemberId, dto);

			if (result.IsFail) return BadRequest(result.ErrorMessage);
			return Ok("密碼修改成功!");
		}

		private Result ChangePwd(int currentMemberId, EditPwdDto dto)
		{
			var hashPwd = _hashUtility.ToBCrypt(dto.OriPassword);
			var memberInDb = _context.Members.FirstOrDefault(m => m.Id == currentMemberId && m.Password == hashPwd);
			if (memberInDb == null) return Result.Fail("密碼錯誤");

			var newPwd = _hashUtility.ToBCrypt(dto.ConfirmPassword);
			memberInDb.Password = newPwd;
			_context.SaveChanges();

			return Result.Success();
		}

		//Todo (2) Edit Individual Avatar
		[HttpPost("UpdateAvatar")]
		public async Task<ActionResult<string>> UpdateAvatar([FromBody]string imgUri)
		{
			var currentMemberId = Int32.Parse(User.FindFirstValue("memberId"));
			var currentMemberImg = User.FindFirstValue("profileImg");
			var memberInDb = _context.Members.FirstOrDefault(m => m.Id == currentMemberId);
			memberInDb.Thumbnail = imgUri;
			//currentMemberImg = imgUri;

			_context.SaveChanges();

			return Ok(imgUri);
		}
	}
}
