import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UsuarioService } from '../../core/services/usuario.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-usuario',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './usuario.component.html',
  styleUrl: './usuario.component.scss'
})
export class UsuarioComponent {
  private usuarioService = inject(UsuarioService);
  authService = inject(AuthService);

  dadosEmail = { novoEmail: '', senhaAtual: '' };
  dadosSenha = { senhaAtual: '', novaSenha: '', confirmarNovaSenha: '' };

  loadingEmail = signal(false);
  loadingSenha = signal(false);
  erroEmail = signal('');
  sucessoEmail = signal('');
  erroSenha = signal('');
  sucessoSenha = signal('');

  atualizarEmail() {
    this.erroEmail.set('');
    this.sucessoEmail.set('');
    this.loadingEmail.set(true);

    this.usuarioService.atualizarEmail({
      novoEmail: this.dadosEmail.novoEmail,
      senhaAtual: this.dadosEmail.senhaAtual
    }).subscribe({
      next: () => {
        this.loadingEmail.set(false);
        this.sucessoEmail.set('E-mail atualizado com sucesso!');
        this.dadosEmail = { novoEmail: '', senhaAtual: '' };
        const usuario = this.authService.currentUser();
        if (usuario) {
          this.authService.currentUser.set({ ...usuario, email: this.dadosEmail.novoEmail });
        }
      },
      error: (err: any) => {
        this.loadingEmail.set(false);
        this.erroEmail.set(err?.error?.mensagem || 'Erro ao atualizar e-mail.');
      }
    });
  }

  atualizarSenha() {
    this.erroSenha.set('');
    this.sucessoSenha.set('');

    if (this.dadosSenha.novaSenha !== this.dadosSenha.confirmarNovaSenha) {
      this.erroSenha.set('A nova senha e a confirmação não coincidem.');
      return;
    }

    this.loadingSenha.set(true);

    this.usuarioService.atualizarSenha({
      senhaAtual: this.dadosSenha.senhaAtual,
      novaSenha: this.dadosSenha.novaSenha
    }).subscribe({
      next: () => {
        this.loadingSenha.set(false);
        this.sucessoSenha.set('Senha atualizada com sucesso!');
        this.dadosSenha = { senhaAtual: '', novaSenha: '', confirmarNovaSenha: '' };
      },
      error: (err: any) => {
        this.loadingSenha.set(false);
        this.erroSenha.set(err?.error?.mensagem || 'Erro ao atualizar senha.');
      }
    });
  }
}
