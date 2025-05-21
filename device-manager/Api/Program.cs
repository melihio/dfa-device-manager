using System.Text.Json;
using dfa_device_manager.API;
using dfa_device_manager.API.DTOs.DeviceDtos;
using dfa_device_manager.API.DTOs.EmployeeDtos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DfaDeviceManagerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

app.UseHttpsRedirection();


app.MapGet("/api/devices/", (DfaDeviceManagerContext context) =>
{
    var devices = new List<DeviceDto>();
    foreach (var d in context.Devices)
    {
        devices.Add(new DeviceDto(d.Id, d.Name));
    }
    return Results.Ok(devices);
});


app.MapGet("/api/devices/{id}", async (DfaDeviceManagerContext context, int id) =>
{
    var device = await context.Devices
        .Include(d => d.DeviceType)
        .Include(d => d.DeviceEmployees.Where(de => de.ReturnDate == null))
        .ThenInclude(de => de.Employee)
        .ThenInclude(e => e.Person)
        .FirstOrDefaultAsync(d => d.Id == id);

    if (device == null)
        return Results.NotFound();

    var currentEmployee = device.DeviceEmployees.FirstOrDefault()?.Employee;

    var dto = new DeviceDetailsDto
    {
        Name = device.Name,
        DeviceTypeName = device.DeviceType?.Name ?? "Unknown",
        IsEnabled = device.IsEnabled,
        AdditionalProperties = JsonSerializer.Deserialize<object>(device.AdditionalProperties ?? "{}"),
        CurrentEmployee = currentEmployee == null ? null : new CurrentEmployeeDto
        {
            Id = currentEmployee.Id,
            Name = $"{currentEmployee.Person.FirstName} {currentEmployee.Person.LastName}"
        }
    };

    return Results.Ok(dto);
});


app.MapPost("/api/devices", async (DfaDeviceManagerContext context, CreateDeviceDto dto) =>
{
    var deviceType = await context.DeviceTypes
        .FirstOrDefaultAsync(dt => dt.Name == dto.DeviceTypeName);

    if (deviceType == null)
        return Results.BadRequest("Device type does not exist.");

    var created = new Device
    {
        Name = dto.Name,
        IsEnabled = dto.IsEnabled,
        AdditionalProperties = JsonSerializer.Serialize(dto.AdditionalProperties),
        DeviceTypeId = deviceType.Id
    };

    context.Devices.Add(created);
    await context.SaveChangesAsync();

    return Results.Created($"/api/devices/{created.Id}", new { created.Id });
});


app.MapPut("/api/devices/{id}", async (DfaDeviceManagerContext context, int id, CreateDeviceDto dto) =>
{
    var device = await context.Devices.FindAsync(id);
    if (device == null)
        return Results.NotFound();               
    
    var deviceType = await context.DeviceTypes
        .FirstOrDefaultAsync(dt => dt.Name == dto.DeviceTypeName);
    if (deviceType == null)
        return Results.BadRequest("Device type does not exist.");
    
    device.Name                 = dto.Name;
    device.IsEnabled            = dto.IsEnabled;
    device.AdditionalProperties = JsonSerializer.Serialize(dto.AdditionalProperties);
    device.DeviceTypeId         = deviceType.Id;
    
    await context.SaveChangesAsync();
    
    return Results.NoContent();
});

app.MapDelete("/api/devices/{id}", async (DfaDeviceManagerContext context, int id) =>
{
    var device = await context.Devices.FindAsync(id);
    if (device == null)
        return Results.NotFound();

    context.Devices.Remove(device);
    await context.SaveChangesAsync();
    
    return Results.NoContent();
});

app.MapGet("/api/employees", async (DfaDeviceManagerContext context) =>
{
    var list = await context.Employees
        .Include(e => e.Person)
        .Select(e => new EmployeeListDto {
            Id = e.Id,
            FullName = $"{e.Person.FirstName} {(string.IsNullOrEmpty(e.Person.MiddleName) ? "" : e.Person.MiddleName + " ")}{e.Person.LastName}"
        })
        .ToListAsync();

    return Results.Ok(list);
});

app.MapGet("/api/employees/{id}", async (DfaDeviceManagerContext context, int id) =>
{
    var emp = await context.Employees
        .Include(e => e.Person)
        .Include(e => e.Position)
        .Where(e => e.Id == id)
        .Select(e => new EmployeeDetailDto {
            PersonId = e.Person.Id,
            PassportNumber = e.Person.PassportNumber,
            FirstName = e.Person.FirstName,
            MiddleName = e.Person.MiddleName,
            LastName = e.Person.LastName,
            PhoneNumber = e.Person.PhoneNumber,
            Email = e.Person.Email,
            Salary = e.Salary,
            HireDate = e.HireDate,
            Position = new PositionDto {
                Id = e.Position.Id,
                Name = e.Position.Name
            }
        })
        .FirstOrDefaultAsync();

    return emp is null
        ? Results.NotFound()
        : Results.Ok(emp);
});

app.Run();