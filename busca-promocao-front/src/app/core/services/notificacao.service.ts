import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Notificacao {
  id: string;
  titulo: string;
  conteudo: string;
  urlTweet: string;
  handlePerfil: string;
  foiLida: boolean;
  postadoEm: string;
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class NotificacaoService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/notificacoes`;

  obterNaoLidas(dias?: number, produto?: string): Observable<Notificacao[]> {
    let params = new HttpParams();
    if (dias) params = params.set('dias', dias.toString());
    if (produto && produto !== 'Tudo') params = params.set('produto', produto);

    return this.http.get<Notificacao[]>(`${this.apiUrl}/nao-lidas`, { params });
  }

  marcarComoLida(id: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/lida`, {});
  }

  remover(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
