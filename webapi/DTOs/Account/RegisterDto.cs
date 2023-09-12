using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;
using webapi.Models;

namespace webapi.DTOs.Account
{
    public class RegisterDto
    {
        public int Id { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Name { get; set; }
        public string? Nickname { get; set; }
        public string? Thumbnail { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public DateTime Birthday { get; set; }
        public string PhoneNumber { get; set; }
        public bool DMSubscribe { get; set; }

    }

    public static class RegisterExt
    {
        public static Member ToMemberEntity(this RegisterDto dto)
        {
            return new Member
            {
                Id = dto.Id,
                Account = dto.Account,
                Password = dto.Password,
                Name = dto.Name,
                Nickname = dto.Nickname,
                Thumbnail = dto.Thumbnail,
                Email = dto.Email,
                Gender = dto.Gender,
                Birthday = dto.Birthday,
                PhoneNumber = dto.PhoneNumber,
                DMSubscribe = dto.DMSubscribe,
            };
        }
    }
}
