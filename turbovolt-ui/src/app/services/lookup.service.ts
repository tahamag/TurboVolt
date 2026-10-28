import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  ClientResponseDto,
  FamilleArticleResponseDto,
  SousFamilleArticleResponseDto,
  UserResponseDto
} from '../models/wms.models';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class LookupService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Lookups`;

  getClients(): Observable<ClientResponseDto[]> {
    return this.http.get<ClientResponseDto[]>(`${this.apiUrl}/clients`);
  }

  getUsers(): Observable<UserResponseDto[]> {
    return this.http.get<UserResponseDto[]>(`${this.apiUrl}/users`);
  }

  getFamilles(): Observable<FamilleArticleResponseDto[]> {
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