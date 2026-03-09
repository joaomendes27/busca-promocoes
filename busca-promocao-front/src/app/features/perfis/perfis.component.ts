import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PerfilService, Perfil } from '../../core/services/perfil.service';

@Component({
  selector: 'app-perfis',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './perfis.component.html',
  styleUrl: './perfis.component.scss'
})
export class PerfisComponent implements OnInit {
  service = inject(PerfilService);
  perfis = signal<Perfil[]>([]);
  idEmExclusao = signal<string | null>(null);
  novoHandle = '';

  ngOnInit() {
    this.carregar();
  }

  carregar() {
    this.service.obterTodos().subscribe(res => this.perfis.set(res || []));
  }

  adicionarPerfil() {
    if (!this.novoHandle) return;
    this.service.adicionar(this.novoHandle).subscribe(() => {
      this.novoHandle = '';
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
