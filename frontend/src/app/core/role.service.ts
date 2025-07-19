import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({ providedIn: 'root' })
export class RoleService {
  constructor(private jwtHelper: JwtHelperService) {}

  getRoles(): string[] {
    const token = localStorage.getItem('jwt_token');
    if (!token) {
      return [];
    }
    
    const decoded = this.jwtHelper.decodeToken(token);
    if (!decoded) {
      return [];
    }
    
    // Check for all possible role claim names
    const possibleRoleClaims = [
      'role',
      'roles',
      'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
      'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role',
      'ClaimTypes.Role'
    ];
    
    let role = null;
    for (const claimName of possibleRoleClaims) {
      if (decoded[claimName]) {
        role = decoded[claimName];
        break;
      }
    }
    
    if (!role) {
      return [];
    }
    
    const roles = Array.isArray(role) ? role : [role];
    
    return roles;
  }

  hasRole(role: string): boolean {
    return this.getRoles().includes(role);
  }
} 