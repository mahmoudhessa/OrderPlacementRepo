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
    
    // Check for both short 'role' and full Microsoft claims role
    const userRole = decoded['role'] || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    const roles = Array.isArray(userRole) ? userRole : [userRole];
    return roles.includes(role);
  }

  getUserId(): string | null {
    const token = this.getToken();
    if (!token) return null;
    const decoded = this.jwtHelper.decodeToken(token);
    return decoded?.sub || decoded?.nameid || decoded?.userId || null;
  }
} 