using Mapster;
using TurboVolt.DTOs;
using TurboVolt.Models;

namespace TurboVolt.Mappings
{
    public static class MapsterConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<Article, ArticleResponseDto>.NewConfig()
                .Map(dest => dest.LibelleFamille, src => src.Famillearticle != null ? src.Famillearticle.LibelleFamArticle: null)
                .Map(dest => dest.LibelleSousFamille, src => src.Sousfamillearticle != null ? src.Sousfamillearticle.LibelleSousFamille : null);

            TypeAdapterConfig<Blivraison, BlivraisonResponseDto>.NewConfig()
                .Map(dest => dest.NomClient, src => src.Client != null ? src.Client.NomClient : null)
                .Map(dest => dest.Telephone, src => src.Client != null ? src.Client.NumTele : null)
                .Map(dest => dest.Ville, src => src.Client != null ? src.Client.Ville : null)
                .Map(dest => dest.AgentCreateur, src => src.IduserNavigation != null ? src.IduserNavigation.NomComplet : null)
                .Map(dest => dest.AgentModificateur, src => src.IdUserModificationNavigation != null ? src.IdUserModificationNavigation.NomComplet : null)
                .Map(dest => dest.BL_Articles, src => src.BlivraisonXArticle);
        }
    }
}
