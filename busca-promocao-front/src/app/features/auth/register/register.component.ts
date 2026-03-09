import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  dados = { nome: '', email: '', senha: '' };
  loading = signal(false);
  erro = signal('');
  sucesso = signal('');

  onSubmit() {
    this.loading.set(true);
    this.erro.set('');
    this.sucesso.set('');

    this.authService.register(this.dados).subscribe({
      next: () => {
        this.loading.set(false);
        this.sucesso.set('Conta criada com sucesso! Redirecionando...');
        setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: (err: any) => {
        this.loading.set(false);
        this.erro.set(err?.error?.mensagem || 'Falha no registro.');
      }
    });
  }
}
