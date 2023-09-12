using webapi.Models;

namespace webapi.DTOs.MemberInfo
{
	public class MemberIndexDto
	{
		public int Id { get; set; }
		public string Account { get; set; }
		public string MemberLevelName { get; set; }
		public string Thumbnail { get; set; }
		public string Name { get; set; }
		public string Nickname { get; set; }
		public string Email { get;  set; }
		public string Gender { get; set; }
		public DateTime Birthday { get; set; }
		public string PhoneNumber { get; set; }
		public int Points { get; set; }
		public bool DMSubscribe { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime LastModified { get; set; }
		public bool IsConfirm { get; set; }
		public bool IsBanned { get; set; }
	}

	public static class MemberIndexExt
	{
		public static MemberIndexDto ToDto(this Member entity)
		{
			return new MemberIndexDto
			{
				Id = entity.Id,
				Account = entity.Account,
				MemberLevelName = entity.MemberLevel.Name,
				Thumbnail = entity.Thumbnail,
				Name = entity.Name,
				Nickname = entity.Nickname,
				Email = entity.Email,
				Gender = entity.Gender,
				Birthday = entity.Birthday,
				PhoneNumber = entity.PhoneNumber,
				Points = entity.Points,
				DMSubscribe = entity.DMSubscribe,
				CreatedAt = entity.CreatedAt,
				LastModified = entity.LastModified,
				IsConfirm = entity.IsConfirm,
				IsBanned = entity.IsBanned,
			};
		}
	}
}
