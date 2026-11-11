using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace GameMatchmaking.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetPlayerID(this ClaimsPrincipal user)
    {
        var sub = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if(sub is null) throw new InvalidOperationException("Token has no 'sub' claim.");

        return int.Parse(sub);
    }
}