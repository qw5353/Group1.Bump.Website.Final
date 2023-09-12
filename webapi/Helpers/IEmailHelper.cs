namespace webapi.Helpers
{
	public interface IEmailHelper
	{
		public void SendEmail(Email email);
		public void MemberConfirmEmail(string email, string url);
		public void ChangePasswordEmail(string name, string email, string changeUrl, string reSendurl);
	}
}
