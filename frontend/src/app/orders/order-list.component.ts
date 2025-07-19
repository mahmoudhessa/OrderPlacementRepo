import { Component, OnInit } from '@angular/core';
import { OrderService } from './order.service';
import { OrderRealtimeService } from './order-realtime.service';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-order-list',
  template: `
    <div class="content">
      <mat-card class="highlight-card" style="width:100%;max-width:900px;">
        <h2>Orders</h2>
        <div *ngIf="notification" class="notification">{{ notification }}</div>
        <div *ngIf="error" style="color:#d32f2f;font-weight:bold;text-align:center;margin-bottom:12px;">{{ error }}</div>
        
        <div *ngIf="orders.length === 0 && !error" style="text-align:center;padding:20px;color:#666;">
          No orders found. Create your first order!
        </div>
        
        <table *ngIf="orders.length > 0" mat-table [dataSource]="orders" class="mat-elevation-z1" style="width:100%;margin-bottom:16px;">
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
          <!-- Debug row: show full order object as JSON -->
          <tr *ngFor="let o of orders">
            <td colspan="3">{{ o | json }}</td>
          </tr>
        </table>
        
        <div *ngIf="orders.length > 0" style="display:flex;gap:8px;justify-content:flex-end;">
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
    .error {
      background: #ffe0e0;
      color: #d32f2f;
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
  error: string = '';

  constructor(
    private orderService: OrderService,
    private authService: AuthService,
    private orderRealtime: OrderRealtimeService
  ) {}

  ngOnInit() {
    console.log('OrderListComponent ngOnInit - starting to fetch orders');
    this.fetchOrders();
    this.orderRealtime.orderCreated$.subscribe(order => {
      this.notification = `New order created! ID: ${order.orderId}`;
      this.load();
      setTimeout(() => this.notification = '', 3000);
    });
  }

  load() {
    this.error = '';
    const buyerId = this.authService.getUserId() || '';
    if (!buyerId) {
      this.error = 'User ID not found. Please log in again.';
      return;
    }
    this.orderService.getOrders(buyerId, this.page, this.pageSize).subscribe({
      next: res => {
        if (Array.isArray(res)) {
          this.orders = res;
          this.total = res.length;
        } else if (res && Array.isArray(res.orders)) {
          this.orders = res.orders;
          this.total = res.totalCount || res.orders.length;
        } else if (res && Array.isArray(res.items)) {
          this.orders = res.items;
          this.total = res.total || res.items.length;
        } else {
          this.orders = res || [];
          this.total = 0;
        }
      },
      error: err => {
        if (err && err.backendDown) {
          this.error = 'Backend is down. Please try again later.';
        } else {
          this.error = 'Failed to load orders.';
        }
      }
    });
  }

  fetchOrders() {
    const isAdmin = this.authService.hasRole('Admin');
    let buyerId = '';
    if (!isAdmin) {
      buyerId = this.authService.getUserId() || '';
      if (!buyerId) {
        this.error = 'User ID not found. Please log in again.';
        console.log('OrderListComponent - No buyerId found');
        return;
      }
    }
    this.orderService.getOrders(isAdmin ? '' : buyerId, this.page, this.pageSize).subscribe({
      next: res => {
        console.log('OrderListComponent - Orders loaded successfully:', res);
        if (Array.isArray(res)) {
          this.orders = res;
          this.total = res.length;
        } else if (res && Array.isArray(res.orders)) {
          this.orders = res.orders;
          this.total = res.totalCount || res.orders.length;
        } else if (res && Array.isArray(res.items)) {
          this.orders = res.items;
          this.total = res.total || res.items.length;
        } else if (res && Array.isArray(res.data)) {
          this.orders = res.data;
          this.total = res.total || res.data.length;
        } else {
          this.orders = [];
          this.total = 0;
        }
        this.error = '';
      },
      error: err => {
        console.error('OrderListComponent - Error loading orders:', err);
        this.error = err?.error || 'Failed to load orders.';
      }
    });
  }

  prev() {
    if (this.page > 1) {
      this.page--;
      this.fetchOrders();
    }
  }

  next() {
    this.page++;
    this.fetchOrders();
  }
} 