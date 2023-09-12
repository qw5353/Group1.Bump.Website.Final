using webapi.Models;

namespace webapi.DTOs.Account
{
    public class LoginDto
    {
        public string Account { get; set; }
        public string Password { get; set; }
		public bool stayLogin { get; set; }

	}

}
