using System.ComponentModel.DataAnnotations;

namespace webapi.DTOs.MemberInfo
{
	public class EditPwdDto
	{
        [Required]
        public string OriPassword { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
