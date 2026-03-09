import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PromocaoService, Promocao } from '../../core/services/promocao.service';
import { ProdutoService, Produto } from '../../core/services/produto.service';

@Component({
  selector: 'app-promocoes',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './promocoes.component.html',
  styleUrl: './promocoes.component.scss'
})
export class PromocoesComponent implements OnInit {
  promocaoService = inject(PromocaoService);
  produtoService = inject(ProdutoService);
  
  promocoes = signal<Promocao[]>([]);
  produtosWishlist = signal<Produto[]>([]);
  produtoSelecionado: string = 'Tudo';
  
  loading = signal<boolean>(false);
  diasSelecionados: number = 15;
  buscou: boolean = false;

  ngOnInit() {
    this.carregarFiltros();
  }

  carregarFiltros() {
    this.produtoService.obterTodos().subscribe(res => {
        const prods = res || [];
        this.produtosWishlist.set(prods);
    });
  }

  buscarImediato() {
    this.buscou = true;
    this.loading.set(true);
    
    const produtosTarget = this.produtoSelecionado === 'Tudo' 
        ? this.produtosWishlist().map(p => p.nome) 
        : [this.produtoSelecionado];
    
    this.promocaoService.buscaImediata(this.diasSelecionados, produtosTarget).subscribe({
      next: (res: Promocao[]) => {
        this.promocoes.set(res || []);
        this.loading.set(false);
      },
      error: (err: any) => {
        console.error('Erro ao fazer busca imediata:', err);
        this.loading.set(false);
      }
    });
  }
}
