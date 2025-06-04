using System.Text;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using dfa_device_manager.API;
using dfa_device_manager.API.DTOs.DeviceDtos;
using dfa_device_manager.API.DTOs.EmployeeDtos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DfaDeviceManagerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"]);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/api/devices", async (DfaDeviceManagerContext context) =>
{
    var devices = await context.Devices
        .Select(d => new DeviceDto(d.Id, d.Name))
        .ToListAsync();
    return Results.Ok(devices);
})
.RequireAuthorization("AdminOnly");

app.MapGet("/api/devices/{id}", async (DfaDeviceManagerContext context, int id, HttpContext http) =>
{
    var role = http.User.FindFirstValue(ClaimTypes.Role);
    if (role == "Admin")
    {
        var device = await context.Devices
            .Include(d => d.DeviceType)
            .Include(d => d.DeviceEmployees.Where(de => de.ReturnDate == null))
            .ThenInclude(de => de.Employee)
            .ThenInclude(e => e.Person)
            .FirstOrDefaultAsync(d => d.Id == id);
        if (device == null) return Results.NotFound();
        var currentEmployee = device.DeviceEmployees.FirstOrDefault()?.Employee;
        var dto = new DeviceDetailsDto
        {
            Name = device.Name,
            DeviceTypeName = device.DeviceType?.Name ?? "Unknown",
            IsEnabled = device.IsEnabled,
            AdditionalProperties = JsonSerializer.Deserialize<object>(device.AdditionalProperties ?? "{}"),
            CurrentEmployee = currentEmployee == null
                ? null
                : new CurrentEmployeeDto
                {
                    Id = currentEmployee.Id,
                    Name = $"{currentEmployee.Person.FirstName} {currentEmployee.Person.LastName}"
                }
        };
        return Results.Ok(dto);
    }
    else if (role == "User")
    {
        var empIdClaim = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(empIdClaim, out var empId)) return Results.Forbid();
        var device = await context.Devices
            .Include(d => d.DeviceType)
            .Include(d => d.DeviceEmployees.Where(de => de.ReturnDate == null && de.EmployeeId == empId))
            .ThenInclude(de => de.Employee)
            .ThenInclude(e => e.Person)
            .FirstOrDefaultAsync(d => d.Id == id);
        if (device == null || !device.DeviceEmployees.Any()) return Results.Forbid();
        var currentEmployee = device.DeviceEmployees.First().Employee;
        var dto = new DeviceDetailsDto
        {
            Name = device.Name,
            DeviceTypeName = device.DeviceType?.Name ?? "Unknown",
            IsEnabled = device.IsEnabled,
            AdditionalProperties = JsonSerializer.Deserialize<object>(device.AdditionalProperties ?? "{}"),
            CurrentEmployee = new CurrentEmployeeDto
            {
                Id = currentEmployee.Id,
                Name = $"{currentEmployee.Person.FirstName} {currentEmployee.Person.LastName}"
            }
        };
        return Results.Ok(dto);
    }
    else
    {
        return Results.Forbid();
    }
})
.RequireAuthorization();

app.MapPost("/api/devices", async (DfaDeviceManagerContext context, CreateDeviceDto dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name) ||
        string.IsNullOrWhiteSpace(dto.DeviceTypeName) ||
        dto.AdditionalProperties.ValueKind == JsonValueKind.Undefined)
    {
        return Results.BadRequest("Invalid payload");
    }
    var propsJson = JsonSerializer.Serialize(dto.AdditionalProperties);
    var deviceType = await context.DeviceTypes.FirstOrDefaultAsync(dt => dt.Name == dto.DeviceTypeName);
    if (deviceType == null) return Results.BadRequest("DeviceType not found");
    var created = new Device
    {
        Name = dto.Name,
        IsEnabled = dto.IsEnabled,
        AdditionalProperties = propsJson,
        DeviceTypeId = deviceType.Id
    };
    context.Devices.Add(created);
    await context.SaveChangesAsync();
    return Results.Created($"/api/devices/{created.Id}", new { created.Id });
})
.RequireAuthorization("AdminOnly");

app.MapPut("/api/devices/{id}", async (DfaDeviceManagerContext context, int id, CreateDeviceDto dto) =>
{
    var device = await context.Devices.FindAsync(id);
    if (device == null) return Results.NotFound();
    var deviceType = await context.DeviceTypes.FirstOrDefaultAsync(dt => dt.Name == dto.DeviceTypeName);
    if (deviceType == null) return Results.BadRequest("DeviceType not found");
    device.Name = dto.Name;
    device.IsEnabled = dto.IsEnabled;
    device.AdditionalProperties = JsonSerializer.Serialize(dto.AdditionalProperties);
    device.DeviceTypeId = deviceType.Id;
    await context.SaveChangesAsync();
    return Results.NoContent();
})
.RequireAuthorization("AdminOnly");

app.MapDelete("/api/devices/{id}", async (DfaDeviceManagerContext context, int id) =>
{
    var device = await context.Devices.FindAsync(id);
    if (device == null) return Results.NotFound();
    context.Devices.Remove(device);
    await context.SaveChangesAsync();
    return Results.NoContent();
})
.RequireAuthorization("AdminOnly");

app.MapGet("/api/employees", async (DfaDeviceManagerContext context) =>
{
    var list = await context.Employees
        .Include(e => e.Person)
        .Select(e => new EmployeeListDto
        {
            Id = e.Id,
            FullName = $"{e.Person.FirstName} {(string.IsNullOrEmpty(e.Person.MiddleName) ? "" : e.Person.MiddleName + " ")}{e.Person.LastName}"
        })
        .ToListAsync();
    return Results.Ok(list);
})
.RequireAuthorization();

app.MapGet("/api/employees/{id}", async (DfaDeviceManagerContext context, int id) =>
{
    var emp = await context.Employees
        .Include(e => e.Person)
        .Include(e => e.Position)
        .Where(e => e.Id == id)
        .Select(e => new EmployeeDetailDto
        {
            PersonId = e.Person.Id,
            PassportNumber = e.Person.PassportNumber,
            FirstName = e.Person.FirstName,
            MiddleName = e.Person.MiddleName,
            LastName = e.Person.LastName,
            PhoneNumber = e.Person.PhoneNumber,
            Email = e.Person.Email,
            Salary = e.Salary,
            HireDate = e.HireDate,
            Position = new PositionDto
            {
                Id = e.Position.Id,
                Name = e.Position.Name
            }
        })
        .FirstOrDefaultAsync();
    return emp is null ? Results.NotFound() : Results.Ok(emp);
})
.RequireAuthorization();

app.Run();
