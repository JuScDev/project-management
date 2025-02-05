import { Component, inject } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
})
export class NavbarComponent {
  private _authService = inject(AuthService);

  private _router = inject(Router);

  public logout(): void {
    this._authService.logout();
    this._router.navigate(['/login']);
  }
}
