import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PerfilService, Perfil } from '../../core/services/perfil.service';

@Component({
  selector: 'app-perfis',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './perfis.component.html',
  styleUrl: './perfis.component.scss',
})
export class PerfisComponent implements OnInit {
  service = inject(PerfilService);
  perfis = signal<Perfil[]>([]);
  idEmExclusao = signal<string | null>(null);
  novoHandle = '';
  adicionando = signal(false);
  erro = signal<string | null>(null);

  ngOnInit() {
    this.carregar();
  }

  carregar() {
    this.service.obterTodos().subscribe((res) => this.perfis.set(res || []));
  }

  adicionarPerfil() {
    if (!this.novoHandle) return;
    this.adicionando.set(true);
    this.erro.set(null);
    const handle = this.novoHandle.replace(/^@+/, '').trim();
    this.service.adicionar(handle).subscribe({
      next: () => {
        this.novoHandle = '';
        this.adicionando.set(false);
        this.carregar();
      },
      error: (err) => {
        this.adicionando.set(false);
        const msg =
          err?.error?.Erro || err?.error?.erro || 'Erro ao adicionar perfil.';
        this.erro.set(msg);
      },
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
