import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class OrderRealtimeService {
  private hubConnection: signalR.HubConnection;
  private orderCreatedSource = new Subject<any>();
  orderCreated$ = this.orderCreatedSource.asObservable();

  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/orderHub')
      .build();

    this.hubConnection.on('OrderCreated', (order) => {
      this.orderCreatedSource.next(order);
    });

    this.hubConnection.start();
  }
} 