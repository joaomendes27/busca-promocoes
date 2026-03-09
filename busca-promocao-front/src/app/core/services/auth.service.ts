import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, map, Observable, of, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface AuthResponse {
  token: string;
  usuario: {
    id: string;
    nome: string;
    email: string;
  };
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private readonly TOKEN_KEY = 'bp_token';

  // Using signals for reactive state
  isAuthenticated = signal<boolean>(this.hasToken());
  currentUser = signal<any | null>(null);

  constructor() {
    if (this.hasToken()) {
      // In a real app we might validate the token or fetch user details here
    }
  }

  login(dados: any): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/Auth/login`, dados).pipe(
      tap(res => {
        this.setToken(res.token);
        this.isAuthenticated.set(true);
        this.currentUser.set(res.usuario);
      })
    );
  }

  register(dados: any): Observable<any> {
    return this.http.post(`${environment.apiUrl}/Auth/registrar`, dados);
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this.isAuthenticated.set(false);
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  private hasToken(): boolean {
    return !!this.getToken();
  }

  private setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }
}
