namespace TurboVolt.DTOs
{
    public class UserResponseDto
    {
        public int IdUser { get; set; }
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public string NomComplet => $"{Nom} {Prenom}".Trim();
        
    }

    public class UserModificateurResponseDto
    {
        public int IdUserModificateur { get; set; }
        public string? NomModificateur { get; set; }
        public string? PrenomModificateur { get; set; }
        public string NomComplet => $"{NomModificateur} {PrenomModificateur}".Trim();

    }
}
