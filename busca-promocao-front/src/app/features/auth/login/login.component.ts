import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  dados = { email: '', senha: '' };
  loading = signal(false);
  erro = signal('');

  onSubmit() {
    this.loading.set(true);
    this.erro.set('');

    this.authService.login(this.dados).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/dashboard']);
      },
      error: (err: any) => {
        this.loading.set(false);
        this.erro.set(err?.error?.mensagem || 'Falha no login. Verifique suas credenciais.');
      }
    });
  }
}
