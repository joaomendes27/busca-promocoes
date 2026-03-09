import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Perfil {
  id: string;
  handlePerfil: string;
  criadoEm: string;
}

@Injectable({
  providedIn: 'root'
})
export class PerfilService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/perfis`;

  obterTodos(): Observable<Perfil[]> {
    return this.http.get<Perfil[]>(this.apiUrl);
  }

  adicionar(handlePerfil: string): Observable<string> {
    return this.http.post<string>(this.apiUrl, { handlePerfil });
  }

  // Not implemented in backend yet, but ready on frontend:
  remover(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
