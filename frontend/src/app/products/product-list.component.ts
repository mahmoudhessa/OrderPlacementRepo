import { Component, OnInit } from '@angular/core';
import { ProductService, Product } from './product.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  error: string = '';
  addProductForm: FormGroup;
  showAddProduct = false;
  displayedColumns: string[] = ['id', 'name', 'inventory'];

  constructor(
    private productService: ProductService,
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
      next: (products: Product[]) => {
        console.log('Products loaded:', products);
        this.products = products;
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