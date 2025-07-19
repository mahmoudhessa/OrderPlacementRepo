import { Component, OnInit } from '@angular/core';
import { ProductService, Product } from './product.service';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../auth/auth.service';

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
  error: string = '';
  addProductForm: FormGroup;
  showAddProduct = false;

  constructor(
    private productService: ProductService,
    private dialog: MatDialog,
    private fb: FormBuilder,
    public authService: AuthService
  ) {
    this.addProductForm = this.fb.group({
      name: ['', Validators.required],
      inventory: [0, [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit() {
    this.loadProducts();
    this.showAddProduct = this.authService.hasRole('Admin') || this.authService.hasRole('InventoryManager');
  }

  loadProducts() {
    this.error = '';
    this.productService.getProducts().subscribe({
      next: products => this.products = products,
      error: err => {
        if (err && err.backendDown) {
          this.error = 'Backend is down. Please try again later.';
        } else {
          this.error = 'Failed to load products.';
        }
      }
    });
  }

  onAddProduct() {
    if (this.addProductForm.valid) {
      this.productService.addProduct(this.addProductForm.value).subscribe({
        next: () => {
          this.loadProducts();
          this.addProductForm.reset({ name: '', inventory: 0 });
        },
        error: err => alert('Failed to add product: ' + err.error?.message || err.message)
      });
    }
  }
} 