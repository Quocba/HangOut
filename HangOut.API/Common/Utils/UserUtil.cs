using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace HangOut.API.Common.Utils
{
    public static class UserUtil
    {
        public static Guid? GetAccountId(HttpContext httpContext)
        {
            if (httpContext == null || httpContext.User == null)
            {
                return null;
            }

            var accountIdClaim = httpContext.User.FindFirst("AccountId");
            if (accountIdClaim == null)
            {
                return null;
            }

            if (!Guid.TryParse(accountIdClaim.Value, out Guid accountId))
            {
                throw new BadHttpRequestException($"Invalid AccountId: {accountIdClaim.Value}");
            }

            return accountId;
        }


        public static string GetRole(HttpContext httpContext)
        {
            if (httpContext == null || httpContext.User == null || !httpContext.User.Identity.IsAuthenticated)
                return null;

            var roleClaim = httpContext.User.FindFirst(ClaimTypes.Role);
            if (roleClaim == null) {

                return null;
            }
            return roleClaim?.Value;
        }

        public static string GenerateVoucherCode()
        {
            return $"VCH-{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}";
        }
    }
}
