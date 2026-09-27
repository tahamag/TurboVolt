using TurboVolt.Models;

namespace TurboVolt.DTOs
{
     record  BlivraisonDtos
     (
         int IdBonLivraison ,
         string NomClient ,
         string User,
         string RefBonLivraison ,
         DateTime DateLivraison ,
         string Description ,
         decimal TotalBonLivraisonHt ,
         decimal TotalBonLivraisonTtc ,
         decimal ResteAPayer ,
         decimal TotalRemise 
     );
}
