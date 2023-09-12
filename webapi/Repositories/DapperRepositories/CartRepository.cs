using Dapper;
using Microsoft.Data.SqlClient;
using webapi.Context;
using webapi.Helpers;
using webapi.Models;

namespace webapi.Repositories.DapperRepositories
{
	public class CartRepository
	{
		private readonly BumpDapperContext _context;


		public CartRepository(BumpDapperContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<CartVM>> GetCartItems(int id)
		{
			using (var conn = _context.CreateConnection())
			{
				string sql = @"
SELECT p.Thumbnail as Img ,p.Name ,p.Price , d.Quantity, s.Style, 
p.ShelfStatus, s.Inventory, m.MemberLevelId　as MemberLevel, d.Id as CartDetailId , d.ProductStylesId, s.ProductId
FROM Carts as c
INNER JOIN CartDetails as d ON c.Id = d.CartsId
INNER JOIN ProductStyles as s ON s.Id = d.ProductStylesId
INNER JOIN Products as p ON p.Id = s.ProductId
INNER JOIN Members as m ON m.Id = c.MemberId
INNER JOIN MemberLevels as l ON l.Id = m.MemberLevelId
WHERE c.MemberId = @Id;";

				IEnumerable<CartVM> cart = await conn.QueryAsync<CartVM>(sql, new { Id = id });

				return cart;
			}

		}

		public async Task<MemberInfoVM> MemberInfo(int id)
		{
			using (var conn = _context.CreateConnection())
			{
				string sql = @"
SELECT Name , PhoneNumber as phone, Email
FROM Members
WHERE Id = @Id;";

				MemberInfoVM cart = await conn.QuerySingleAsync<MemberInfoVM>(sql, new { Id = id });

				return cart;
			}

		}

		public async Task<MemberInfoVM> MemberRedPoint(int id)
		{
			using (var conn = _context.CreateConnection())
			{
				string sql = @"
SELECT Points
FROM Members
WHERE Id = @Id;";

				MemberInfoVM cart = await conn.QuerySingleAsync<MemberInfoVM>(sql, new { Id = id });

				return cart;
			}

		}

		public async Task<bool> UpdateCartDetailQuantity(int id, int quantity)
		{

			using (var conn = _context.CreateConnection())
			{
				string sql = @"
UPDATE CartDetails 
SET Quantity = @Quantity
WHERE Id = @Id;";

				int affectedRows = await conn.ExecuteAsync(sql, new { Id = id, Quantity = quantity });

				// 更新成功
				return affectedRows == 0 ? false : true;
			}


		}

		public async Task<bool> DeleteCartItems(int id)
		{
			using (var conn = _context.CreateConnection())
			{
				string sql = @"
DELETE FROM CartDetails
WHERE Id = @Id;";

				int affectedRows = await conn.ExecuteAsync(sql, new { Id = id });

				// 更新成功
				return affectedRows == 0 ? false : true;
			}

		}

		public async Task<int> CreateOrder(CheckoutVM checkoutVM)
		{
			using (var conn = _context.CreateConnection())
			{
				string insertOrderSql = @"
INSERT INTO Orders(MemberId, RecipientName, RecipientPhone, RecipientAddress, RecipientEmail,
OrderStatusId, CreateAt, Note, TotalProductsPrice, DeliverPrice,
UsedPoint, DiscountPrice)
VALUES (@MemberId, @RecipientName, @RecipientPhone, @RecipientAddress, @RecipientEmail,
@OrderStatusId, GETDATE(), @Note, @TotalProductsPrice, @DeliverPrice, @UsedPoint, @DiscountPrice);

update CouponSendＭembers
set Usage = 1
where CouponId = @CouponId and MemberId=@MemberId;

UPDATE Members
SET Points = Points + @GetRedPoint
WHERE Id = @MemberId

UPDATE Members
SET Points = Points - @UsedPoint
WHERE Id = @MemberId

SELECT CAST(SCOPE_IDENTITY() as int);";

				string insertDetailSql = @"INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice, ProductName,
Subtotal, ProductStyleId)
VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice, @ProductName, @Subtotal, @ProductStyleId);

DELETE FROM CartDetails
WHERE Id = @CartDetailId;

UPDATE ProductStyles
SET [Inventory] = [Inventory] - @Quantity
WHERE Id = @ProductStyleId;
";
				conn.Open();

				using (var transaction = conn.BeginTransaction())
				{
					try
					{
						// 插入 Order 記錄
						var orderParams = new
						{
							checkoutVM.MemberId,
							checkoutVM.RecipientName,
							checkoutVM.RecipientPhone,
							checkoutVM.RecipientAddress,
							checkoutVM.RecipientEmail,
							checkoutVM.OrderStatusId,
							checkoutVM.Note,
							checkoutVM.TotalProductsPrice,
							checkoutVM.DeliverPrice,
							checkoutVM.UsedPoint,
							checkoutVM.DiscountPrice,
							checkoutVM.CouponId,
							checkoutVM.GetRedPoint
						};

						var orderId = await conn.ExecuteScalarAsync<int>(insertOrderSql, orderParams, transaction);

						// 插入 OrderDetails 記錄
						foreach (var item in checkoutVM.CheckoutItems)
						{
							var detailParams = new
							{
								OrderId = orderId,
								item.ProductId,
								item.Quantity,
								item.UnitPrice,
								item.ProductName,
								item.Subtotal,
								item.ProductStyleId,
								item.CartDetailId
							};

							await conn.ExecuteAsync(insertDetailSql, detailParams, transaction);
						}

						transaction.Commit();
						return orderId;
					}
					catch (Exception ex)
					{
						transaction.Rollback();
						Console.WriteLine(ex.Message);
						return 0;
					}
				}
			}
		}
	}

	public class CartVM
	{

		public int MemberId { get; set; }
		public string? Img { get; set; }
		public string? Name { get; set; }
		public int Price { get; set; }
		public int Quantity { get; set; }
		public string? Style { get; set; }
		public string? ShelfStatus { get; set; }
		public int Inventory { get; set; }
		public int MemberLevel { get; set; }
		public int CartDetailId { get; set; }
		public int ProductStylesId { get; set; }
        public int ProductId { get; set; }	
    }

	public class MemberInfoVM
	{
		public string? Name { get; set; }
		public string? Phone { get; set; }
		public string? Email { get; set; }
		public int Points { get; set; }
	}

	public class CheckoutVM
	{
    public int MemberId { get; set;}
    public string? RecipientName { get; set; }
    public string? RecipientEmail { get; set;}
    public string? RecipientPhone { get; set; }	
    public string? RecipientAddress { get; set; }
    public DateTime CreateAt { get; set; }
    public string? Note { get; set; }
    public int TotalProductsPrice { get; set; }
    public int DeliverPrice { get; set; }
    public int DiscountPrice { get; set; }
    public int UsedPoint { get; set; }
    public int OrderStatusId { get; set; }
    public string? PayType { get; set; }
    public int CouponId { get; set; }
    public int GetRedPoint { get; set; }
    public string? TradeType { get; set; }
    public IEnumerable<CheckoutItemsVM>? CheckoutItems { get; set; }
	}

	public class CheckoutItemsVM
	{
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public int UnitPrice { get; set; }
		public string? ProductName { get; set; }
		public int Subtotal { get; set; }
		public int ProductStyleId { get; set; }
    public int CartDetailId { get; set; }
    }
}
