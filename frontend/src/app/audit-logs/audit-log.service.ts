import { Injectable } from '@angular/core';
import { ApiBaseService } from '../core/api-base.service';
import { Observable } from 'rxjs';

export interface AuditLog {
  id: number;
  change: string;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class AuditLogService {
  constructor(private api: ApiBaseService) {}

  getAuditLogs(): Observable<AuditLog[]> {
    return this.api.get<AuditLog[]>('/api/auditlogs');
  }
} 