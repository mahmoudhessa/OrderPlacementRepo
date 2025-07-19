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

  getProducts(): Observable<Product[]> {
    return this.api.get<Product[]>('/api/products');
  }

  addProduct(product: Partial<Product>): Observable<Product> {
    return this.api.post<Product>('/api/products', product);
  }
} 