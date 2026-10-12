import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BreceptionFilterDto, BreceptionResponseDto, PagedResult } from '../models/wms.models';

@Injectable({ providedIn: 'root' })
export class BreceptionService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7108/api/Breceptions';

  getBreceptions(filter: BreceptionFilterDto): Observable<PagedResult<BreceptionResponseDto>> {
    let params = new HttpParams()
      .set('pageNumber', filter.pageNumber.toString())
      .set('pageSize', filter.pageSize.toString());

    if (filter.dateDebut) params = params.set('dateDebut', filter.dateDebut);
    if (filter.dateFin) params = params.set('dateFin', filter.dateFin);
    //if (filter.idFournisseur) params = params.set('idFournisseur', filter.idFournisseur.toString());
    //if (filter.idDepot) params = params.set('idDepot', filter.idDepot.toString());
    if (filter.idUser) params = params.set('idUser', filter.idUser.toString());
    if (filter.refArticle) params = params.set('refArticle', filter.refArticle);
    if (filter.idFamille) params = params.set('idFamille', filter.idFamille.toString());

    return this.http.get<PagedResult<BreceptionResponseDto>>(this.apiUrl, { params });
  }

  getBreceptionById(id: number): Observable<BreceptionResponseDto> {
    return this.http.get<BreceptionResponseDto>(`${this.apiUrl}/${id}`);
  }
}