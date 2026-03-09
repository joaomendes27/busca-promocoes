import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../core/services/auth.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss'
})
export class LayoutComponent {
  authService = inject(AuthService);
  menuAberto = signal(false);

  toggleMenu() {
    this.menuAberto.update(estado => !estado);
  }

  fecharMenuMobile() {
    if (window.innerWidth <= 768) {
      this.menuAberto.set(false);
    }
  }

  sair() {
    this.authService.logout();
  }
}
