using BlogApi.Models;
using BlogApi.Repositories;

namespace BlogApi.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _repo;

    public PostService(IPostRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Post>> GetAllAsync() => await _repo.GetAllAsync();

    public async Task<Post?> GetByIdAsync(int id) => await _repo.GetPostCompletoAsync(id);

    public async Task<Post> CreateAsync(Post post)
    {
        post.FechaCreacion = DateTime.UtcNow;
        post.FechaActualizacion = DateTime.UtcNow;
        await _repo.AddAsync(post);
        await _repo.SaveChangesAsync();
        return post;
    }

    public async Task<bool> UpdateAsync(int id, Post post)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null)
            return false;
        existing.Titulo = post.Titulo;
        existing.Contenido = post.Contenido;
        existing.CategoriaId = post.CategoriaId;
        existing.FechaActualizacion = DateTime.UtcNow;
        _repo.Update(existing);
        await _repo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var post = await _repo.GetByIdAsync(id);
        if (post == null)
            return false;
        _repo.Remove(post);
        await _repo.SaveChangesAsync();
        return true;
    }
}
