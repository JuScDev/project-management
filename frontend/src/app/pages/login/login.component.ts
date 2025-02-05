import { Component, inject, signal } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  imports: [FormsModule],
})
export class LoginComponent {
  public username = signal<string>('');

  public password = signal<string>('');

  public errorMessage = signal<string>('');

  private _authService = inject(AuthService);

  private _router = inject(Router);

  public login(): void {
    this._authService.login(this.username(), this.password()).subscribe({
      next: () => {
        this._router.navigate(['/dashboard']);
      },
      error: () => {
        this.errorMessage.set(
          'Login failed. Please check your username and password.'
        );
      },
    });
  }
}
