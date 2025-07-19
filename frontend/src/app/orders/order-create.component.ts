import { Component, OnInit } from '@angular/core';
import { ProductService, Product } from '../products/product.service';
import { OrderService, OrderProduct } from './order.service';

@Component({
  selector: 'app-order-create',
  template: `
    <div class="content">
      <mat-card class="highlight-card" style="width:100%;max-width:600px;">
        <h2>Create Order</h2>
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

  constructor(private productService: ProductService, private orderService: OrderService) {}

  ngOnInit() {
    this.productService.getProducts().subscribe(products => {
      this.products = products;
      this.quantities = {};
      products.forEach(p => this.quantities[p.id] = 0);
    });
  }

  submit() {
    const orderProducts: OrderProduct[] = Object.entries(this.quantities)
      .filter(([_, qty]) => qty > 0)
      .map(([productId, quantity]) => ({ productId: +productId, quantity: +quantity }));

    this.orderService.placeOrder(orderProducts).subscribe({
      next: res => this.message = `Order placed! ID: ${res.orderId}`,
      error: err => this.message = err.error?.[0] || 'Order failed'
    });
  }
} 