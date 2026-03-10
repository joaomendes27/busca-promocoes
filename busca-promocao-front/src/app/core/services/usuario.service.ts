import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface AtualizarEmailPayload {
  novoEmail: string;
  senhaAtual: string;
}

export interface AtualizarSenhaPayload {
  senhaAtual: string;
  novaSenha: string;
}

@Injectable({
  providedIn: 'root'
})
export class UsuarioService {
  private http = inject(HttpClient);

  atualizarEmail(payload: AtualizarEmailPayload): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/Usuarios/AtualizarEmail`, payload);
  }

  atualizarSenha(payload: AtualizarSenhaPayload): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/Usuarios/AtualizarSenha`, payload);
  }
}
