using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace BlogApi.Authorization;

/// <summary>
/// Requisito de autorización para editar un post
/// </summary>
public class PuedeEditarPostRequirement : IAuthorizationRequirement { }
