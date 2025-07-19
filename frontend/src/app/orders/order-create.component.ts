import { Component, OnInit } from '@angular/core';
import { ProductService, Product } from '../products/product.service';
import { OrderService, OrderProduct } from './order.service';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-order-create',
  template: `
    <div class="content">
      <mat-card class="highlight-card" style="width:100%;max-width:600px;">
        <h2>Create Order</h2>
        <div *ngIf="error" style="color:#d32f2f;font-weight:bold;text-align:center;margin-bottom:12px;">{{ error }}</div>
        <form (ngSubmit)="submit()" style="display:flex;flex-direction:column;gap:16px;">
          <div *ngFor="let p of products" class="card card-small" style="display:flex;align-items:center;justify-content:space-between;">
            <label style="flex:1;">
              {{ p.name }} (Available: {{ p.inventory }})
            </label>
            <mat-form-field style="width:80px;">
              <input matInput type="number" min="0" [(ngModel)]="quantities[p.id]" name="qty{{p.id}}" />
            </mat-form-field>
          </div>
          <button mat-raised-button color="primary" type="submit">Place Order</button>
          <div *ngIf="message" style="color:#1976d2;font-weight:bold;">{{ message }}</div>
        </form>
      </mat-card>
    </div>
  `
})
export class OrderCreateComponent implements OnInit {
  products: Product[] = [];
  quantities: { [id: number]: number } = {};
  message = '';
  error: string = '';

  constructor(
    private productService: ProductService,
    private orderService: OrderService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.productService.getProducts().subscribe({
      next: products => {
        this.products = products;
        this.quantities = {};
        products.forEach(p => this.quantities[p.id] = 0);
      },
      error: err => {
        if (err && err.backendDown) {
          this.error = 'Backend is down. Please try again later.';
        } else {
          this.error = 'Failed to load products.';
        }
      }
    });
  }

  submit() {
    const buyerId = this.authService.getUserId() || '';
    if (!buyerId) {
      this.error = 'User ID not found. Please log in again.';
      return;
    }
    const products = Object.entries(this.quantities)
      .filter(([_, qty]) => qty > 0)
      .map(([id, qty]) => ({ productId: +id, quantity: qty }));
    // Generate a unique Idempotency-Key (UUID v4)
    const idempotencyKey = self.crypto?.randomUUID ? self.crypto.randomUUID() : Math.random().toString(36).substring(2) + Date.now();
    this.orderService.placeOrder(buyerId, products, idempotencyKey).subscribe({
      next: res => {
        this.message = 'Order placed successfully!';
        this.error = '';
      },
      error: err => {
        this.error = err?.error || 'Failed to place order.';
        this.message = '';
      }
    });
  }
} 