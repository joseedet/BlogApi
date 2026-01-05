using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAdmin.Services;

public class AuthService
{
    private string? _token;

    public void SetToken(string token)
    {
        _token = token;
    }

    public string? GetToken()
    {
        return _token;
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(_token);
}
