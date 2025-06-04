using System.ComponentModel.DataAnnotations;

namespace dfa_device_manager.API.DTOs.Account;

public class CreateAccountDto
{
    [Required]
    [RegularExpression(@"^[^0-9].*", ErrorMessage = "Username must not start with a digit.")]
    public string Username { get; set; } = null!;

    [Required]
    [MinLength(12, ErrorMessage = "Password must be at least 12 characters.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).*$",
        ErrorMessage = "Password must contain lowercase, uppercase, digit, and symbol.")]
    public string Password { get; set; } = null!;

    [Required] 
    public int EmployeeId { get; set; }
}