using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace webapi.DTOs.Account
{
    public class MemberStatusDto
    {
        public Dictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();

        public MemberStatusDto(IEnumerable<Claim> claims)
        {
            foreach (var claim in claims)
            {
                string type = claim.Type.Split("/").Last();
                string value = claim.Value;

                Claims[type] = value;
            }
        }

    }
}
