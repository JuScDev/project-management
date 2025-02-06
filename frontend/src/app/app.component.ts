import { Component, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
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
    RouterOutlet,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    MatToolbarModule,
    MatButtonModule,
  ],
})
export class AppComponent {
  private _authService = inject(AuthService);

  private _router = inject(Router);

  navbarOpened: boolean = false;

  handleBackdropClick() {
    this.navbarOpened = false;
  }

  toggleNavbar() {
    this.navbarOpened = !this.navbarOpened;
  }

  public logout(): void {
    this._authService.logout();
    this._router.navigate(['/login']);
  }
}
