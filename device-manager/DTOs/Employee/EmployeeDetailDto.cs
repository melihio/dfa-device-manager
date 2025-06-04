namespace dfa_device_manager.API.DTOs.EmployeeDtos;

public class EmployeeDetailDto
{
    // From Person
    public int PersonId { get; set; }
    public string PassportNumber { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;

    // From Employee
    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }

    // Position object
    public PositionDto Position { get; set; } = null!;
}