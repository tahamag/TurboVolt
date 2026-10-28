import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BeneficeResponseDto } from '../models/wms.models';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class BeneficeService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Benefices`;

  getBenefices(idUser?: number, dateDebut?: string, dateFin?: string): Observable<BeneficeResponseDto[]> {
    let params = new HttpParams();
    if (idUser) params = params.set('idUser', idUser.toString());
    if (dateDebut) params = params.set('dateDebut', dateDebut);
    if (dateFin) params = params.set('dateFin', dateFin);

    return this.http.get<BeneficeResponseDto[]>(this.apiUrl, { params });
  }
}