namespace TurboVolt.DTOs
{
    public class UserResponseDto
    {
        public int IdUser { get; set; }
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public string NomComplet => $"{Nom} {Prenom}".Trim();
        
    }
}
