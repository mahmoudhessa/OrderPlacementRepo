import { Component, OnInit } from '@angular/core';
import { ProductService, Product } from './product.service';

@Component({
  selector: 'app-product-list',
  template: `
    <h2>Products</h2>
    <ul>
      <li *ngFor="let p of products">
        {{ p.name }} (Inventory: {{ p.inventory }})
      </li>
    </ul>
  `
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  constructor(private productService: ProductService) {}
  ngOnInit() {
    this.productService.getProducts().subscribe(products => this.products = products);
  }
} 