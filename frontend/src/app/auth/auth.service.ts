import { Injectable } from '@angular/core';
import { ApiBaseService } from '../core/api-base.service';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private tokenKey = 'jwt_token';
  private jwtHelper = new JwtHelperService();

  constructor(private api: ApiBaseService) {}

  login(email: string, password: string): Observable<{ token: string }> {
    return this.api.post<{ token: string }>('/api/auth/login', { email, password }).pipe(
      tap(res => {
        localStorage.setItem(this.tokenKey, res.token);
      })
    );
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
  }

  getToken() {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn() {
    return !!this.getToken();
  }

  hasRole(role: string): boolean {
    const token = this.getToken();
    if (!token) return false;
    const decoded = this.jwtHelper.decodeToken(token);
    if (!decoded) return false;
    const roles = Array.isArray(decoded['role']) ? decoded['role'] : [decoded['role']];
    return roles.includes(role);
  }
} 