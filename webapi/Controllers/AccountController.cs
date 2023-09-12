using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using webapi.DTOs.Account;
using webapi.Helpers;
using webapi.Infra;
using webapi.Models;
using System.Net;
using Microsoft.Extensions.Options;
using FluentValidation;
using webapi.DTOs.CustomerService;
using Humanizer;
using FluentValidation.Results;

namespace webapi.Controllers
{
	[Route("[controller]")]
	[AllowAnonymous]
	[ApiController]

	public class AccountController : ControllerBase
	{
		private readonly BumpContext _context;
		private readonly HashUtility _hashUtility;
		private readonly IEmailHelper _emailHelper;
		private readonly IConfiguration _config;
		private readonly IValidator<RegisterDto> _validator;

		public AccountController(BumpContext context, HashUtility hashUtility, IEmailHelper emailHelper, IConfiguration config, IValidator<RegisterDto> validator)
		{
			_context = context;
			_hashUtility = hashUtility;
			_emailHelper = emailHelper;
			_config = config;
			_validator = validator;
		}

		[HttpGet]
		public IActionResult IsLogin()
		{
			bool isAuthenticated = User?.Identity?.IsAuthenticated ?? false;
			return isAuthenticated ? Ok("You are Login") : BadRequest("Non User");
		}

		[HttpPost]
		[Route("Login")]
		public async Task<IActionResult> Login(LoginDto member)
		{
			LoginResult result = ValidLogin(member);
			if (result.IsFail) return BadRequest(result.ErrorMessage);

			var memberInDb = result.Member;

			var claim = new List<Claim>
			{
					new Claim("memberId", memberInDb.Id.ToString()),
					new Claim(ClaimTypes.Name, memberInDb.Name),
					new Claim("account", memberInDb.Account),
					new Claim(ClaimTypes.Email, memberInDb.Email),
					new Claim("profileImg", memberInDb.Thumbnail),
					new Claim("phoneNumber", memberInDb.PhoneNumber)
				};

			var claimsIdentity = new ClaimsIdentity(
				claim, CookieAuthenticationDefaults.AuthenticationScheme);

			var authProperties = new AuthenticationProperties
			{
				IsPersistent = member.stayLogin,
				ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
				//RedirectUri = "/"
			};

			await HttpContext.SignInAsync(
			  CookieAuthenticationDefaults.AuthenticationScheme,
			  new ClaimsPrincipal(claimsIdentity),
			  authProperties);

			return Ok("LogIn!");
		}

		[HttpDelete]
		[Route("Logout")]
		public async Task<IActionResult> Logout()
		{
			if (User?.Identity?.IsAuthenticated == true)
			{
				await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			}
			return Ok("LogOut.");
		}

		[HttpGet]
		[Route("GetMemberStatus")]
		[Authorize]
		public IActionResult GetMemberStatus()
		{
			if (User?.Identity?.IsAuthenticated == false) return Unauthorized();
			var memberInfo = new MemberStatusDto(User.Claims.ToList());
			return Ok(memberInfo);
		}

		[HttpPost]
		[Route("google")]
		public async Task<ActionResult> ProcessGoogleLogin()
		{
			//呼叫的同時將認證ID傳入
			var handler = new JwtSecurityTokenHandler();
			string? credential = Request.Form["credential"];
			string? token = Request.Form["g_csrf_token"];
			string? cookiesToken = Request.Cookies["g_csrf_token"];

			//GoogleJsonWebSignature.Payload? payload = VerifyGoogleToken(credential, token, cookiesToken).Result;
			//if (payload == null) return BadRequest("Google授權失敗");

			var jsonToken = handler.ReadToken(credential) as JwtSecurityToken;
			if (jsonToken == null) return BadRequest("Google登入失敗");

			//解析認證ID
			var email = jsonToken.Payload["email"].ToString();
			var name = jsonToken.Payload["name"].ToString();
			var pictureUrl = jsonToken.Payload["picture"].ToString();

			var atIndex = email.IndexOf("@");
			var account = email.Substring(0, atIndex);

			// 跟資料庫比對
			var memberInDb = _context.Members.FirstOrDefault(m => m.Email == email);

			// 沒資料就去 Google 註冊的頁面
			if (memberInDb == null) return RedirectToAction("OAuthRegister", new
			{
				Account = account,
				Email = email,
				Name = name,
				Thumbnail = pictureUrl
			});

			var claims = new List<Claim>
			{
				new Claim("memberId", memberInDb.Id.ToString()),
				new Claim("account", account),
				new Claim(ClaimTypes.Name, name),
				new Claim("profileImg", pictureUrl),
				new Claim(ClaimTypes.Email, email)
			};
			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var principal = new ClaimsPrincipal(identity);

			// 如果有此資料就按照一般登入處理
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

			// 使用 ASP.NET Core 身份驗證來登入用戶
			//var accessToken = await HttpContext.GetTokenAsync( GoogleDefaults.AuthenticationScheme, "access_token");

			return Redirect($"https://localhost:5002/VerifyGoogle/");
		}

		private async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(string? credential, string? token, string? cookiesToken)
		{
			if (credential == null || token == null && cookiesToken == null) return null;
			GoogleJsonWebSignature.Payload? payload;
			try
			{
				if (token != cookiesToken) return null;
				//IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
				string googleApiClientId = _config["Authentication:Google:ClientSecret"];
				var settings = new GoogleJsonWebSignature.ValidationSettings()
				{
					Audience = new List<string>() { googleApiClientId }
				};
				payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);
				//if (!payload.Issuer.Equals("account.google.com") && !payload.Issuer.Equals("https://account.google.com")) return null;
				//if (payload.ExpirationTimeSeconds==null) return null;

				DateTime now = DateTime.Now.ToUniversalTime();
				DateTime expiration = DateTimeOffset.FromUnixTimeSeconds((long)payload.ExpirationTimeSeconds).DateTime;
				if (now > expiration) return null;
			}
			catch
			{
				return null;
			}
			return payload;
		}

		[HttpPost]
		[Route("facebook")]
		public ActionResult ProcessFBLogin()
		{
			//前端將認證ID傳入
			//解析內容
			return Redirect("/register/OAuth");
		}

		[HttpPost]
		[Route("Register")]
		public async Task<ActionResult> Register(RegisterDto memberDto)
		{
			ValidationResult result = await _validator.ValidateAsync(memberDto);

			if (!result.IsValid) return BadRequest(Results.ValidationProblem(result.ToDictionary()));

			Result newMember = RegisterMember(memberDto);
			if (newMember.IsFail) return BadRequest(newMember.ErrorMessage);

			return Ok("註冊成功!");
		}

		[HttpPost]
		[Route("OAuthRegister")]
		public async Task<ActionResult> OAuthRegister([FromBody] OAuthRegisterDto newMember)
		{
			//var googleName = User.Claims.FirstOrDefault(c => c.Type == "urn:google:name")?.Value;
			//var googleThumbnail = User.Claims.FirstOrDefault(c => c.Type == "urn:google:picture")?.Value;
			//var googleEmail = User.Claims.FirstOrDefault(c => c.Type == "urn:google:email")?.Value;

			var googleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
			var googleThumbnail = User.Claims.FirstOrDefault(c => c.Type == "profileImg")?.Value;
			var googleEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
			var googleAccount = User.Claims.FirstOrDefault(c => c.Type == "account")?.Value; ;


			//前端收到的值 + 後端Claims 存入資料庫
			_context.Members.Add(new Member
			{
				Id = newMember.Id,
				Password = " ",

				Account = googleAccount,
				Name = googleName,
				Thumbnail = googleThumbnail,
				Email = googleEmail,

				Nickname = newMember.Nickname,
				Gender = newMember.Gender,
				Birthday = newMember.Birthday,
				PhoneNumber = newMember.PhoneNumber,
				DMSubscribe = newMember.DMSubscribe,
				IsConfirm = true
            });
			_context.SaveChanges();
			return Ok("註冊成功!");
		}

		[HttpPost]
		[Route("ActiveRegister")]
		[AllowAnonymous]
		public async Task<ActionResult> ActiveRegister([FromBody] VerifyMemberDto memberDto)
		{
			Result result = await MemberActive(memberDto.Id, memberDto.ConfirmCode);
			//if(result.IsFail) return Ok();
			return Ok("認證成功!");
		}
		private async Task<Result> MemberActive(int id, string confirmCode)
		{
			var memberInDb = await _context.Members.FirstOrDefaultAsync(m => m.Id == id && m.ConfirmCode == confirmCode);
			//防止奇怪的人來驗證
			if (memberInDb == null) return Result.Success();

			memberInDb.IsConfirm = true;
			memberInDb.ConfirmCode = null;
			await SendRegisterCoupons(id);

			//await _context.SaveChangesAsync();

			return Result.Success();
		}
		//忘記密碼 step1 先輸入email => 找有沒有
		//step 2 收信後按按鈕
		//step3 跳轉到輸入密碼畫面 後儲存密碼

		[HttpPost("newPassword")]
		public async Task<ActionResult> ForgetPassword([FromBody] string email)
		{
			var memberInDb = _context.Members.FirstOrDefault(m => m.Email == email);
			if (memberInDb == null) return BadRequest("查無會員");
			//if (!memberInDb.IsConfirm) return BadRequest("未驗證會員，請先驗證信箱後再重設密碼");

			var confirmCode = Guid.NewGuid().ToString("N");

			memberInDb.ConfirmCode = confirmCode;
			_context.SaveChanges();

			//找到後寄出驗證信
			//忘記密碼首頁 (only輸入email)
			var newPwdUrl = "https://localhost:5002/ForgetPwd";

			//輸入新的密碼頁面
			var changePwdUrl = $"https://localhost:5002/ChangePwd?memberId={memberInDb.Id}&confirmCode={confirmCode}";
			_emailHelper.ChangePasswordEmail(memberInDb.Name, memberInDb.Email, changePwdUrl, newPwdUrl);

			return Ok("請去收信更改密碼");
		}

		[HttpPost("editPassword")]
		public async Task<ActionResult> ForgertPassword(ForgetPasswordDto newPassword)
		{
			//先找到有沒有相同email確認會員是否存在
			var memberInDb = _context.Members.FirstOrDefault(m => m.Id == newPassword.Id && m.ConfirmCode == newPassword.ConfirmCode);
			if (memberInDb == null) return BadRequest("查無會員");
			if (!memberInDb.IsConfirm) return BadRequest("未驗證會員，請先驗證信箱後再重設密碼");

			//將變更密碼的值雜湊 後存入DB
			var hashPwd = _hashUtility.ToBCrypt(newPassword.ConfirmPassword);
			memberInDb.Password = hashPwd;
			memberInDb.ConfirmCode = null;

			_context.SaveChanges();

			return Ok("密碼變更成功!");
		}

		private LoginResult ValidLogin(LoginDto member)
		{
			var memberInDb = _context.Members.FirstOrDefault(m => m.Account == member.Account);

			if (memberInDb == null) return LoginResult.Fail("帳號或密碼有誤");
			//if (memberInDb.IsConfirm == false) return LoginResult.Fail("會員未認證");

			//Todo - 如果密碼輸入錯三次 鎖住帳號30分鐘
			var hashPwd = _hashUtility.ToBCrypt(member.Password);
			if (string.Compare(hashPwd, memberInDb.Password) != 0) return LoginResult.Fail("帳號或密碼有誤");

			return LoginResult.Success(memberInDb);
		}
		private Result RegisterMember(RegisterDto memberDto)
		{
			//確認現在帳號是否存在
			var memberInDb = _context.Members.FirstOrDefault(m => m.Account == memberDto.Account);

			if (memberInDb != null) return Result.Fail("帳號已存在");
			if (memberInDb == null && _context.Members.Any(m => m.Email == memberDto.Email)) return Result.Fail("信箱已存在");

			//幫密碼雜湊
			var encryPwd = _hashUtility.ToBCrypt(memberDto.Password);
			var confrimCode = Guid.NewGuid().ToString("N");

			memberDto.Password = encryPwd;

			var memberToAdd = memberDto.ToMemberEntity();
			memberToAdd.ConfirmCode = confrimCode;
			//存入資料庫
			_context.Members.Add(memberToAdd);
			_context.SaveChanges();

			//發送驗證信
			var url = $"https://localhost:5002/VerifyRegister?id={memberToAdd.Id}&confirmCode={confrimCode}";
			_emailHelper.MemberConfirmEmail(memberDto.Email, url);

			return Result.Success();

		}
		private async Task SendRegisterCoupons(int memberId)
		{
			var registerCouponId = 27;
			var sendingTime = DateTime.Now;
			var couponSendMember = new CouponSendＭember
			{
				CouponId = registerCouponId,
				MemberId = memberId,
				Usage = false,
				StartTime = sendingTime,
				EndTime = sendingTime.AddMonths(1).AddDays(-1),
				SendingTime = sendingTime
			};
			_context.CouponSendＭembers.Add(couponSendMember);
			await _context.SaveChangesAsync();
		}
	}
}
