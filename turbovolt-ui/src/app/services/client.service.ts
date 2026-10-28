import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ClientResponseDto } from '../models/wms.models';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class ClientService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Lookups/clients`;

  getClients(): Observable<ClientResponseDto[]> {
    console.log(this.apiUrl)
    return this.http.get<ClientResponseDto[]>(this.apiUrl);
  }
}