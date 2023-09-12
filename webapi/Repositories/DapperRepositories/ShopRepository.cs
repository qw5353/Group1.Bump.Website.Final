using Dapper;
using MailKit.Search;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Linq;
using webapi.Context;
using webapi.DTOs.Shop;
using webapi.Models;

namespace webapi.Repositories.DapperRepositories
{
	public class ShopRepository
	{
		private readonly BumpDapperContext _context;
		public ShopRepository(BumpDapperContext context)
		{
			_context = context;
		}
		//新商品
		public async Task<IEnumerable<ProductDto>> GetNewProductGroup()
		{
			using (var conn = _context.CreateConnection())
			{
				string sql = @"
SELECT TOP 12 P.Id,P.Name,B.Name as BrandName,P.Price,P.Thumbnail 
FROM Products P
JOIN Brands B ON B.Id = P.BrandId
WHERE P.ShelfStatus = '上架中'
ORDER BY CreateAt Desc";

				IEnumerable<ProductDto> productGroup = await conn.QueryAsync<ProductDto>(sql);

				return productGroup;
			}

		}
		//熱銷中(愛不釋手)
		public async Task<IEnumerable<ProductDto>> GetSellsWellProductGroup()
		{
			using (var conn = _context.CreateConnection())
			{
				string sql = @"
SELECT TOP 12 P.Id,P.Name,B.Name as BrandName,P.Price,P.Thumbnail,SUM(Quantity) 
FROM OrderDetails OD
JOIN Products P ON P.Id = OD.ProductId
JOIN Brands B ON B.Id = P.BrandId
JOIN Orders O ON O.Id = OD.OrderId
WHERE O.OrderStatusId = 8
AND P.ShelfStatus = '上架中'
GROUP BY P.Id,P.Name,B.Name,P.Price,P.Thumbnail
ORDER BY 6 DESC ";



				IEnumerable<ProductDto> productGroup = await conn.QueryAsync<ProductDto>(sql);

				return productGroup;
			}

		}

		public async Task<IEnumerable<BrandsDto>> GetBrands()
		{
			using (var conn = _context.CreateConnection())
			{
				string sql = @"
SELECT B.Name AS BrandName
FROM Brands B;";

				IEnumerable<BrandsDto> brands = await conn.QueryAsync<BrandsDto>(sql);

				return brands;
			}

		}

		public async Task<BrandsDto> GetBrandDesc(string? name)
		{
			using (var conn = _context.CreateConnection())
			{
				string sql = @"
SELECT B.Description AS BrandDesc
FROM Brands B
WHERE B.Name = @Name;";

				BrandsDto brands = await conn.QuerySingleAsync<BrandsDto>(sql, new {Name = name});

				return brands;
			}

		}

		public async Task<(int,int)> GetProductListPriceRange(ProductFilterDto productFilterDto)
		{
			var keyword = productFilterDto.Keyword;

			var parameters = new DynamicParameters();
			(string whereConditions, DynamicParameters parametersResult) whereStatement = GetProductFilterWhereStatment(parameters, productFilterDto, keyword);


			string sql = @"
SELECT MIN(P.Price),MAX(P.Price) 
FROM Products P
JOIN Brands B on B.Id = P.BrandId 
JOIN ThirdCategories T on T.Id =  P.ThirdCategoryId
JOIN SecondCategories S on S.Id = T.SecondCategoryId
JOIN FirstCategories F on F.Id = S.FirstCategoryId " +
'\n' + whereStatement.whereConditions +
';';


			using (var conn = _context.CreateConnection())
			{
				return await conn.QuerySingleAsync<(int,int)>(sql,
				whereStatement.parametersResult);

			}

		}
		public async Task<IEnumerable<string>> GetProductListBrands(ProductFilterDto productFilterDto)
		{
			var keyword = productFilterDto.Keyword;

			var parameters = new DynamicParameters();
			(string whereConditions, DynamicParameters parametersResult) whereStatement = GetProductFilterWhereStatment(parameters, productFilterDto, keyword);


			string sql = @"
SELECT DISTINCT B.Name 
FROM Products P
JOIN Brands B on B.Id = P.BrandId 
JOIN ThirdCategories T on T.Id =  P.ThirdCategoryId
JOIN SecondCategories S on S.Id = T.SecondCategoryId
JOIN FirstCategories F on F.Id = S.FirstCategoryId " +
'\n' + whereStatement.whereConditions +
';';


			using (var conn = _context.CreateConnection())
			{
				return await conn.QueryAsync<string>(sql,
				whereStatement.parametersResult);

			}

		}

		public async Task<IEnumerable<ProductCategoryDto>> GetSecondCategories(string? firstCategory)
		{

			string sql = @"
SELECT DISTINCT S.Name AS SecondCategory
FROM SecondCategories S
JOIN ThirdCategories T on T.SecondCategoryId = S.Id
JOIN FirstCategories F ON F.Id = S.FirstCategoryId
JOIN Products P ON P.ThirdCategoryId = T.Id
WHERE F.Name = @firstCategory
AND P.ShelfStatus = '上架中' ;";

			using (var conn = _context.CreateConnection())
			{

				var secondCategories = await conn.QueryAsync<ProductCategoryDto>(sql, new { firstCategory = firstCategory });

				return secondCategories;
			}

		}

		public async Task<IEnumerable<ProductCategoryDto>> GetThirdCategories(string? firstCategory, string? secoondCategory)
		{
			string sql = @"
SELECT DISTINCT T.Name AS ThirdCategory
FROM ThirdCategories T
JOIN SecondCategories S ON S.Id = T.SecondCategoryId
JOIN FirstCategories F ON F.Id = S.FirstCategoryId
JOIN Products P ON P.ThirdCategoryId = T.Id
WHERE F.Name = @firstCategory
AND S.Name = @secondCategory
AND P.ShelfStatus = '上架中' ;";

			using (var conn = _context.CreateConnection())
			{
				var thirdCategories = await conn.QueryAsync<ProductCategoryDto>(sql, new { secondCategory = secoondCategory, firstCategory = firstCategory });

				return thirdCategories;
			}

		}

		public async Task<int> GetProductListCount(ProductFilterDto productFilterDto)
		{
			var keyword = productFilterDto.Keyword;

			var parameters = new DynamicParameters();
			(string whereConditions, DynamicParameters parametersResult) whereStatement = GetProductFilterWhereStatment(parameters, productFilterDto, keyword);


			string sql = @"
SELECT COUNT(*)
FROM Products P
JOIN Brands B on B.Id = P.BrandId 
JOIN ThirdCategories T on T.Id =  P.ThirdCategoryId
JOIN SecondCategories S on S.Id = T.SecondCategoryId
JOIN FirstCategories F on F.Id = S.FirstCategoryId " +
'\n' + whereStatement.whereConditions +
';';


			using (var conn = _context.CreateConnection())
			{
				return await conn.QuerySingleAsync<int>(sql,
				whereStatement.parametersResult);
			}

		}
		public async Task<IEnumerable<ProductDto>> GetProductList(ProductFilterDto productFilterDto)
		{
			var page = productFilterDto.Page;
			var pageSize = productFilterDto.PageSize;
			var keyword = productFilterDto.Keyword;

			var parameters = new DynamicParameters(new
			{
				OffsetRows = (page - 1) * pageSize,
				FetchRows = pageSize
			});
			(string whereConditions, DynamicParameters parametersResult) whereStatement = GetProductFilterWhereStatment(parameters, productFilterDto, keyword);

			string orderStatement = "";
			switch (productFilterDto.OrderKey)
			{
				case 1:
					orderStatement = " ORDER BY P.CreateAt DESC ";
					break;
				case 2:
					orderStatement = " ORDER BY P.CreateAt ";
					break;
				case 3:
					orderStatement = " ORDER BY P.Price ";
					break;
				case 4:
					orderStatement = " ORDER BY P.Price DESC ";
					break;
				default:
					orderStatement = " ORDER BY P.Id ";
					break;
			}

			string sql = @"
SELECT P.Id,P.Name,P.Price,P.Thumbnail,
B.Name as BrandName
FROM Products P
JOIN Brands B on B.Id = P.BrandId 
JOIN ThirdCategories T on T.Id =  P.ThirdCategoryId
JOIN SecondCategories S on S.Id = T.SecondCategoryId
JOIN FirstCategories F on F.Id = S.FirstCategoryId " +
'\n' + whereStatement.whereConditions +
'\n' + orderStatement +
@" OFFSET @OffsetRows ROWS
FETCH NEXT @FetchRows ROWS ONLY; ";

			using (var conn = _context.CreateConnection())
			{
				return await conn.QueryAsync<ProductDto>(sql,
				whereStatement.parametersResult);

			}
		}
		private (string, DynamicParameters) GetProductFilterWhereStatment(DynamicParameters parameters, ProductFilterDto productFilterDto, string? keyword)
		{
			//Filter
			string whereConditions = "WHERE P.ShelfStatus = '上架中' \n";
			if (productFilterDto.BrandName != null && productFilterDto.BrandName.Count > 0)
			{
				string where = "";
				int i = 0;
				foreach (string BrandName in productFilterDto.BrandName)
				{
					i++;
					where = where + $",@BrandName{i}";
					parameters.Add($"BrandName{i}", $"{BrandName}");
				}
				whereConditions = whereConditions + $" AND B.Name IN (" + where.Substring(1) + ") \n";
			}

			if (productFilterDto.ProductStyle != null && productFilterDto.ProductStyle.Count > 0)
			{
				string where = "";
				int i = 0;
				foreach (string ProductStyle in productFilterDto.ProductStyle)
				{
					i++;
					where = where + $",@ProductStyle{i}";
					parameters.Add($"ProductStyle{i}", $"{ProductStyle}");
				}
				whereConditions = whereConditions +
					@" AND EXISTS (SELECT * FROM 
WHERE ProductId = P.Id 
AND Style IN (" + where.Substring(1) + ")";

			}

			if (productFilterDto.MinPrice.HasValue)
			{
				whereConditions = whereConditions + $" AND P.Price >= @MinPrice \n";
				parameters.Add("MinPrice", productFilterDto.MinPrice);
			}

			if (productFilterDto.MaxPrice.HasValue)
			{
				whereConditions = whereConditions + $" AND P.Price <= @MaxPrice \n";
				parameters.Add("MaxPrice", productFilterDto.MaxPrice);
			}

			//Search
			if (!string.IsNullOrEmpty(keyword))
			{
				whereConditions = whereConditions + $"AND ( B.Name like @BrandName \n";
				parameters.Add("BrandName", $"%{keyword}%");

				whereConditions = whereConditions + $" OR P.Name like @Name \n";
				parameters.Add("Name", $"%{keyword}%");

				whereConditions = whereConditions + $" OR P.Code like @Code ) \n";
				parameters.Add("Code", $"%{keyword}%");

				return (whereConditions, parameters);
			}

			//Navbar Categories || Filter Categories
			if (!productFilterDto.FirstCategory.IsNullOrEmpty())
			{
				whereConditions = whereConditions + $" AND F.Name = @FirstCategory \n";
				parameters.Add("FirstCategory", $"{productFilterDto.FirstCategory}");
			}

			if (productFilterDto.SecondCategories != null && productFilterDto.SecondCategories.Count > 0)
			{
				string where = "";
				int i = 0;
				foreach (string SecondCategories in productFilterDto.SecondCategories)
				{
					i++;
					where = where + $",@SecondCategories{i}";
					parameters.Add($"SecondCategories{i}", $"{SecondCategories}");
				}
				whereConditions = whereConditions + $" AND S.Name IN (" + where.Substring(1) + ") \n";
			}

			if (productFilterDto.ThirdCategories != null && productFilterDto.ThirdCategories.Count > 0)
			{
				string where = "";
				int i = 0;
				foreach (string ThirdCategories in productFilterDto.ThirdCategories)
				{
					i++;
					where = where + $",@ThirdCategories{i}";
					parameters.Add($"ThirdCategories{i}", $"{ThirdCategories}");
				}
				whereConditions = whereConditions + $" AND T.Name IN (" + where.Substring(1) + ") \n";
			}

			return (whereConditions, parameters);
		}

		public async Task<IEnumerable<ProductCategoriesDto>> GetProductCategories(string? firstCategory)
		{
			using (var conn = _context.CreateConnection())
			{
				string sql = @"
SELECT  S.Name AS SecondCategories,T.Name AS ThirdCategories
FROM ThirdCategories T
JOIN SecondCategories S ON S.Id = T.SecondCategoryId
JOIN FirstCategories F ON F.Id = S.FirstCategoryId
WHERE F.Name = @FirstCategory
GROUP BY S.Name ,T.Name;";

				//IEnumerable<ProductCategoriesDto> productCategories = new List<ProductCategoriesDto>();
				var result = await conn.QueryAsync(sql, new { FirstCategory = firstCategory });

				var productCategories = result
					.GroupBy(r => r.SecondCategories)
					.Select(g => new ProductCategoriesDto
					{
						SecondCategories = g.Key,
						ThirdCategories = g.Select(s => (string)s.ThirdCategories).ToList()
					});

				return productCategories;
			}

		}
		public async Task<ProductDetailsDto> GetProductDetails(int? id)
		{
			using (var conn = _context.CreateConnection())
			{
				string sql = @"
SELECT P.Id,B.Id AS BrandId,B.Thumbnail AS BrandThumbnail,P.Thumbnail AS ProductThumbnail
,P.Name,P.Code,P.Price,P.ShortDescription,P.Description,B.Description AS BrandDescription
from Products P
JOIN Brands  B ON B.Id = P.BrandId 
WHERE  P.ShelfStatus != '未上架'
AND P.Id = @Id";


				ProductDetailsDto productDetails = await conn.QuerySingleAsync<ProductDetailsDto>(sql,new { Id = id });

				return productDetails;
			}

		}
		public async Task<IEnumerable<ProductStylesDto>> GetProductStyles(int? id)
		{
			using (var conn = _context.CreateConnection())
			{
				string sql = @"
SELECT PS.Id,PS.Style,PS.Inventory
FROM productStyles PS
JOIN Products P ON P.Id =  PS.ProductId
WHERE P.Id = @Id";

				var productStyles = await conn.QueryAsync<ProductStylesDto>(sql, new { Id = id });

				return productStyles;
			}
		}
		public async Task<bool> IsNewProduct (int id)
		{
			using (var conn = _context.CreateConnection())
			{
				DateTime oneAndHalfMonthsAgo = DateTime.Now.AddMonths(-1).AddDays(-20);

				string sql = @"
SELECT 1 
FROM Products P
WHERE P.Id = @Id
AND P.CreateAt >= @OneAndHalfMonthsAgo";

				var result = await conn.ExecuteScalarAsync<int>(sql, new { Id = id, OneAndHalfMonthsAgo = oneAndHalfMonthsAgo });

				return result == 1;
			}
		}

		public async Task<IEnumerable<ProductCommentsDto>> GetProductComments(int? id,int? orderKey)
		{
			using (var conn = _context.CreateConnection())
			{
				string orderStatement = "";
				switch (orderKey)
				{
					case 1:
						orderStatement = "ORDER BY PC.CreateAt DESC,Rating DESC";
						break;
					case 2:
						orderStatement = "ORDER BY PC.CreateAt ASC,Rating DESC";
						break;
					case 3:
						orderStatement = "ORDER BY PC.Rating DESC,PC.CreateAt DESC";
						break;
					case 4:
						orderStatement = "ORDER BY PC.Rating ASC,PC.CreateAt DESC";
						break;
				}

				string sql = @"
SELECT  DISTINCT PC.Rating,PC.Description,PC.CreateAt,PS.Style AS ProductStyle, 
M.Account,M.Thumbnail AS MemberThumbnail
FROM ProductComments PC
JOIN OrderDetails OD ON OD.OrderId = PC.OrderId
JOIN Orders O ON O.Id = OD.OrderId
JOIN Members M ON M.Id = O.MemberId
JOIN ProductStyles PS ON PS.Id = OD.ProductStyleId
WHERE OD.ProductId = PC.ProductId
AND PC.ProductId = @Id "+"\n"+ orderStatement + ";";


				var productComments = await conn.QueryAsync<ProductCommentsDto>(sql, new { Id = id });

				return productComments;
			}
		}
		public async Task<ProductCommentAvgDto> GetProductCommentAVG(int? id)
		{
			using (var conn = _context.CreateConnection())
			{
				string sql = @"
SELECT  DISTINCT COUNT(*) AS CommentCount,
CAST(SUM(Rating) / CAST(COUNT(*) AS decimal) AS decimal(2,1)) AS AvgRating
FROM ProductComments PC
JOIN OrderDetails OD ON OD.OrderId = PC.OrderId
JOIN Orders O ON O.Id = OD.OrderId
WHERE OD.ProductId = PC.ProductId
AND PC.ProductId = @Id;";

				var productCommentAvg = await conn.QuerySingleAsync<ProductCommentAvgDto>(sql, new { Id = id });

				return productCommentAvg;
			}
		}

		public async Task<ProductCategoryDto> GetWhichProductCategory(int? id)
		{
			using ( var conn = _context.CreateConnection())
			{
				string sql = @"
SELECT F.Name AS FirstCategory,S.Name AS SecondCategory,T.Name AS ThirdCategory
FROM Products P
JOIN ThirdCategories T ON T.Id = P.ThirdCategoryId
JOIN SecondCategories S ON S.Id = T.SecondCategoryId
JOIN FirstCategories F ON F.Id = S.FirstCategoryId
WHERE P.Id = @Id;";

				var productCategory = await conn.QuerySingleAsync<ProductCategoryDto>(sql, new { Id = id });

				return productCategory;
			}
		}

		public async Task<ProductImagesDto> GetProductImages(int? id)
		{
			using (var conn = _context.CreateConnection())
			{
				string sql = @"
SELECT [Image]
FROM ProductImages
WHERE ProductId = @Id;";

				var productImages = new ProductImagesDto();
				var getProductImages = await conn.QueryAsync<string>(sql, new { Id = id });
				productImages.Images = getProductImages.ToList();

				return productImages;
			}
		}

		public async Task<IEnumerable<PromotionProductTagsDto>> GetPromotionProductTag()
		{
			using (var conn = _context.CreateConnection())
			{
				string sql = @"
SELECT [Name] AS TagName
FROM ProductTags
WHERE ProductTagCategoryId = 2;"; 

				var productTag = await conn.QueryAsync<PromotionProductTagsDto>(sql);

				return productTag;
			}
		}

		public async Task<string> AddCart(AddCartDto addCart)
		{
			using (var conn = _context.CreateConnection())
			{
				string sql = @"
BEGIN TRANSACTION

DECLARE @CartId INT,@CartDetailsQuantity INT,@ProductStyleInventory INT,@ErrorMsg NVARCHAR(150),@QuantityAvailable INT;

IF(SELECT  COUNT(*) FROM Carts WHERE MemberId = @MemberId) = 0
   INSERT INTO Carts VALUES (@MemberId);

SELECT @CartId = Id FROM Carts
WHERE MemberId =@MemberId ;

SELECT @CartDetailsQuantity = quantity FROM CartDetails
WHERE CartsId = @CartId
AND ProductStylesId = @ProductStyleId;

SELECT @productStyleInventory = Inventory FROM ProductStyles PS
JOIN Products P ON P.Id = PS.ProductId
WHERE PS.Id = @productStyleId
AND  P.ShelfStatus ='上架中';

IF(@productStyleInventory IS NULL)
BEGIN
	ROLLBACK TRANSACTION;
	SET @ErrorMsg = '加入失敗，商品尚未上架!';
	THROW 50000,@ErrorMsg, 1;
END

IF(@productStyleInventory = 0)
BEGIN
	ROLLBACK TRANSACTION;
	SET @ErrorMsg = '加入失敗，商品已完售!';
	THROW 50000,@ErrorMsg, 1;
END


IF (@CartDetailsQuantity IS NULL OR @CartDetailsQuantity < 1)
BEGIN
   IF (@ProductStyleInventory >= @AddProductStyleQuantity)
   BEGIN
	    INSERT INTO CartDetails VALUES (@CartId,@ProductStyleId,@AddProductStyleQuantity)
   END
   ELSE
   BEGIN
		
		ROLLBACK TRANSACTION;
		SET @ErrorMsg =  CONCAT('加入失敗，商品只剩', CAST(@productStyleInventory AS VARCHAR(10)),'件!');
		THROW 50000,@ErrorMsg, 1;
		
   END 
END
ELSE
   IF (@CartDetailsQuantity + @AddProductStyleQuantity  < =  @ProductStyleInventory)
   BEGIN
	    UPDATE CartDetails  
	    SET Quantity = Quantity + @AddProductStyleQuantity
	    WHERE CartsId = @CartId
	    AND ProductStylesId = @ProductStyleId
   END
   ELSE
   BEGIN
		ROLLBACK TRANSACTION;
		IF (@productStyleInventory = @cartDetailsQuantity)
		BEGIN
			SET @ErrorMsg =	'加入失敗，您購物車所選數量已達庫存!';
			THROW 50000,@ErrorMsg, 1;
		END
		ELSE
		BEGIN
			SET @QuantityAvailable = @ProductStyleInventory - @CartDetailsQuantity
			SET @ErrorMsg =	'加入失敗，與購物車的數量已超過庫存，僅能再加'+ CAST(@QuantityAvailable AS VARCHAR(10))+ '件!';
			THROW 50000,@ErrorMsg, 1;
		END
	
   END
COMMIT TRANSACTION
";
				conn.Open();
				using (var transaction = conn.BeginTransaction())
				{
					try
					{
						var addCartParams = new
						{
							addCart.MemberId,
							addCart.ProductStyleId,
							addCart.AddProductStyleQuantity
						};

						await conn.ExecuteAsync(sql, addCartParams, transaction);
						transaction.Commit(); 
						return ""; 

					}
					catch (SqlException ex) when (ex.Number == 50000) 
					{
						transaction.Rollback();
						Console.WriteLine(ex.Message);
						return ex.Message;
					}
					catch (Exception ex)
					{
						transaction.Rollback();
						Console.WriteLine(ex.Message);
						return "";
					}
				}
			}
		}
	}
}
