using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Data;
using Microsoft.EntityFrameworkCore;
using BlogApi.Services.Interfaces;
using BlogApi.Models;

namespace BlogApi.Services.Interfaces;

public interface IMenuService
{
     Task<List<MenuItem>> GetMenuAsync();
}
