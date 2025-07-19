import { Component, OnInit } from '@angular/core';
import { AuditLogService, AuditLog } from './audit-log.service';

@Component({
  selector: 'app-audit-log-list',
  template: `
    <div class="content">
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
  constructor(private auditLogService: AuditLogService) {}
  ngOnInit() {
    this.auditLogService.getAuditLogs().subscribe(logs => this.logs = logs);
  }
} 