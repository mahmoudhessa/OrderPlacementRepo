import { Injectable } from '@angular/core';
import { ApiBaseService } from '../core/api-base.service';
import { Observable } from 'rxjs';

export interface Product {
  id: number;
  name: string;
  inventory: number;
}

@Injectable({ providedIn: 'root' })
export class ProductService {
  constructor(private api: ApiBaseService) {}

  getProducts(name?: string, page: number = 1, pageSize: number = 10): Observable<Product[]> {
    let url = `/api/products?page=${page}&pageSize=${pageSize}`;
    if (name) {
      url += `&name=${encodeURIComponent(name)}`;
    }
    return this.api.get<Product[]>(url);
  }

  addProduct(product: Partial<Product>): Observable<Product> {
    return this.api.post<Product>('/api/products', product);
  }
} 