using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;
using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using TurboVolt.DTOs;

namespace TurboVolt.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto request);
    }
    public class AuthService : IAuthService
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;
        private readonly IValidator<LoginRequestDto> _validator;

        public AuthService(IConfiguration configuration, IValidator<LoginRequestDto> validator)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration));
            _configuration = configuration;
            _validator = validator;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto request)
        {
            await _validator.ValidateAndThrowAsync(request);

            using var connection = CreateConnection();

            // Sélection de l'utilisateur par Login ou Nom/Prénom dans la table [USER]
            string sql = @"
                SELECT TOP 1
                    IdUser AS IdUser,
                    NOM AS Nom,
                    PRENOM AS Prenom,
                    Username AS Login,
                    Password AS Password
                FROM dbo.[USER]
                WHERE (Username = @Username OR NOM = @Username)
              AND (SUPPRIME IS NULL OR SUPPRIME = 0);";

            var userRecord = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Username = request.Username.Trim() });

            if (userRecord == null)
                return null;

            // Vérification du mot de passe (adaptez selon votre méthode de hachage si nécessaire)
            string storedPassword = userRecord.Password ?? string.Empty;
            if (storedPassword != request.Password)
                return null;

            var userDto = new UserResponseDto
            {
                IdUser = userRecord.IdUser,
                Nom = userRecord.Nom,
                Prenom = userRecord.Prenom
            };

            // Génération du Token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? "TurboVoltSecretKeyForJwtAuthentication2026!");
            var tokenExpiration = DateTime.UtcNow.AddHours(8);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, userDto.IdUser.ToString()),
                new Claim(ClaimTypes.Name, userDto.Nom ?? string.Empty),
                new Claim(ClaimTypes.GivenName, userDto.Prenom ?? string.Empty)
            }),
                Expires = tokenExpiration,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"] ?? "TurboVoltAPI",
                Audience = _configuration["Jwt:Audience"] ?? "TurboVoltDashboard"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new LoginResponseDto
            {
                Token = tokenHandler.WriteToken(token),
                Expiration = tokenExpiration,
                User = userDto
            };
        }
    }
}
