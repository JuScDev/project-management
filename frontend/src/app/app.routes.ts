import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'auth',
    loadComponent: () =>
      import('./login/login.component').then((x) => x.LoginComponent),
  },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./dashboard/dashboard.component').then(
        (x) => x.DashboardComponent
      ),
  },
  { path: '', redirectTo: 'auth', pathMatch: 'full' },
];
