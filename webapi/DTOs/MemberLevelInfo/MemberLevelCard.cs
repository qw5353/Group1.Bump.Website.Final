using webapi.Models;

namespace webapi.DTOs.MemberLevelInfo
{
	public class MemberLevelCards
	{
		public int MemberCardId { get; set; }
		public int LevelOrder { get; set; }
		public string? LevelName { get; set; }
		public string? Name { get; set; }
		public int UpgradePrice { get; set; }
        public int UpgradeOrderCount { get; set; }
        public int TimePeriod { get; set; }
        public decimal GainPointRate { get; set; }
		public string? Description { get; set; }
    }

	public class MemberLevelCard
	{
		public int MemberId { get; set; }
		public int MemberCardId { get; set; }
		public int LevelOrder { get; set; }
		public string? LevelName { get; set; }
		public string? Name { get; set; }
		public int UpgradePrice { get; set; }
		//public int CurrentPriceSum { get; set; }
		public int UpgradeOrderCount { get; set; }
        //public int CurrentOrderCount { get; set; }
        public string? LeftTimePeriod { get; set; }
		public decimal GainPointRate { get; set; }
		public string? Description { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}

	public static class LevelCardExt
	{
		public static MemberLevelCards ToLevelCardsDto(this MemberLevel entity)
		{
			return new MemberLevelCards
			{
				MemberCardId = entity.Id,
				LevelOrder = entity.LevelOrder,
				LevelName = entity.LevelName,
				Name = entity.Name,
				UpgradePrice = entity.UpgradePrice,
				UpgradeOrderCount = entity.UpgradeOrderCount,
				TimePeriod = entity.TimePeriod,
				GainPointRate = entity.GainPointRate,
				Description = entity.Description,
			};
		}
		public static MemberLevelCard ToMemberLevelCardDto(this MemberLevelDetail entity)
		{
			TimeSpan LeftTime = entity.EndTime - DateTime.Now;
			string FormattedTime = $"{LeftTime.Days}.{LeftTime.Hours:D2}:{LeftTime.Minutes:D2}";
			return new MemberLevelCard
			{ 
				MemberId = entity.Member.Id,
				MemberCardId = entity.MemberLevel.Id,
				LevelOrder = entity.MemberLevel.LevelOrder,
				LevelName = entity.MemberLevel.LevelName,
				Name = entity.MemberLevel.Name,
				UpgradePrice = entity.MemberLevel.UpgradePrice,
				UpgradeOrderCount= entity.MemberLevel.UpgradeOrderCount,
				GainPointRate=entity.MemberLevel.GainPointRate,
				LeftTimePeriod = FormattedTime,
				Description = entity.MemberLevel.Description,
				StartDate = entity.StartTime, 
				EndDate = entity.EndTime
			};
		}
	}
}
