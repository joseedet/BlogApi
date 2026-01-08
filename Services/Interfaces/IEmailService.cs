using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApi.Services.Interfaces;

public interface IEmailService
{
    Task EnviarAsync(string toEmail, string subject, string message);
}
