using BlogApi.Models;

namespace BlogApi.Repositories.Interfaces;

public interface IPostRepository : IGenericRepository<Post>
{
    Task<Post?> GetPostCompletoAsync(int id);
}
