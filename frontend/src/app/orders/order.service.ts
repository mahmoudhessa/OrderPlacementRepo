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

  placeOrder(buyerId: string, products: OrderProduct[], idempotencyKey?: string): Observable<{ orderId: number }> {
    const headers: any = {};
    if (idempotencyKey) {
      headers['Idempotency-Key'] = idempotencyKey;
    }
    return this.api.post<{ orderId: number }>('/api/orders', { buyerId, products }, { headers });
  }

  getOrders(buyerId: string, page: number = 1, pageSize: number = 10): Observable<any> {
    let url = `/api/orders?page=${page}&pageSize=${pageSize}`;
    if (buyerId) {
      url = `/api/orders?buyerId=${buyerId}&page=${page}&pageSize=${pageSize}`;
    }
    return this.api.get<any>(url);
  }
} 