using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Data;
using BlogApi.Models;

namespace BlogApi.Repositories;

public class TagRepository : GenericRepository<Tag>, ITagRepository
{
    public TagRepository(BlogDbContext context)
        : base(context) { }
}
