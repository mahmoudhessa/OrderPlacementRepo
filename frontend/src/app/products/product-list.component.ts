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
  page = 1;
  pageSize = 10;
  total = 0;
  searchName = '';

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
    this.productService.getProducts(this.searchName, this.page, this.pageSize).subscribe({
      next: (products: Product[]) => {
        console.log('Products loaded:', products);
        this.products = products;
        // For now, we'll assume the total is the current page size if we have products
        // In a real implementation, the API should return total count in headers or response
        this.total = products.length >= this.pageSize ? this.page * this.pageSize + 10 : this.page * this.pageSize;
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

  prev() {
    if (this.page > 1) {
      this.page--;
      this.loadProducts();
    }
  }

  next() {
    this.page++;
    this.loadProducts();
  }

  onSearch() {
    this.page = 1; // Reset to first page when searching
    this.loadProducts();
  }

  clearSearch() {
    this.searchName = '';
    this.page = 1;
    this.loadProducts();
  }
} 