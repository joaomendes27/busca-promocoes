import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProdutoService, Produto } from '../../core/services/produto.service';

@Component({
  selector: 'app-produtos',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './produtos.component.html',
  styleUrl: './produtos.component.scss'
})
export class ProdutosComponent implements OnInit {
  service = inject(ProdutoService);
  produtos = signal<Produto[]>([]);
  idEmExclusao = signal<string | null>(null);
  novoProduto = '';

  ngOnInit() {
    this.carregar();
  }

  carregar() {
    this.service.obterTodos().subscribe(res => this.produtos.set(res || []));
  }

  adicionarProduto() {
    if (!this.novoProduto) return;
    this.service.adicionar(this.novoProduto).subscribe(() => {
      this.novoProduto = '';
      this.carregar();
    });
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
