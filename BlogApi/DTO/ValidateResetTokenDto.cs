using System;

namespace BlogApi.DTO;

/// <summary>
/// Valida el resest del token
/// </summary>
public class ValidateResetTokenDto
{
    /// <summary>
    /// correo electrónico
    /// </summary>/
    public string Email { get; set; }

    /// <summary>
    /// Token
    /// </summary>
    public string Token { get; set; }
}
