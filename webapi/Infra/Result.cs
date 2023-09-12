using webapi.Models;

namespace webapi.Infra
{
	public class Result
	{
		public bool IsSuccess { get; protected set; }
		public bool IsFail => !IsSuccess;
		public string? ErrorMessage { get; set; }

		public static Result Success() => new Result() { IsSuccess = true, ErrorMessage = null };

		public static Result Fail(string errMsg) => new Result() { IsSuccess = false, ErrorMessage = errMsg };


	}

	public class LoginResult : Result
	{
		public Member? Member { get; set; }
		public static LoginResult Success(Member member) => new LoginResult() { IsSuccess = true, ErrorMessage = null, Member = member };
		public static new LoginResult Fail(string errMsg) => new LoginResult() { IsSuccess = false, ErrorMessage = errMsg };

	}
}
