using webapi.Repositories.DapperRepositories;

namespace webapi.DTOs.Activities
{
	public class CartActivityDiscountsDto
	{
        public CartVM[] CartVM { get; set; }
        public int MemberId { get; set; }
    }
}
