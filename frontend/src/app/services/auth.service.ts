import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { AuthTokens } from '../models/auth.models';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = 'https://localhost:5001/api/auth';

  private _http = inject(HttpClient);

  public login(username: string, password: string): Observable<AuthTokens> {
    return this._http
      .post<AuthTokens>(`${this.apiUrl}/login`, { username, password })
      .pipe(tap((tokens) => this._storeTokens(tokens)));
  }

  public logout(): void {
    const refreshToken = this.getRefreshToken();

    if (refreshToken) {
      this._http.post(`${this.apiUrl}/logout`, { refreshToken }).subscribe();
    }

    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
  }

  public getAccessToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  public getRefreshToken(): string | null {
    return localStorage.getItem('refreshToken');
  }

  public refreshToken(): Observable<AuthTokens> {
    const refreshToken = this.getRefreshToken();

    return this._http
      .post<AuthTokens>(`${this.apiUrl}/refresh`, { refreshToken })
      .pipe(tap((tokens) => this._storeTokens(tokens)));
  }

  private _storeTokens(tokens: {
    accessToken: string;
    refreshToken: string;
  }): void {
    localStorage.setItem('accessToken', tokens.accessToken);
    localStorage.setItem('refreshToken', tokens.refreshToken);
  }
}
