using System.ComponentModel.DataAnnotations;

namespace dfa_device_manager.API.DTOs.Account;

public class AuthRequestDto
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}