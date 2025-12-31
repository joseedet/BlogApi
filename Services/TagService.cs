using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Models;
using BlogApi.Repositories;

namespace BlogApi.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _repo;

    public TagService(ITagRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Tag>> GetAllAsync() => await _repo.GetAllAsync();

    public async Task<Tag?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    public async Task<Tag> CreateAsync(Tag tag)
    {
        await _repo.AddAsync(tag);
        await _repo.SaveChangesAsync();
        return tag;
    }

    public async Task<bool> UpdateAsync(int id, Tag tag)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null)
            return false;
        existing.Nombre = tag.Nombre;
        _repo.Update(existing);
        await _repo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null)
            return false;
        _repo.Remove(existing);
        await _repo.SaveChangesAsync();
        return true;
    }
}
