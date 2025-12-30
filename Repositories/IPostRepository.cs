using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Models;

namespace BlogApi.Repositories;

public interface IPostRepository : IGenericRepository<Post>
{
    Task<Post?> GetPostCompletoAsync(int id);
}
