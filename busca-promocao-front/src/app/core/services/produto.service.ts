import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Produto {
  id: string;
  nome: string;
  dataCriacao: string;
}

@Injectable({
  providedIn: 'root'
})
export class ProdutoService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/wishlist`;

  obterTodos(): Observable<Produto[]> {
    return this.http.get<Produto[]>(this.apiUrl);
  }

  adicionar(nome: string): Observable<string> {
    return this.http.post<string>(this.apiUrl, { nomeProduto: nome });
  }

  remover(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
