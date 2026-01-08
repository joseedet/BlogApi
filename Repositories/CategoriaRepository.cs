using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Repositories.Interfaces;

namespace BlogApi.Repositories;

public class CategoriaRepository : GenericRepository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(BlogDbContext context)
        : base(context) { }
}
