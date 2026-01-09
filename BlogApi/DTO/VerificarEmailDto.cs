using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.DTO;

/// <summary>
/// DTO para la verificación de email
/// </summary>
public class VerificarEmailDto
{
    /// <summary>
    /// Token de verificación
    /// </summary>
    public string Token { get; set; } = string.Empty;
}
