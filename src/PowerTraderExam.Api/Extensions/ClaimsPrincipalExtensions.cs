using System.Security.Claims;
using PowerTraderExam.Api.Auth;

namespace PowerTraderExam.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetServerId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(AuthClaims.ServerId)
            ?? throw new UnauthorizedAccessException("缺少考试服务器身份。");
    }
}
