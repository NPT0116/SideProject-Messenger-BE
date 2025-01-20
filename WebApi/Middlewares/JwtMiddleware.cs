using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Middlewares;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _jwtKey;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;

    public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _jwtKey = configuration["Jwt:Key"];
        _jwtIssuer = configuration["Jwt:Issuer"];
        _jwtAudience = configuration["Jwt:Audience"];
    }

    public async Task Invoke(HttpContext context, ITokenService tokenService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
        {
           await  AttachUserIdToContext(context, token, tokenService);
        }

        await _next(context);
    }

    private async Task AttachUserIdToContext(HttpContext context, string token, ITokenService tokenService)
    {
        try
        {
                var userId = tokenService.GetUserIdFromToken(context.Request.Headers["Authorization"].ToString().Replace("Bearer ", ""));

                if (userId != null)
                {
                    context.Items["UserId"] = userId;
                }
            
        }
        catch
        {
            // Token validation failed, do nothing
        }
    }
}