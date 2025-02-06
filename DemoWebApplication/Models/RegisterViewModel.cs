using System.ComponentModel.DataAnnotations;

namespace DemoWebApplication.Models;

public class RegisterViewModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "密碼與確認密碼不一致")]
    public string ConfirmPassword { get; set; }
}