import { Component, OnInit } from '@angular/core';
import { ProductService, Product } from '../products/product.service';
import { OrderService, OrderProduct } from './order.service';

@Component({
  selector: 'app-order-create',
  template: `
    <h2>Create Order</h2>
    <form (ngSubmit)="submit()">
      <div *ngFor="let p of products">
        <label>
          {{ p.name }} (Available: {{ p.inventory }})
          <input type="number" min="0" [(ngModel)]="quantities[p.id]" name="qty{{p.id}}" />
        </label>
      </div>
      <button type="submit">Place Order</button>
      <div *ngIf="message">{{ message }}</div>
    </form>
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