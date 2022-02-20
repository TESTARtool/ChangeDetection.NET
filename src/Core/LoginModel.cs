using System.ComponentModel.DataAnnotations;

namespace Testar.ChangeDetection.Core;

public class LoginModel
{
    [Required]
    public string Username { get; init; }

    [Required]
    public string Password { get; init; }
}