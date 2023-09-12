using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webapi.DTOs.Cart;
using webapi.Models;
using webapi.Repositories.DapperRepositories;
using webapi.Services;

namespace webapi.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly OrderRepositiory _orderRepository;
		private readonly BumpContext _context;

		public OrderController(OrderRepositiory orderRepository, BumpContext context)
		{
			_orderRepository = orderRepository;
			_context = context;
		}

		[HttpGet]
		[Route("/order")]
		public async Task<ActionResult<IEnumerable<OrdersVM>>> GetOrder(int id)
		{
			var orderItems = await _orderRepository.GetOrderItems(id);

			return Ok(orderItems);
		}


		[HttpGet]
		[Route("/orderDetail")]
		public async Task<ActionResult<SearchOrdersVM>> GetOrderDetail(int id)
		{
			var orderItems = await _orderRepository.GetOrderDetail(id);

			return Ok(orderItems);
		}

		[HttpPost]
		[Route("/orderComment")]
		public async Task<ActionResult> orderComment(List<OrderCommentVM> vm)
		{
			//await _cartRepository.UpdateCartDetailQuantity(id ,quantity);
			bool insertSuccess = await _orderRepository.InsertOrderComment(vm);

			if (insertSuccess)
			{
				return Ok();
			}
			else
			{
				return BadRequest("商品評價失敗。");
			}
		}
	}
}
