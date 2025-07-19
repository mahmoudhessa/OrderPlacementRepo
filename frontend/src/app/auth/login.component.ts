import { Component } from '@angular/core';
import { AuthService } from './auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  template: `
    <div class="content" style="justify-content:center;align-items:center;min-height:80vh;">
      <mat-card style="min-width:320px;max-width:400px;width:100%;margin-top:60px;padding:32px 24px;">
        <h2 style="margin-bottom:24px;text-align:center;">Login</h2>
        <form (ngSubmit)="login()" style="display:flex;flex-direction:column;gap:20px;">
          <mat-form-field appearance="outline">
            <mat-label>Email</mat-label>
            <input matInput [(ngModel)]="email" name="email" required type="email" autocomplete="username" />
          </mat-form-field>
          <div style="font-size:12px;color:#888;margin-bottom:8px;">
            <span>Test emails:</span>
            <span *ngFor="let e of dummyEmails" style="display:inline-block;margin-right:8px;">
              <span style="font-family:monospace;">{{ e.email }}</span> ({{ e.role }})
            </span>
          </div>
          <mat-form-field appearance="outline">
            <mat-label>Password</mat-label>
            <input matInput [type]="hide ? 'password' : 'text'" [(ngModel)]="password" name="password" required autocomplete="current-password" />
            <button mat-icon-button matSuffix (click)="hide = !hide" [attr.aria-label]="hide ? 'Show password' : 'Hide password'" type="button">
              <mat-icon>{{hide ? 'visibility_off' : 'visibility'}}</mat-icon>
            </button>
          </mat-form-field>
          <button mat-raised-button color="primary" type="submit" style="font-weight:600;">Login</button>
          <div *ngIf="error" style="color:#d32f2f;font-weight:bold;text-align:center;">{{ error }}</div>
        </form>
      </mat-card>
    </div>
  `
})
export class LoginComponent {
  email = '';
  password = '';
  hide = true;
  error = '';
  dummyEmails = [
    { email: 'admin@demo.com', role: 'Admin', password: 'admin123' },
    { email: 'sales@demo.com', role: 'Sales', password: 'sales123' },
    { email: 'auditor@demo.com', role: 'Auditor', password: 'auditor123' },
    { email: 'inventory@demo.com', role: 'InventoryManager', password: 'inventory123' }
  ];

  constructor(private auth: AuthService, private router: Router) {}

  login() {
    // Auto-fill password if a dummy email is selected
    const selected = this.dummyEmails.find(e => e.email === this.email);
    if (selected && this.password === '') {
      this.password = selected.password;
    }
    this.error = '';
    this.auth.login(this.email, this.password).subscribe({
      next: () => this.router.navigate(['/']),
      error: err => {
        if (err && err.backendDown) {
          this.error = 'Backend is down. Please try again later.';
        } else if (err && err.status === 401) {
          this.error = 'Invalid credentials';
        } else {
          this.error = 'An unexpected error occurred.';
        }
      }
    });
  }
} 