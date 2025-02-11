import { Component, inject } from '@angular/core';
import { Router, RouterModule, RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth.service';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  imports: [
    RouterModule,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    MatToolbarModule,
    MatButtonModule,
  ],
})
export class AppComponent {
  public authService = inject(AuthService);

  private _router = inject(Router);

  navbarOpened: boolean = false;

  closeSidebar() {
    this.navbarOpened = false;
  }

  toggleNavbar() {
    this.navbarOpened = !this.navbarOpened;
  }

  public logout(): void {
    this.authService.logout();
    this._router.navigate(['/login']);
  }
}
