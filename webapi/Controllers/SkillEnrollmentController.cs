using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.DTOs.FieldService;
using webapi.DTOs.Payment.ECPay;
using webapi.Models;
using webapi.Services;

namespace webapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SkillEnrollmentController : ControllerBase
    {
        private readonly BumpContext _context;
        private readonly PayService _payService;
        public SkillEnrollmentController(BumpContext context, PayService payService)
        {
            _context = context;
            _payService = payService;
        }

        [HttpPost]
        public async Task<IActionResult> PostSkillEnrollment(SkillEnrollmentDto dto)
        {
            if (_context.Skillcurriculums == null)
            {
                return Problem("Entity set 'BumpContext.Skillcurriculums'  is null.");
            }
            dto.CreatedAt = DateTime.Now;
            var skillEnrollment = dto.ToEntity();
            var entity = _context.SkillEnrollments.Add(skillEnrollment);
            await _context.SaveChangesAsync();

            // checkout 
            switch (dto.PayType)
            {
                case "Line Pay":
                    var url = await LinePayCheckout(dto, entity.Entity.Id);
                    return Ok(url);
                case "ECPay":
                    SortedDictionary<string, string?> form = await PrepareECPayForm(dto, entity.Entity.Id);
                    return Ok(form);
                default:
                    return BadRequest();
            }
        }

        private async Task<SortedDictionary<string, string?>> PrepareECPayForm(SkillEnrollmentDto dto, int enrollId)
        {
            var entity = _context.Payments.Add(new Payment()
            {
                Amount = dto.Amount,
                PayType = "綠界支付",
                Status = "未付款",
                BumpSkillCourseEnrollmentId = enrollId
            });


            int affectedRows = await _context.SaveChangesAsync();

            if (affectedRows == 0) throw new Exception("無法結帳");

            var merchantTradeNo = entity.Entity.Id.ToString().PadLeft(8, '0') + DateTimeOffset.Now.ToUnixTimeSeconds().ToString().PadLeft(12, '0');
            entity.State = EntityState.Modified;
            entity.CurrentValues.SetValues(new { ECPayMerchantTradeNo = merchantTradeNo });
            await _context.SaveChangesAsync();

            var form = _payService.PrepareForm(new ECPayCheckoutDTO
            {
                MerchantTradeNo = merchantTradeNo,
                Amount = dto.Amount,
                Name = "Bump 技巧課程"
            });


            return form;
        }

        private async Task<string> LinePayCheckout(SkillEnrollmentDto dto, int enrollId)
        {
            string lineOrderId = Guid.NewGuid().ToString();

            var entity = _context.Payments.Add(new Payment()
            {
                Amount = dto.Amount,
                LineOrderId = lineOrderId,
                PayType = "Line Pay",
                Status = "未付款",
                BumpSkillCourseEnrollmentId = enrollId,
            });

            int affectedRows = await _context.SaveChangesAsync();

            if (affectedRows == 0) throw new Exception("無法結帳");

            var linepayCheckoutUrl = await _payService.LinePayRequest(new DTOs.Payment.LinePay.LinePayRequestDto
            {
                Amount = dto.Amount,
                OrderId = lineOrderId,
                ConfirmUrl = "https://localhost:5002/api/confirm/linepay",
                ProductName = "Bump 技巧課程"
            });

            return linepayCheckoutUrl;
        }

        [HttpGet]
        public async Task<ActionResult<GetSkillEnrollmentDto>> GetSkillEnrollment(int id)
        {
            if (_context.Skillcurriculums == null)
            {
                return Problem("Entity set 'BumpContext.Skillcurriculums'  is null.");
            }
            var skillEnrollments=await _context.SkillEnrollments
                .Include(ss=>ss.Skillcurriculums)
                .Include(sem => sem.Payments)
                .Where(s=>s.MemberId==id)
                .Select(dto => dto.ToDto())
                .ToListAsync();
            if (skillEnrollments == null)
            {
                return NotFound();
            }
            return Ok(skillEnrollments);
        }
    }
}
