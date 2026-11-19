import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  AvoirBenificeByUser,
  AvoirBenificeDetail,
  AvoirBenificeFilter,
}from '../models/wms.models';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AvoirBenificeService {
  
  
  private apiUrl = `${environment.apiUrl}/AvoirBenifice`;

  constructor(private http: HttpClient) {}

  private buildParams(filter: AvoirBenificeFilter): HttpParams {
    let params = new HttpParams();

    if (filter.userId != null) {
      params = params.set('userId', filter.userId.toString());
    }
    if (filter.articleId != null) {
      params = params.set('articleId', filter.articleId.toString());
    }
    if (filter.articleSearch && filter.articleSearch.trim() !== '') {
      params = params.set('articleSearch', filter.articleSearch.trim());
    }
    if (filter.dateDebut) {
      params = params.set('dateDebut', filter.dateDebut);
    }
    if (filter.dateFin) {
      params = params.set('dateFin', filter.dateFin);
    }

    return params;
  }

  getDetails(filter: AvoirBenificeFilter = {}): Observable<AvoirBenificeDetail[]> {
    const params = this.buildParams(filter);
    return this.http.get<AvoirBenificeDetail[]>(this.apiUrl, { params });
  }

  getByUser(filter: AvoirBenificeFilter = {}): Observable<AvoirBenificeByUser[]> {
    const params = this.buildParams(filter);
    return this.http.get<AvoirBenificeByUser[]>(`${this.apiUrl}/by-user`, { params });
  }
}