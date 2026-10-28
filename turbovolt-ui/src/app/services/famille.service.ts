import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FamilleArticleResponseDto, SousFamilleArticleResponseDto } from '../models/wms.models';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class FamilleService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Lookups`;

  getFamilles(): Observable<FamilleArticleResponseDto[]> {
    console.log(this.http.get<FamilleArticleResponseDto[]>(`${this.apiUrl}/familles`))
    return this.http.get<FamilleArticleResponseDto[]>(`${this.apiUrl}/familles`);
  }

  getSousFamilles(idFamille?: number): Observable<SousFamilleArticleResponseDto[]> {
    let params = new HttpParams();
    if (idFamille) {
      params = params.set('idFamille', idFamille.toString());
    }
    return this.http.get<SousFamilleArticleResponseDto[]>(`${this.apiUrl}/sous-familles`, { params });
  }
}