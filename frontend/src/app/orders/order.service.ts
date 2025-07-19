import { Injectable } from '@angular/core';
import { ApiBaseService } from '../core/api-base.service';
import { Observable } from 'rxjs';

export interface OrderProduct {
  productId: number;
  quantity: number;
}

@Injectable({ providedIn: 'root' })
export class OrderService {
  constructor(private api: ApiBaseService) {}

  placeOrder(products: OrderProduct[]): Observable<{ orderId: number }> {
    return this.api.post<{ orderId: number }>('/api/orders', { products });
  }

  getOrders(page = 1, pageSize = 10): Observable<any> {
    return this.api.get<any>(`/api/orders?page=${page}&pageSize=${pageSize}`);
  }
} 