import { Component, OnInit } from '@angular/core';
import { AuditLogService, AuditLog } from './audit-log.service';

@Component({
  selector: 'app-audit-log-list',
  template: `
    <h2>Audit Logs</h2>
    <ul>
      <li *ngFor="let log of logs">
        {{ log.change }} ({{ log.createdAt | date:'short' }})
      </li>
    </ul>
  `
})
export class AuditLogListComponent implements OnInit {
  logs: AuditLog[] = [];
  constructor(private auditLogService: AuditLogService) {}
  ngOnInit() {
    this.auditLogService.getAuditLogs().subscribe(logs => this.logs = logs);
  }
} 