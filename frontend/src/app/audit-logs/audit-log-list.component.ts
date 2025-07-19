import { Component, OnInit } from '@angular/core';
import { AuditLogService, AuditLog } from './audit-log.service';

@Component({
  selector: 'app-audit-log-list',
  template: `
    <div class="content">
      <div *ngIf="error" style="color:#d32f2f;font-weight:bold;text-align:center;margin-bottom:12px;">{{ error }}</div>
      <mat-card class="highlight-card" style="width:100%;max-width:600px;">
        <h2>Audit Logs</h2>
        <mat-list>
          <mat-list-item *ngFor="let log of logs">
            <span>{{ log.change }}</span>
            <span style="color:#888;margin-left:auto;">({{ log.createdAt | date:'short' }})</span>
          </mat-list-item>
        </mat-list>
      </mat-card>
    </div>
  `
})
export class AuditLogListComponent implements OnInit {
  logs: AuditLog[] = [];
  error: string = '';
  constructor(private auditLogService: AuditLogService) {}
  ngOnInit() {
    this.auditLogService.getAuditLogs().subscribe({
      next: logs => this.logs = logs,
      error: err => {
        if (err && err.backendDown) {
          this.error = 'Backend is down. Please try again later.';
        } else {
          this.error = 'Failed to load audit logs.';
        }
      }
    });
  }
} 