import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BlivraisonFilterDto, BlivraisonResponseDto, PagedResult } from '../models/wms.models';

@Injectable({ providedIn: 'root' })
export class BlivraisonService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7108/api/Blivraisons';

  getBlivraisons(filter: BlivraisonFilterDto): Observable<PagedResult<BlivraisonResponseDto>> {
    let params = new HttpParams()
      .set('pageNumber', filter.pageNumber.toString())
      .set('pageSize', filter.pageSize.toString());

    if (filter.dateDebut) params = params.set('dateDebut', filter.dateDebut);
    if (filter.dateFin) params = params.set('dateFin', filter.dateFin);
    if (filter.idClient) params = params.set('idClient', filter.idClient.toString());
    if (filter.idUser) params = params.set('idUser', filter.idUser.toString());
    if (filter.refArticle) params = params.set('refArticle', filter.refArticle);
    if (filter.idFamille) params = params.set('idFamille', filter.idFamille.toString());
    if (filter.idSousFamille) params = params.set('idSousFamille', filter.idSousFamille.toString());

    return this.http.get<PagedResult<BlivraisonResponseDto>>(this.apiUrl, { params });
  }

  getBlivraisonById(id: number): Observable<BlivraisonResponseDto> {
    return this.http.get<BlivraisonResponseDto>(`${this.apiUrl}/${id}`);
  }
}