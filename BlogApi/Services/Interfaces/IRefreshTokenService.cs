using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.Services.Interfaces;

public interface IRefreshTokenService
{
    RefreshToken GenerarRefreshToken(int usuarioId);
    Task GuardarRefreshTokenAsync(RefreshToken token);
    Task<RefreshToken?> ObtenerRefreshTokenAsync(string token);
    Task RevocarRefreshTokenAsync(RefreshToken token, string? reemplazadoPor = null);
    Task RevocarTokensDelUsuarioAsync(int usuarioId);
}
