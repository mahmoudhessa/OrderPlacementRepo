import { Component } from '@angular/core';
import { RoleService } from './core/role.service';
import { AuthService } from './auth/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'frontend';

  constructor(public roleService: RoleService, public authService: AuthService) {}

  hasRole(role: string): boolean {
    return this.roleService.hasRole(role);
  }

  logout() {
    this.authService.logout();
    window.location.reload();
  }
}
