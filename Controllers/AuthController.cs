using GameMatchmaking.Models;
using GameMatchmaking.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameMatchmaking.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        if(string.IsNullOrWhiteSpace(request.Username) || request.Username.Length > 20)
            return BadRequest("Username must be 1-20 characters.");
        if(string.IsNullOrEmpty(request.Password) || request.Password.Length < 8)
            return BadRequest("Password must be at least 8 characters");
        
        var result = await _auth.RegisterAsync(request.Username, request.Password);
        if(result is null)
            return Conflict("That username is taken.");

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var result = await _auth.LoginAsync(request.Username, request.Password);
        if(result is null)
            return Conflict("Wrong username or password");

        return Ok(result);
    }
}