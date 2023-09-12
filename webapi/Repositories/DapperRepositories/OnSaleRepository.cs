using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using webapi.Context;
using webapi.DTOs.Activities;
using webapi.DTOs.Shop;

namespace webapi.Repositories.DapperRepositories
{
    public class OnSaleRepository
    {
        private readonly BumpDapperContext _context;
        public OnSaleRepository(BumpDapperContext context)
        {
            _context = context;
        }

        public async Task<int> GetProductListCount(OnSaleFilterDto onSaleFilterDto)
        {
            var parameters = new DynamicParameters();
            (string whereConditions, DynamicParameters parametersResult) whereStatement = GetProductFilterWhereStatment(parameters, onSaleFilterDto);


            string sql = @"
SELECT COUNT(*)
FROM Products P
JOIN TagOfProducts TP on TP.ProductId = P.Id
JOIN ProductTags PT on PT.Id = TP.ProductTagId
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
        public async Task<IEnumerable<ProductDto>> GetProductList(OnSaleFilterDto onSaleFilterDto)
        {
            var page = onSaleFilterDto.Page;
            var pageSize = onSaleFilterDto.PageSize;

            var parameters = new DynamicParameters(new
            {
                OffsetRows = (page - 1) * pageSize,
                FetchRows = pageSize
            });
            (string whereConditions, DynamicParameters parametersResult) whereStatement = GetProductFilterWhereStatment(parameters, onSaleFilterDto);


            string sql = @"
SELECT P.Id,P.Name,P.Price,P.Thumbnail,
B.Name as BrandName, PT.Name as ProductTagName
FROM Products P
JOIN TagOfProducts TP on TP.ProductId = P.Id
JOIN ProductTags PT on PT.Id = TP.ProductTagId
JOIN Brands B on B.Id = P.BrandId 
JOIN ThirdCategories T on T.Id =  P.ThirdCategoryId
JOIN SecondCategories S on S.Id = T.SecondCategoryId
JOIN FirstCategories F on F.Id = S.FirstCategoryId " +
'\n' + whereStatement.whereConditions +
@" ORDER BY P.Id
OFFSET @OffsetRows ROWS
FETCH NEXT @FetchRows ROWS ONLY; ";

            using (var conn = _context.CreateConnection())
            {
                return await conn.QueryAsync<ProductDto>(sql,
                whereStatement.parametersResult);

            }
        }
        private (string, DynamicParameters) GetProductFilterWhereStatment(DynamicParameters parameters, OnSaleFilterDto onSaleFilterDto)
        {
            string whereConditions = "WHERE P.ShelfStatus = '上架中' \n";

            if (onSaleFilterDto.ProductTagNames != null && onSaleFilterDto.ProductTagNames.Count > 0)
            {
                string where = "";
                int i = 0;
                foreach (string ProductTagNames in onSaleFilterDto.ProductTagNames)
                {
                    i++;
                    where = where + $",@ProductTagNames{i}";
                    parameters.Add($"ProductTagNames{i}", $"{ProductTagNames}");
                }
                whereConditions = whereConditions + $" AND PT.Name IN (" + where.Substring(1) + ") \n";
            }

            if (!onSaleFilterDto.FirstCategory.IsNullOrEmpty())
            {
                whereConditions = whereConditions + $" AND F.Name = @FirstCategory \n";
                parameters.Add("FirstCategory", $"{onSaleFilterDto.FirstCategory}");
            }

            if (onSaleFilterDto.SecondCategories != null && onSaleFilterDto.SecondCategories.Count > 0)
            {
                string where = "";
                int i = 0;
                foreach (string SecondCategories in onSaleFilterDto.SecondCategories)
                {
                    i++;
                    where = where + $",@SecondCategories{i}";
                    parameters.Add($"SecondCategories{i}", $"{SecondCategories}");
                }
                whereConditions = whereConditions + $" AND S.Name IN (" + where.Substring(1) + ") \n";
            }

            if (onSaleFilterDto.ThirdCategories != null && onSaleFilterDto.ThirdCategories.Count > 0)
            {
                string where = "";
                int i = 0;
                foreach (string ThirdCategories in onSaleFilterDto.ThirdCategories)
                {
                    i++;
                    where = where + $",@ThirdCategories{i}";
                    parameters.Add($"ThirdCategories{i}", $"{ThirdCategories}");
                }
                whereConditions = whereConditions + $" AND T.Name IN (" + where.Substring(1) + ") \n";
            }

            if (onSaleFilterDto.BrandName != null && onSaleFilterDto.BrandName.Count > 0)
            {
                string where = "";
                int i = 0;
                foreach (string BrandName in onSaleFilterDto.BrandName)
                {
                    i++;
                    where = where + $",@BrandName{i}";
                    parameters.Add($"BrandName{i}", $"{BrandName}");
                }
                whereConditions = whereConditions + $" AND B.Name IN (" + where.Substring(1) + ") \n";
            }

            if (onSaleFilterDto.ProductStyle != null && onSaleFilterDto.ProductStyle.Count > 0)
            {
                string where = "";
                int i = 0;
                foreach (string ProductStyle in onSaleFilterDto.ProductStyle)
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

            if (onSaleFilterDto.MinPrice.HasValue)
            {
                whereConditions = whereConditions + $" AND P.Price >= @MinPrice \n";
                parameters.Add("MinPrice", onSaleFilterDto.MinPrice);
            }

            if (onSaleFilterDto.MaxPrice.HasValue)
            {
                whereConditions = whereConditions + $" AND P.Price <= @MaxPrice \n";
                parameters.Add("MaxPrice", onSaleFilterDto.MaxPrice);
            }

            return (whereConditions, parameters);
        }
    }
}
