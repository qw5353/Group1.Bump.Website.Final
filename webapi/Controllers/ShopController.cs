using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Core.Types;
using Swashbuckle.AspNetCore.Filters;
using System.Drawing.Printing;
using webapi.DTOs.Shop;
using webapi.Helpers;
using webapi.Models;
using webapi.Repositories.DapperRepositories;

namespace webapi.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class ShopController : Controller
	{

		private readonly ShopRepository _repository;
		public ShopController(ShopRepository repository)
		{
			_repository = repository;
		}

		//新商品 幻燈片
		[HttpGet]
		[Route("NewProductGroup")]
		public async Task<ActionResult<IEnumerable<ProductDto>>> GetNewProductGroup()
		{
			var productGroup = await _repository.GetNewProductGroup();
			return Ok(productGroup);
		}

		//促銷中 幻燈片
		[HttpGet]
		[Route("OnSaleProductGroup")]
		public async Task<ActionResult<IEnumerable<ProductDto>>> GetOnSaleProductGroup(int typeId = 1)
		{
			throw new NotImplementedException();
		}

		//熱銷中(愛不釋手) 幻燈片
		[HttpGet]
		[Route("SellsWellProductGroup")]
		public async Task<ActionResult<IEnumerable<ProductDto>>> GetSellsWellProductGroup()
		{
			var productGroup = await _repository.GetSellsWellProductGroup();
			return Ok(productGroup);

		}

		////相似商品 幻燈片
		//[HttpGet]
		//[Route("SimilarProductGroup")]
		//public async Task<ActionResult<IEnumerable<ProductDto>>> GetSimilarProductGroup(int typeId = 1)
		//{
		//	throw new NotImplementedException();

		//}
		[HttpPost]
		[Route("ProductList")]
		[SwaggerRequestExample(typeof(ProductFilterDto), typeof(ProductFilterExample))]
		public async Task<ActionResult<ProductListDto>> GetProductList(ProductFilterDto productFilterDto)
		{
			ProductListDto productList = new ProductListDto();

			int productsCount = await _repository.GetProductListCount(productFilterDto);
			int totalPages = productsCount % productFilterDto.PageSize == 0 ? productsCount / productFilterDto.PageSize : productsCount / productFilterDto.PageSize + 1;
			productList.TotalPage = totalPages;

			if (productsCount < 1)
			{
				return productList;
			}

			var products = await _repository.GetProductList(productFilterDto);
			productList.Products = products.ToList();
			return Ok(productList);
		}

		[HttpPost]
		[Route("ProductListBrands")]
		public async Task<ActionResult<IEnumerable<string>>> GetProductListBrands(ProductFilterDto productFilterDto)
		{
			var brands = await _repository.GetProductListBrands(productFilterDto);
			return Ok(brands);
		}

		[HttpPost]
		[Route("ProductListPriceRange")]
		public async Task<ActionResult<(int,int)>> GetProductListPriceRange(ProductFilterDto productFilterDto)
		{
			(int minPrice, int maxPrice) price = await _repository.GetProductListPriceRange(productFilterDto);
			var result = new {minPrice = price.minPrice,maxPrice = price.maxPrice };
			return Ok(result);
		}
		[HttpGet]
		[Route("ProductCategories")]
		public async Task<ActionResult<IEnumerable<ProductCategoriesDto>>> GetProductCategories(string? firstCategory)
		{
			var categories = await _repository.GetProductCategories(firstCategory);

			return Ok(categories);
		}

		[HttpGet]
		[Route("ProductSecondCategories")]
		public async Task<ActionResult<IEnumerable<string?>>> GetSecondCategories(string? firstCategory)
		{
			var categories = await _repository.GetSecondCategories(firstCategory);
			IEnumerable<string?> result = categories.Select(r => { return r.SecondCategory; });
			return Ok(result);
		}

		[HttpGet]
		[Route("ProductThirdCategories")]
		public async Task<ActionResult<IEnumerable<string?>>> GetThirdCategories(string? firstCategory, string? secoondCategory)
		{
			var categories = await _repository.GetThirdCategories(firstCategory, secoondCategory);
			IEnumerable<string?> result = categories.Select(r => { return r.ThirdCategory; });
			return Ok(result);
		}

		[HttpGet]
		[Route("ProductDetails")]
		public async Task<ActionResult<ProductDetailsDto>> GetProductDetails(int? id)
		{
			var categories = await _repository.GetProductDetails(id);

			return Ok(categories);
		}

		[HttpGet]
		[Route("ProductStyles")]
		public async Task<ActionResult<IEnumerable<ProductStylesDto>>> GetProductStyles(int? id)
		{
			var productStyles = await _repository.GetProductStyles(id);

			return Ok(productStyles);
		}

		[HttpPost]
		[Route("AddCart")]
		public async Task<ActionResult<IEnumerable<ProductStylesDto>>> AddCart(AddCartDto addCart)
		{
			var message = await _repository.AddCart(addCart);

			if (string.IsNullOrWhiteSpace(message))
			{
				return Ok();
			}
			else
			{
				return BadRequest(message);
			}
		}

		[HttpGet]
		[Route("ProductComments")]
		public async Task<ActionResult<IEnumerable<ProductCommentsDto>>> GetProductComments(int? id,int? orderKey = 1)
		{
			var comments = await _repository.GetProductComments(id, orderKey);
			return Ok(comments);
		}
		[HttpGet]
		[Route("ProductCommentAvg")]
		public async Task<ActionResult<IEnumerable<ProductCommentsDto>>> GetProductCommentAVG(int? id)
		{
			var commentsAvg = await _repository.GetProductCommentAVG(id);
			return Ok(commentsAvg);
		}
		[HttpGet]
		[Route("WhichProductCategory")]
		public async Task<ActionResult<IEnumerable<ProductCategoryDto>>> GetWhichProductCategory(int? id)
		{
			var productCategory = await _repository.GetWhichProductCategory(id);
			return Ok(productCategory);
		}

		[HttpGet]
		[Route("PromotionProductTags")]
		public async Task<ActionResult<IEnumerable<PromotionProductTagsDto>>> GetPromotionProductTag()
		{
			var productTags = await _repository.GetPromotionProductTag();
			return Ok(productTags);
		}

		[HttpGet]
		[Route("ProductImages")]
		public async Task<ActionResult<ProductImagesDto>> GetProductImages(int? id)
		{
			var productImages = await _repository.GetProductImages(id);
			return Ok(productImages);
		}

		[HttpGet]
		[Route("IsNewProduct")]
		public async Task<ActionResult<bool>> IsNewProduct(int id)
		{
			bool result = await _repository.IsNewProduct(id);
			return Ok(result);
		}

		[HttpGet]
		[Route("Brands")]
		public async Task<ActionResult<IEnumerable<BrandsDto>>> GetBrands()
		{
			var result = await _repository.GetBrands();
			return Ok(result);
		}


		[HttpGet]
		[Route("BrandDesc")]
		public async Task<ActionResult<string>> GetBrandDesc(string? name)
		{
			var result = await _repository.GetBrandDesc(name);
			return Ok(result.BrandDesc);
		}
	}
}

public class ProductFilterExample : IMultipleExamplesProvider<ProductFilterDto>
{
	public IEnumerable<SwaggerExample<ProductFilterDto>> GetExamples()
	{
		yield return SwaggerExample.Create(
			"filter",
			new ProductFilterDto
			{
				Page = 1,
				PageSize = 5,
				Keyword = "",
				FirstCategory = "攀登",
				SecondCategories = null,
				ProductStyle = null,
				ThirdCategories = null,
				MinPrice = null, 
				MaxPrice = null,
				BrandName = null,
			}
		);
	}

}
