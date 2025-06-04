using System.Security.Claims;
using dfa_device_manager.API.DTOs.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dfa_device_manager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly DfaDeviceManagerContext _context;
    private readonly IConfiguration _config;

    public AccountsController(DfaDeviceManagerContext context, IConfiguration config)
    {
        _context = context;
        _config  = config;
    }
    
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] CreateAccountDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _context.Accounts.AnyAsync(a => a.Username == dto.Username))
            return Conflict("Username already exists.");

        var employee = await _context.Employees.FindAsync(dto.EmployeeId);
        if (employee == null)
            return NotFound("Employee not found.");

        var userRole = await _context.Roles.SingleAsync(r => r.Name == "User");
        
        var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<string>();
        var hashed = hasher.HashPassword(null, dto.Password);

        var account = new Account
        {
            Username     = dto.Username,
            PasswordHash = hashed,
            RoleId       = userRole.Id,
            EmployeeId   = dto.EmployeeId
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = account.Id },
            new { account.Id, account.Username }
        );
    }
    
    [HttpGet("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetById(int id)
    {
        var account = await _context.Accounts
            .Include(a => a.Role)
            .SingleOrDefaultAsync(a => a.Id == id);

        if (account == null)
            return NotFound();

        return Ok(new
        {
            account.Id,
            account.Username,
            RoleName     = account.Role.Name,
            account.EmployeeId
        });
    }
    
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAll()
    {
        var list = await _context.Accounts
            .Include(a => a.Role)
            .Select(a => new
            {
                a.Id,
                a.Username,
                RoleName     = a.Role.Name,
                a.EmployeeId
            })
            .ToListAsync();
        return Ok(list);
    }
    
    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateAccountDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var account = await _context.Accounts.FindAsync(id);
        if (account == null)
            return NotFound();

        if (await _context.Accounts.AnyAsync(a => a.Username == dto.Username && a.Id != id))
            return Conflict("Username already in use by another account.");

        var employee = await _context.Employees.FindAsync(dto.EmployeeId);
        if (employee == null)
            return NotFound("Employee not found.");

        var role = await _context.Roles.SingleAsync(r => r.Name == "User"); 

        var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<string>();
        var hashed = hasher.HashPassword(null, dto.Password);

        account.Username     = dto.Username;
        account.PasswordHash = hashed;
        account.RoleId       = role.Id;
        account.EmployeeId   = dto.EmployeeId;

        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account == null)
            return NotFound();

        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpGet("me")]
    [Authorize(Policy = "UserOnly")]
    public async Task<IActionResult> GetMyAccount()
    {
        var accountIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(accountIdClaim, out var id))
            return Forbid();

        var account = await _context.Accounts
            .Include(a => a.Role)
            .Include(a => a.Employee)
                .ThenInclude(e => e.Person)
            .SingleOrDefaultAsync(a => a.Id == id);

        if (account == null)
            return NotFound();

        return Ok(new
        {
            account.Id,
            account.Username,
            RoleName     = account.Role.Name,
            Employee     = new
            {
                account.Employee.Person.FirstName,
                account.Employee.Person.LastName,
                account.Employee.Person.Email
            }
        });
    }
}