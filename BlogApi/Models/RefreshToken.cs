namespace BlogApi.Models
{
    /// <summary>
    /// Refresh Token
    /// </summary>
    public class RefreshToken
    {
        /// <summary>
        /// Identificador
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de expiración
        /// </summary>
        public DateTime Expira { get; set; }

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime Creado { get; set; }

        /// <summary>
        /// Fecha de revocación
        /// </summary>
        public DateTime? Revocado { get; set; }

        /// <summary>
        /// Reemplazado por
        /// </summary>
        public string? ReemplazadoPor { get; set; }

        /// <summary>
        /// Id del usuario
        /// </summary>
        // Relación con Usuario
        public int UsuarioId { get; set; }

        /// <summary>
        /// Llave de navegación
        /// </summary>
        public Usuario Usuario { get; set; } = null!;

        /// <summary>
        /// Si el token está activo
        /// </summary>
        public bool EstaActivo => Revocado == null && !EstaExpirado;

        /// <summary>
        /// Si ha expirado el token
        /// </summary>
        public bool EstaExpirado => DateTime.UtcNow >= Expira;
    }
}
