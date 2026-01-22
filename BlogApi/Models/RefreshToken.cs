namespace BlogApi.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime Expira { get; set; }

        public DateTime Creado { get; set; }

        public DateTime? Revocado { get; set; }

        public string? ReemplazadoPor { get; set; }

        // Relación con Usuario
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public bool EstaActivo => Revocado == null && !EstaExpirado;
        public bool EstaExpirado => DateTime.UtcNow >= Expira;
    }
}
