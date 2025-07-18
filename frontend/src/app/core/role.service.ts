import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({ providedIn: 'root' })
export class RoleService {
  constructor(private jwtHelper: JwtHelperService) {}

  getRoles(): string[] {
    const token = localStorage.getItem('jwt_token');
    if (!token) return [];
    const decoded = this.jwtHelper.decodeToken(token);
    if (!decoded) return [];
    return Array.isArray(decoded['role']) ? decoded['role'] : [decoded['role']];
  }

  hasRole(role: string): boolean {
    return this.getRoles().includes(role);
  }
} 