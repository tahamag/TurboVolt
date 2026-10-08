import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private platformId = inject(PLATFORM_ID);
  private apiUrl = 'https://localhost:7108/api/Auth';

  private isBrowser(): boolean {
    return isPlatformBrowser(this.platformId);
  }

  login(credentials: { username: string; password: string }): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, credentials).pipe(
      tap(res => {
        if (res.token && this.isBrowser()) {
          localStorage.setItem('jwt_token', res.token);
          localStorage.setItem('user_info', JSON.stringify(res.user));
        }
      })
    );
  }

  logout(): void {
    if (this.isBrowser()) {
      localStorage.removeItem('jwt_token');
      localStorage.removeItem('user_info');
    }
  }

  isLoggedIn(): boolean {
    if (this.isBrowser()) {
      return !!localStorage.getItem('jwt_token');
    }
    return false;
  }

  getUser(): any {
    if (this.isBrowser()) {
      const user = localStorage.getItem('user_info');
      return user ? JSON.parse(user) : null;
    }
    return null;
  }
}