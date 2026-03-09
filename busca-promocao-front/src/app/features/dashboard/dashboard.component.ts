import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificacaoService, Notificacao } from '../../core/services/notificacao.service';
import { PerfilService } from '../../core/services/perfil.service';
import { ProdutoService } from '../../core/services/produto.service';
import { PromocaoService } from '../../core/services/promocao.service';
import { RouterModule } from '@angular/router';
import { forkJoin } from 'rxjs';

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
  promocaoService = inject(PromocaoService);

  notificacoes = signal<Notificacao[]>([]);
  totalProdutos = signal<number>(0);
  totalPerfis = signal<number>(0);
  totalPromocoes = signal<number>(0);

  ngOnInit() {
    this.carregarDadosIniciais();
  }

  carregarDadosIniciais() {
    this.notificacaoService.obterNaoLidas().subscribe(res => {
      this.notificacoes.set(res || []);
    });

    forkJoin({
      produtos: this.produtoService.obterTodos(),
      perfis: this.perfilService.obterTodos(),
      promocoes: this.promocaoService.obterHistorico(30)
    }).subscribe({
      next: (dados) => {
        this.totalProdutos.set(dados.produtos?.length || 0);
        this.totalPerfis.set(dados.perfis?.length || 0);
        this.totalPromocoes.set(dados.promocoes?.length || 0);
      },
      error: (err) => console.error('Erro ao carregar métricas:', err)
    });
  }
}
