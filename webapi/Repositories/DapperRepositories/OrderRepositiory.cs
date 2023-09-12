using webapi.Context;
using Dapper;
using Microsoft.Data.SqlClient;
using webapi.Helpers;
using webapi.Models;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;


namespace webapi.Repositories.DapperRepositories
{
	public class OrderRepositiory
	{
		private readonly BumpDapperContext _context;

		public OrderRepositiory(BumpDapperContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<OrdersVM>> GetOrderItems(int id)
		{
			using (var conn = _context.CreateConnection())
			{
				string sql = @"
SELECT o.[Id] as OrderId, m.[Name] as MemberName, o.TotalProductsPrice, o.CreateAt , s.[Name] as OrderStatusName, 
pa.PayType , pa.Status, d.ProductName, d.Quantity, d.UnitPrice, d.SubTotal 
FROM Orders as o
INNER JOIN OrderDetails as d on d.OrderId = o.Id
INNER JOIN Members as m on m.Id = o.MemberId
INNER JOIN OrderStatuses as s on s.Id = o.OrderStatusId
INNER JOIN Products as p on p.id = d.ProductId 
LEFT JOIN Payments as pa on pa.BumpOrderId = o.Id
WHERE o.MemberId = @Id
ORDER BY o.Id DESC;";

				IEnumerable<OrdersVM> orderData = await conn.QueryAsync<OrdersVM, OrderDetailVM, OrdersVM>(
					sql,
					(order, detail) =>
					{
						if (order.Details is null)
						{
							order.Details = new List<OrderDetailVM>();
						}
						order.Details.Add(detail);
						return order;
					},
					new { Id = id },
					splitOn: "ProductName"	
				);

				var result = orderData
					.GroupBy(o => o.OrderId)
					.Select(g =>
					{
						var groupedOrder = g.First();
						groupedOrder.Details = g.Select(o => o.Details.Single()).ToList();
						return groupedOrder;
					})
					.ToList();

				return result;
			}

		}

		public async Task<SearchOrdersVM> GetOrderDetail(int id)
		{
			using (var conn = _context.CreateConnection())
			{
				string sql = @"
SELECT o.RecipientName,o.RecipientPhone,o.RecipientAddress,o.RecipientEmail,o.CreateAt, m.Name as MemberName,
o.OrderStatusId, o.TotalProductsPrice,o.DeliverPrice, o.DiscountPrice, o.UsedPoint,
o.Note,p.Thumbnail, d.ProductName,d.Quantity,d.Subtotal,s.Style,d.ProductId,d.OrderId
FROM Orders as o
INNER JOIN OrderDetails as d ON d.OrderId = o.Id
INNER JOIN Products as p ON p.Id = d.ProductId
INNER JOIN ProductStyles as s ON s.Id = d.ProductStyleId
INNER JOIN Members as m ON m.Id = o.MemberId
WHERE o.Id = @Id;;";

				IEnumerable<SearchOrdersVM> orderData = await conn.QueryAsync<SearchOrdersVM, OrderDetailVM, SearchOrdersVM>(
					sql,
					(order, detail) =>
					{
						if (order.Details is null)
						{
							order.Details = new List<OrderDetailVM>();
						}
						order.Details.Add(detail);
						return order;
					},
					new { Id = id },
					splitOn: "Thumbnail"
				);

				var result = orderData
					.GroupBy(o => o.MemberName)
					.Select(g =>
					{
						var groupedOrder = g.First();
						groupedOrder.Details = g.Select(o => o.Details.Single()).ToList();
						return groupedOrder;
					})
					.Single();

				return result;
			}

		}

		public async Task<bool> InsertOrderComment(List<OrderCommentVM> vm)
		{

			using (var conn = _context.CreateConnection())
			{
				string sql = @"
INSERT INTO ProductComments (OrderId,ProductId,Description,Rating,CreateAt)
VALUES (@OrderId,@ProductId,@UserComment,@Rating,getDate())
;";

				int totalAffectedRows = 0;

				foreach (var item in vm)
				{
					var orderParams = new
					{
						item.OrderId,
						item.ProductId,
						item.UserComment,
						item.Rating
					};
					int affectedRows = await conn.ExecuteAsync(sql, orderParams);
					totalAffectedRows += affectedRows;
				}
				

				// 更新成功
				return totalAffectedRows > 0 ? true :false ;
			}
		}
	}

	public class OrdersVM
	{
		[Display(Name = "訂單編號")]
		public int OrderId { get; set; }


		[Display(Name = "會員名稱")]
		public string? MemberName { get; set; }

		[Display(Name = "產品資訊")]
		public List<OrderDetailVM>? Details { get; set; }


		[Display(Name = "總金額")]
		public int TotalProductsPrice { get; set; }


		[Display(Name = "訂單成立時間")]
		public DateTime CreateAt { get; set; }


		[Display(Name = "訂單狀態")]
		public string? OrderStatusName { get; set; }

        public string? PayType { get; set; }

		public string? Status { get; set; }	

    }

	public class OrderDetailVM
	{
        public string? Thumbnail { get; set; }

        public int OrderId { get; set; }


		[Display(Name = "產品名稱")]
		public string? ProductName { get; set; }

		[Display(Name = "數量")]
		public int Quantity { get; set; }

		[Display(Name = "單價")]
		public int UnitPrice { get; set; }

        public string? Style { get; set; }

        public int Subtotal { get; set; }
        public int ProductId { get; set; }
    }

	public class SearchOrdersVM
	{
        public string? RecipientEmail { get; set; }

		[Display(Name = "收件人名稱")]
		public string? RecipientName { get; set; }

		[Display(Name = "收件人電話")]
		public string? RecipientPhone { get; set; }

		[Display(Name = "收件人地址")]
		public string? RecipientAddress { get; set; }

		[Display(Name = "訂購進度")]
		public int OrderStatusId { get; set; }

		[Display(Name = "訂單成立時間")]
		public DateTime CreateAt { get; set; }
	
		public string CreateAtString => CreateAt.ToString();


		[Display(Name = "備註")]
		public string? Note { get; set; }

		[Display(Name = "總金額")]
		public int TotalProductsPrice { get; set; }

		[Display(Name = "運費")]
		public int DeliverPrice { get; set; }

		[Display(Name = "折價券")]
		public int DiscountPrice { get; set; }

		[Display(Name = "紅利點數")]
		public int UsedPoint { get; set; }
		
		[Display(Name = "會員名稱")]
		public string? MemberName { get; set; }
		public List<OrderDetailVM>? Details { get; set; }

    }

	public class OrderCommentVM
	{
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string? UserComment { get; set; }
        public int Rating { get; set; }

    }
}
