import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpRequest,
} from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, Observable, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class AuthInterceptor {
  private _isRefreshing = false;

  private _authService = inject(AuthService);

  public intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const accessToken = this._authService.getAccessToken();

    let clonedReq = req;
    if (accessToken) {
      clonedReq = req.clone({
        setHeaders: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
    }

    return next.handle(clonedReq).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401 && !this._isRefreshing) {
          this._isRefreshing = true;

          return this._authService.refreshToken().pipe(
            switchMap((newTokens) => {
              this._isRefreshing = false;

              clonedReq = req.clone({
                setHeaders: {
                  Authorization: `Bearer ${newTokens.accessToken}`,
                },
              });

              return next.handle(clonedReq);
            })
          );
        }
        return throwError(() => error);
      })
    );
  }
}
