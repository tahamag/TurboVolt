import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ArticleFilterDto, ArticleResponseDto, PagedResult } from '../models/wms.models';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class ArticleService {
  private http = inject(HttpClient);
  
  private apiUrl = `${environment.apiUrl}/Articles`;

  getArticles(filter: ArticleFilterDto): Observable<PagedResult<ArticleResponseDto>> {
    let params = new HttpParams()
      .set('pageNumber', filter.pageNumber.toString())
      .set('pageSize', filter.pageSize.toString());

    if (filter.searchTerm) params = params.set('searchTerm', filter.searchTerm.trim());
    if (filter.idFamille) params = params.set('idFamille', filter.idFamille.toString());
    if (filter.idSousFamille) params = params.set('idSousFamille', filter.idSousFamille.toString());

    return this.http.get<PagedResult<ArticleResponseDto>>(this.apiUrl, { params });
  }

  getArticleById(id: number): Observable<ArticleResponseDto> {
    return this.http.get<ArticleResponseDto>(`${this.apiUrl}/${id}`);
  }
}