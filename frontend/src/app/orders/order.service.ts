import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface OrderProduct {
  productId: number;
  quantity: number;
}

@Injectable({ providedIn: 'root' })
export class OrderService {
  private apiUrl = 'http://localhost:5000/api/orders';

  constructor(private http: HttpClient) {}

  placeOrder(products: OrderProduct[]): Observable<{ orderId: number }> {
    return this.http.post<{ orderId: number }>(this.apiUrl, { products });
  }

  getOrders(page = 1, pageSize = 10): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}?page=${page}&pageSize=${pageSize}`);
  }
} 