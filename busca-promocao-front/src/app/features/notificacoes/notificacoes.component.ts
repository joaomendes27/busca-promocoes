import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NotificacaoService, Notificacao } from '../../core/services/notificacao.service';
import { ProdutoService, Produto } from '../../core/services/produto.service';

@Component({
  selector: 'app-notificacoes',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './notificacoes.component.html',
  styleUrl: './notificacoes.component.scss'
})
export class NotificacoesComponent implements OnInit {
  service = inject(NotificacaoService);
  produtoService = inject(ProdutoService);
  
  notificacoes = signal<Notificacao[]>([]);
  produtosWishlist = signal<Produto[]>([]);
  produtoSelecionado = signal<string>('Tudo');
  diasSelecionados = signal<number>(15);
  idEmExclusao = signal<string | null>(null);

  ngOnInit() {
    this.carregarProdutos();
    this.carregar();
  }

  carregarProdutos() {
    this.produtoService.obterTodos().subscribe(res => this.produtosWishlist.set(res || []));
  }

  carregar() {
    this.service.obterNaoLidas(this.diasSelecionados(), this.produtoSelecionado()).subscribe(
      res => this.notificacoes.set(res || [])
    );
  }

  marcarLida(id: string) {
    this.service.marcarComoLida(id).subscribe(() => this.carregar());
  }

  iniciarExclusao(id: string) {
    this.idEmExclusao.set(id);
  }

  cancelarExclusao() {
    this.idEmExclusao.set(null);
  }

  confirmarExclusao(id: string) {
    this.service.remover(id).subscribe(() => {
      this.idEmExclusao.set(null);
      this.carregar();
    });
  }
}
