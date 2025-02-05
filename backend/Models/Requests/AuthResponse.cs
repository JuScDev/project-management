
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPI.Models;

public class AuthResponse
{
    [Required]
    public string AccessToken { get; set; } = string.Empty;

    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}