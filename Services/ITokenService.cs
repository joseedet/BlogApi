using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Models;

namespace BlogApi.Services;

public interface ITokenService
{
    string GenerateToken(Usuario usuario);
}
