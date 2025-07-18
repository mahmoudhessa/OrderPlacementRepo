import { Component, OnInit } from '@angular/core';
import { OrderService } from './order.service';
import { OrderRealtimeService } from './order-realtime.service';

@Component({
  selector: 'app-order-list',
  template: `
    <h2>Orders</h2>
    <div *ngIf="notification" class="notification">{{ notification }}</div>
    <table>
      <tr>
        <th>ID</th><th>Status</th><th>Created At</th>
      </tr>
      <tr *ngFor="let o of orders">
        <td>{{ o.id }}</td>
        <td>{{ o.status }}</td>
        <td>{{ o.createdAt | date:'short' }}</td>
      </tr>
    </table>
    <button (click)="prev()" [disabled]="page === 1">Prev</button>
    <button (click)="next()" [disabled]="page * pageSize >= total">Next</button>
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