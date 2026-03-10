import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificacaoService, Notificacao } from '../../core/services/notificacao.service';
import { PerfilService } from '../../core/services/perfil.service';
import { ProdutoService } from '../../core/services/produto.service';
import { RouterModule, Router, NavigationEnd } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { filter, catchError } from 'rxjs/operators';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  notificacaoService = inject(NotificacaoService);
  perfilService = inject(PerfilService);
  produtoService = inject(ProdutoService);
  private router = inject(Router);

  notificacoes = signal<Notificacao[]>([]);
  totalProdutos = signal<number>(0);
  totalPerfis = signal<number>(0);
  totalPromocoes = signal<number>(0);

  ngOnInit() {
    this.carregarDadosIniciais();

    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd && event.urlAfterRedirects === '/dashboard')
    ).subscribe(() => this.carregarDadosIniciais());
  }

  carregarDadosIniciais() {
    this.notificacaoService.obterNaoLidas().subscribe(res => {
      const lista = res || [];
      this.notificacoes.set(lista);
      this.totalPromocoes.set(lista.length);
    });

    forkJoin({
      produtos: this.produtoService.obterTodos().pipe(catchError(() => of([]))),
      perfis: this.perfilService.obterTodos().pipe(catchError(() => of([])))
    }).subscribe({
      next: (dados) => {
        this.totalProdutos.set(dados.produtos?.length || 0);
        this.totalPerfis.set(dados.perfis?.length || 0);
      },
      error: (err) => console.error('Erro ao carregar métricas:', err)
    });
  }

  removerAlerta(id: string) {
    this.notificacaoService.remover(id).subscribe(() => {
      this.notificacoes.update(lista => lista.filter(n => n.id !== id));
    });
  }

  marcarComoLido(notif: Notificacao) {
    if (notif.foiLida) return;
    this.notificacaoService.marcarComoLida(notif.id).subscribe(() => {
      this.notificacoes.update(lista =>
        lista.map(n => n.id === notif.id ? { ...n, foiLida: true } : n)
      );
    });
  }

  naoLidasCount() {
    return this.notificacoes().filter(n => !n.foiLida).length;
  }
}
