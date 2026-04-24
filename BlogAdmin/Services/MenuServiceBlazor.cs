using System.Net.Http.Json;
using BlogAdmin.Models;

namespace BlogAdmin.Services;

public class MenuServiceBlazor
{
    private readonly HttpClient _http;

    public MenuServiceBlazor(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<MenuItem>> GetMenuAsync()
    {
        return await _http.GetFromJsonAsync<List<MenuItem>>("api/menu")
               ?? new List<MenuItem>();
    }
}
