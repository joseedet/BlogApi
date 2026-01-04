using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.DTO;
using BlogApi.Models;

namespace BlogApi.Services;

public interface IPostService
{
    Task<IEnumerable<Post>> GetAllAsync();
    Task<Post?> GetByIdAsync(int id);

    //Task<Post> CreateAsync(Post post);
    //Task<bool> UpdateAsync(int id, Post post);
    Task<bool> DeleteAsync(int id);
    Task<Post> CreateAsync(Post post, List<int> tagIds);

    //Task<bool> UpdateAsync(int id, Post post, List<int> tagIds);
    Task<bool> UpdateAsync(int id, Post post, List<int> tagIds, bool puedeEditarTodo);
    Task<PaginationDto<Post>> GetPagedAsync(int pagina, int tamano);
    Task<Post?> GetBySlugAsync(string slug);
}
