import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { LayoutComponent } from './layout/layout.component';
import { authGuard } from './core/guards/auth.guard';

// Lazy loading the features to improve bundle size
export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'registrar', component: RegisterComponent },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { 
        path: 'dashboard', 
        loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) 
      },
      { 
        path: 'promocoes', 
        loadComponent: () => import('./features/promocoes/promocoes.component').then(m => m.PromocoesComponent) 
      },
      { 
        path: 'perfis', 
        loadComponent: () => import('./features/perfis/perfis.component').then(m => m.PerfisComponent) 
      },
      { 
        path: 'produtos', 
        loadComponent: () => import('./features/produtos/produtos.component').then(m => m.ProdutosComponent) 
      },
      { 
        path: 'notificacoes', 
        loadComponent: () => import('./features/notificacoes/notificacoes.component').then(m => m.NotificacoesComponent) 
      }
    ]
  },
  { path: '**', redirectTo: 'dashboard' } // wildcard route
];
