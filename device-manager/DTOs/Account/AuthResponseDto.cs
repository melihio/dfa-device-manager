namespace dfa_device_manager.API.DTOs.Account;

public class AuthResponseDto
{
    public string Token { get; set; } = null!;
    public DateTime Expires { get; set; }
}