import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./pages/login/login.component').then((x) => x.LoginComponent),
  },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./pages/dashboard/dashboard.component').then(
        (x) => x.DashboardComponent
      ),
    canActivate: [AuthGuard],
  },
  {
    path: 'project-detail/:id',
    loadComponent: () =>
      import('./pages/project-detail/project-detail.component').then(
        (x) => x.ProjectDetailComponent
      ),
    canActivate: [AuthGuard],
  },
  { path: '**', redirectTo: 'login', pathMatch: 'full' },
];
