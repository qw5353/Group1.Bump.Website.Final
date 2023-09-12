using System.ComponentModel.DataAnnotations;
using webapi.Models;

namespace webapi.DTOs.Account
{
	public class OAuthRegisterDto
	{
		public int Id { get; set; }

		public string Nickname { get; set; }

		[Required]
		public string Gender { get; set; }

		[Required]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
		public DateTime Birthday { get; set; }

		[Required]
		public string PhoneNumber { get; set; }

		public bool DMSubscribe { get; set; }
	}
	
}
