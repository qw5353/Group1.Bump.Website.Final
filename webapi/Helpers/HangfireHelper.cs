using Microsoft.EntityFrameworkCore;
using webapi.Models;

namespace webapi.Helpers
{
    public class HangfireHelper
    {
        private readonly BumpContext _context;
        public HangfireHelper(BumpContext context)
        {
            _context = context;
        }

        public async Task SendBirthdayCoupons(int couponId)
        {
            var coupon = await _context.Coupons
                .Include(c => c.TargetType)
                .Include(m => m.MemberLevels)
                .Include(m => m.MemberTags)
                .Where(c => c.Id == couponId).FirstAsync();
            var members = await _context.Members.ToListAsync();

            string targetTypeName = coupon.TargetType.Name;

            var memberSelect = new List<Member> { };
            switch (targetTypeName)
            {
                case "全部會員":
                    memberSelect = members;
                    break;
                case "會員等級":
                    var memberLevelIds = coupon.MemberLevels.Select(ml => ml.Id).ToList();
                    memberSelect = members.Where(m => memberLevelIds.Contains(m.MemberLevelId)).ToList();
                    break;
                case "會員標籤":
                    var memberTagIds = coupon.MemberTags.Select(mt => mt.Id).ToList();
                    memberSelect = members
                      .Where(m => m.MemberTags.Any(mt => memberTagIds.Contains(mt.Id))).ToList();
                    break;
            }

            var currentTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var couponSendMemberList = new List<CouponSendＭember> { };
            foreach (var member in memberSelect)
            {
                var startTime = new DateTime(DateTime.Now.Year, member.Birthday.Month, DateTime.Now.Day);
                if (currentTime == startTime)
                {
                    var couponSendMember = new CouponSendＭember
                    {
                        CouponId = couponId,
                        MemberId = member.Id,
                        Usage = false,
                        StartTime = startTime,
                        EndTime = startTime.AddMonths(1).AddDays(-1),
                        SendingTime = DateTime.Now
                    };

                    _context.CouponSendＭembers.Add(couponSendMember);
                    couponSendMemberList.Add(couponSendMember);
                }
            }

            if (couponSendMemberList.Count() == 0) return;
            _context.SaveChanges();
        }

        public async Task SendActivityCoupons(int couponId)
        {
            var coupon = await _context.Coupons
                .Include(c => c.TargetType)
                .Include(c => c.EventType)
                .Include(m => m.MemberLevels)
                .Include(m => m.MemberTags)
                .Where(c => c.Id == couponId).FirstAsync();
            var members = await _context.Members.ToListAsync();

            string targetTypeName = coupon.TargetType.Name;
            string eventTypeName = coupon.EventType.Name;
            string repeatRule = coupon.RepeatRule;
            byte? repeatNum = coupon.RepeatNum;
            DateTime? startTime = coupon.StartTime;
            DateTime? endTime = coupon.EndTime;

            List<int> memberIds = new List<int> { };
            List<DateTime> sendingStartTime = new List<DateTime> { };
            List<DateTime> sendingEndTime = new List<DateTime> { };

            if (eventTypeName != "活動") return;

            DateTime currentDate = (DateTime)startTime;
            switch (repeatRule)
            {
                case "每日":
                    while (currentDate <= endTime)
                    {
                        sendingStartTime.Add(currentDate);
                        sendingEndTime.Add(currentDate.AddDays(1));

                        currentDate = currentDate.AddDays(1);
                    }
                    break;
                case "每週":
                    while (currentDate <= endTime)
                    {
                        // 找到下個週一的日期
                        DateTime nextMonday = GetNextWeekday(currentDate, DayOfWeek.Monday);

                        // 加入當前日期的時間到 sendStartTime 列表中
                        sendingStartTime.Add(nextMonday);
                        sendingEndTime.Add(nextMonday.AddDays(7));

                        // 移到下一週的第一天（下週一）
                        currentDate = nextMonday.AddDays(7);
                    }
                    break;
                case "每月":
                    while (currentDate <= endTime)
                    {
                        sendingStartTime.Add(currentDate);
                        sendingEndTime.Add(currentDate.AddMonths(1).AddDays(-1));

                        currentDate = currentDate.AddMonths(1);
                        currentDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                    }
                    break;
                default:
                    sendingStartTime.Add((DateTime)startTime);
                    sendingEndTime.Add((DateTime)endTime);
                    break;
            }

            var currentTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var timeIndex = sendingStartTime.IndexOf(currentTime);
            if (timeIndex == -1) return;

            switch (targetTypeName)
            {
                case "全部會員":
                    memberIds = members.Select(m => m.Id).ToList();
                    break;
                case "會員等級":
                    var memberLevelIds = coupon.MemberLevels.Select(ml => ml.Id).ToList();
                    memberIds = members.Where(m => memberLevelIds.Contains(m.MemberLevelId)).Select(m => m.Id).ToList();
                    break;
                case "會員標籤":
                    var memberTagIds = coupon.MemberTags.Select(mt => mt.Id).ToList();
                    memberIds = members
                      .Where(m => m.MemberTags.Any(mt => memberTagIds.Contains(mt.Id)))
                      .Select(m => m.Id)
                      .ToList();
                    break;
            }

            foreach (var memberId in memberIds)
            {
                if (currentTime == sendingStartTime[timeIndex])
                {
                    var couponSendMember = new CouponSendＭember
                    {
                        CouponId = couponId,
                        MemberId = memberId,
                        Usage = false,
                        StartTime = sendingStartTime[timeIndex],
                        EndTime = sendingEndTime[timeIndex],
                        SendingTime = sendingStartTime[timeIndex] < DateTime.Now ? sendingStartTime[timeIndex] : DateTime.Now
                    };

                    _context.CouponSendＭembers.Add(couponSendMember);
                }
            }
            await _context.SaveChangesAsync();
        }

        private DateTime GetNextWeekday(DateTime startDate, DayOfWeek dayOfWeek)
        {
            int daysUntilNextDay = ((int)dayOfWeek - (int)startDate.DayOfWeek + 7) % 7;
            return startDate.AddDays(daysUntilNextDay);
        }
    }
}
