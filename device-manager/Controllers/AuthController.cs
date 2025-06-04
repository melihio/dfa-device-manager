using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using dfa_device_manager.API.DTOs.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace dfa_device_manager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly DfaDeviceManagerContext _context;
    private readonly IConfiguration _config;

    public AuthController(DfaDeviceManagerContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] AuthRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var account = await _context.Accounts
            .Include(a => a.Role)
            .SingleOrDefaultAsync(a => a.Username == dto.Username);

        if (account == null)
            return Unauthorized();
        
        var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<string>();
        var result = hasher.VerifyHashedPassword(null, account.PasswordHash, dto.Password);
        if (result != Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success)
            return Unauthorized();
        
        var jwtSection = _config.GetSection("Jwt");
        var keyBytes = Encoding.UTF8.GetBytes(jwtSection["Key"]);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new Claim(ClaimTypes.Name, account.Username),
            new Claim(ClaimTypes.Role, account.Role.Name)
        };
        var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSection["ExpireMinutes"]!)),
            signingCredentials: creds
        );

        return Ok(new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expires = token.ValidTo
        });
    }
}