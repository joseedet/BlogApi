using System;

namespace BlogApi.DTO;

public class PageVersionDto
{
    public int Id { get; set; }
    public int PageId { get; set; }
    public string Titulo { get; set; }
    public string Slug { get; set; }
    public string Contenido { get; set; }
    public bool Publicado { get; set; }
    public bool EsInicio { get; set; }
    public DateTime FechaVersion { get; set; }
    public string IpCreacion { get; set; }
    public string UserAgentCreacion { get; set; }
}
