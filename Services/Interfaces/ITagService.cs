using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Models;

namespace BlogApi.Services.Interfaces;

public interface ITagService
{
    Task<IEnumerable<Tag>> GetAllAsync();
    Task<Tag?> GetByIdAsync(int id);
    Task<Tag> CreateAsync(Tag tag);
    Task<bool> UpdateAsync(int id, Tag tag);
    Task<bool> DeleteAsync(int id);
    //IQueryable<Tag> Query();
}
