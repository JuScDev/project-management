import { Component, inject, signal } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
  imports: [
    FormsModule,
    RouterModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
  ],
})
export class RegisterComponent {
  public username = signal<string>('');

  public password = signal<string>('');

  public errorMessage = signal<string>('');

  private _authService = inject(AuthService);

  private _router = inject(Router);

  private _snackBar = inject(MatSnackBar);

  public register(): void {
    this._authService.register(this.username(), this.password()).subscribe({
      next: () => {
        this._snackBar.open('Registration successful!', 'Close', {
          duration: 3000,
        });
        this._router.navigate(['/login']);
      },
      error: () => {
        this.errorMessage.set('Registration failed. Please try again.');
        this._snackBar.open(this.errorMessage(), 'Close', {
          duration: 3000,
          panelClass: 'snackbar-error',
        });
      },
    });
  }
}
