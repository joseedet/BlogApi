using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.DTO;


public class LogoutDto
{
    public string RefreshToken { get; set; } = string.Empty;
}
