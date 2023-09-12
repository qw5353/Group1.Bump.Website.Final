using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using webapi.DTOs.Activities;
using webapi.DTOs.Shop;
using webapi.Repositories.DapperRepositories;

namespace webapi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OnSaleController : Controller
    {
        private readonly OnSaleRepository _repository;
        public OnSaleController(OnSaleRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [Route("OnSaleProductList")]
        [SwaggerRequestExample(typeof(OnSaleFilterDto), typeof(OnSaleFilterExample))]
        public async Task<ActionResult<ProductListDto>> GetProductList(OnSaleFilterDto onSaleFilterDto)
        {
            ProductListDto productList = new ProductListDto();

            int productsCount = await _repository.GetProductListCount(onSaleFilterDto);
            int totalPages = productsCount % onSaleFilterDto.PageSize == 0 ? productsCount / onSaleFilterDto.PageSize : productsCount / onSaleFilterDto.PageSize + 1;
            productList.TotalPage = totalPages;

            if (productsCount < 1)
            {
                return productList;
            }

            var products = await _repository.GetProductList(onSaleFilterDto);
            productList.Products = products.ToList();
            return Ok(productList);
        }

        public class OnSaleFilterExample : IMultipleExamplesProvider<OnSaleFilterDto>
        {
            public IEnumerable<SwaggerExample<OnSaleFilterDto>> GetExamples()
            {
                yield return SwaggerExample.Create(
                    "filter",
                    new OnSaleFilterDto
                    {
                        ProductTagNames= null,
                        Page = 1,
                        PageSize = 5,
                        FirstCategory = "",
                        SecondCategories = null,
                        ProductStyle = null,
                        ThirdCategories = null,
                        MinPrice = null,
                        MaxPrice = null,
                        BrandName = null,
                    }
                );;
            }

        }
    }
}
