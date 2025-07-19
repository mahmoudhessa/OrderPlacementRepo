import { Component, ViewChild } from '@angular/core';
import { RoleService } from './core/role.service';
import { AuthService } from './auth/auth.service';
import { MatSidenav } from '@angular/material/sidenav';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'frontend';
  @ViewChild('sidenav') sidenav!: MatSidenav;

  constructor(public roleService: RoleService, public authService: AuthService) {}

  get userEmail(): string | null {
    // For now, return a placeholder since we don't store user email in localStorage
    // In a real app, you'd decode the JWT token or store user info separately
    return this.authService.isLoggedIn() ? 'user@example.com' : null;
  }

  hasRole(role: string): boolean {
    return this.roleService.hasRole(role);
  }

  logout() {
    this.authService.logout();
    window.location.reload();
  }
}
