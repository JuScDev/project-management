
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPI.Models;

public class RegisterResponse
{
    [Required]
    public bool Success { get; set; } = false;

    [Required]
    public string Message { get; set; } = string.Empty;
}