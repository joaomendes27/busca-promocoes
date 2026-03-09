import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Promocao {
  id: string;
  nomeProduto: string;
  urlReferencia: string;
  tituloContexto: string;
  perfilOrigem: string;
  precoEncontrado: number | null;
  encontradaEm: string;
}

@Injectable({
  providedIn: 'root'
})
export class PromocaoService {
  private http = inject(HttpClient);

  obterHistorico(dias: number = 30, palavrasChave?: string): Observable<Promocao[]> {
    let url = `${environment.apiUrl}/promocoes/historico?diasAtras=${dias}`;
    if (palavrasChave && palavrasChave !== 'Tudo') {
      url += `&palavrasChave=${encodeURIComponent(palavrasChave)}`;
    }
    return this.http.get<Promocao[]>(url);
  }

  buscaImediata(dias: number, produtos: string[]): Observable<Promocao[]> {
    const url = `${environment.apiUrl}/promocoes/busca-imediata`;
    return this.http.post<Promocao[]>(url, { dias, produtos });
  }
}
