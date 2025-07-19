import { Component, OnInit } from '@angular/core';
import { OrderService } from './order.service';
import { OrderRealtimeService } from './order-realtime.service';

@Component({
  selector: 'app-order-list',
  template: `
    <div class="content">
      <mat-card class="highlight-card" style="width:100%;max-width:900px;">
        <h2>Orders</h2>
        <div *ngIf="notification" class="notification">{{ notification }}</div>
        <table mat-table [dataSource]="orders" class="mat-elevation-z1" style="width:100%;margin-bottom:16px;">
          <ng-container matColumnDef="id">
            <th mat-header-cell *matHeaderCellDef>ID</th>
            <td mat-cell *matCellDef="let o">{{ o.id }}</td>
          </ng-container>
          <ng-container matColumnDef="status">
            <th mat-header-cell *matHeaderCellDef>Status</th>
            <td mat-cell *matCellDef="let o">{{ o.status }}</td>
          </ng-container>
          <ng-container matColumnDef="createdAt">
            <th mat-header-cell *matHeaderCellDef>Created At</th>
            <td mat-cell *matCellDef="let o">{{ o.createdAt | date:'short' }}</td>
          </ng-container>
          <tr mat-header-row *matHeaderRowDef="['id', 'status', 'createdAt']"></tr>
          <tr mat-row *matRowDef="let row; columns: ['id', 'status', 'createdAt'];"></tr>
        </table>
        <div style="display:flex;gap:8px;justify-content:flex-end;">
          <button mat-stroked-button color="primary" (click)="prev()" [disabled]="page === 1">Prev</button>
          <button mat-stroked-button color="primary" (click)="next()" [disabled]="page * pageSize >= total">Next</button>
        </div>
      </mat-card>
    </div>
  `,
  styles: [`
    .notification {
      background: #e0ffe0;
      color: #1976d2;
      padding: 0.5rem 1rem;
      margin-bottom: 1rem;
      border-radius: 4px;
      font-weight: bold;
    }
  `]
})
export class OrderListComponent implements OnInit {
  orders: any[] = [];
  page = 1;
  pageSize = 10;
  total = 0;
  notification = '';

  constructor(
    private orderService: OrderService,
    private orderRealtime: OrderRealtimeService
  ) {}

  ngOnInit() {
    this.load();
    this.orderRealtime.orderCreated$.subscribe(order => {
      this.notification = `New order created! ID: ${order.orderId}`;
      this.load();
      setTimeout(() => this.notification = '', 3000);
    });
  }

  load() {
    this.orderService.getOrders(this.page, this.pageSize).subscribe(res => {
      this.orders = res.orders;
      this.total = res.totalCount;
    });
  }

  prev() { if (this.page > 1) { this.page--; this.load(); } }
  next() { if (this.page * this.pageSize < this.total) { this.page++; this.load(); } }
} 