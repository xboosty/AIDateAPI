using Newtonsoft.Json;
using System.Security.Claims;

namespace APICore.Utils
{
    public static class Extensions
    {
        public static string ToForgotPasswordEmail(this string resource, string password)
        {
            return resource.Replace("#PASSWORD#", password);
        }

        public static int GetUserIdFromToken(this ClaimsPrincipal user)
        {
            var claimsIdentity = user.Identity as ClaimsIdentity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.UserData)?.Value;

            if (userId == null)
                return 0;

            return int.Parse(userId);
        }

        public static void AddPagingHeaders(this HttpResponse response, object paginationData)
        {
            response.Headers.Add("PagingData", JsonConvert.SerializeObject(paginationData));
            response.Headers["Access-Control-Expose-Headers"] = "PagingData";
            response.Headers["Access-Control-Allow-Headers"] = "PagingData";
        }

    }
}