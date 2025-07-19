import { Component, OnInit } from '@angular/core';
import { ProductService, Product } from './product.service';

@Component({
  selector: 'app-product-list',
  template: `
    <div class="content">
      <mat-card class="highlight-card" style="width:100%;max-width:600px;">
        <h2>Products</h2>
        <mat-list>
          <mat-list-item *ngFor="let p of products">
            <span>{{ p.name }}</span>
            <span style="color:#888;margin-left:auto;">(Inventory: {{ p.inventory }})</span>
          </mat-list-item>
        </mat-list>
      </mat-card>
    </div>
  `
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  constructor(private productService: ProductService) {}
  ngOnInit() {
    this.productService.getProducts().subscribe(products => this.products = products);
  }
} 