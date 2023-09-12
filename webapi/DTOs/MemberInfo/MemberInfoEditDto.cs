using webapi.Models;

namespace webapi.DTOs.MemberInfo
{
	public class MemberInfoEditDto
	{
		public int Id { get; set; }
		//public string Thumbnail { get; set; }
		public string? Name { get; set; }
		public string? Nickname { get; set; }
		public string? Gender { get; set; }
		public DateTime Birthday { get; set; }
		public string? PhoneNumber { get; set; }
		public bool DMSubscribe { get; set; }
	}
	public static class MemberEditExt
	{
		public static Member ToEntity(this MemberInfoEditDto dto)
		{
			return new Member
			{
				Id = dto.Id,
				Name = dto.Name,
				Nickname = dto.Nickname,
				Gender = dto.Gender,
				PhoneNumber = dto.PhoneNumber,
				DMSubscribe = dto.DMSubscribe
			};
		}

		public static void UpdateEntity(this MemberInfoEditDto dto, Member member)
		{
			member.Name = dto.Name;
			member.Nickname = dto.Nickname;
			member.Gender = dto.Gender;
			member.PhoneNumber = dto.PhoneNumber;
			member.DMSubscribe = dto.DMSubscribe;
			member.LastModified = DateTime.UtcNow;
		}

		public static MemberInfoEditDto ToEditDto(this Member entity)
		{
			return new MemberInfoEditDto
			{
				Id = entity.Id,
				Name = entity.Name,
				Nickname = entity.Nickname,
				Birthday = entity.Birthday,
				Gender = entity.Gender,
				PhoneNumber = entity.PhoneNumber,
				DMSubscribe = entity.DMSubscribe
			};
		}
	}
}
