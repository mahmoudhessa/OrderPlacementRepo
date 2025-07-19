import { Component, OnInit, OnDestroy } from '@angular/core';
import { OrderRealtimeService, 
         OrderCreatedEvent, 
         OrderStatusChangedEvent, 
         InventoryChangedEvent, 
         LowInventoryEvent, 
         OrderCancelledEvent, 
         SystemAlertEvent, 
         ConcurrencyConflictEvent } from './order-realtime.service';
import { Subscription } from 'rxjs';

interface NotificationItem {
  id: string;
  type: 'success' | 'warning' | 'error' | 'info';
  title: string;
  message: string;
  timestamp: Date;
  icon: string;
}

@Component({
  selector: 'app-order-notifications',
  template: `
    <div class="notifications-container">
      <div class="notifications-header">
        <h3>Real-Time Notifications</h3>
        <div class="connection-status" [class.connected]="isConnected" [class.disconnected]="!isConnected">
          <i class="fas fa-circle"></i>
          {{ isConnected ? 'Connected' : 'Disconnected' }}
        </div>
        <button class="clear-btn" (click)="clearNotifications()" [disabled]="notifications.length === 0">
          <i class="fas fa-trash"></i> Clear All
        </button>
      </div>
      
      <div class="notifications-list" *ngIf="notifications.length > 0; else noNotifications">
        <div *ngFor="let notification of notifications" 
             class="notification-item" 
             [class]="notification.type"
             [@notificationAnimation]>
          <div class="notification-icon">
            <i [class]="notification.icon"></i>
          </div>
          <div class="notification-content">
            <div class="notification-title">{{ notification.title }}</div>
            <div class="notification-message">{{ notification.message }}</div>
            <div class="notification-time">{{ notification.timestamp | date:'short' }}</div>
          </div>
          <button class="close-btn" (click)="removeNotification(notification.id)">
            <i class="fas fa-times"></i>
          </button>
        </div>
      </div>
      
      <ng-template #noNotifications>
        <div class="no-notifications">
          <i class="fas fa-bell-slash"></i>
          <p>No notifications yet</p>
          <small>Real-time updates will appear here</small>
        </div>
      </ng-template>
    </div>
  `,
  styles: [`
    .notifications-container {
      max-width: 400px;
      background: white;
      border-radius: 8px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.1);
      overflow: hidden;
    }

    .notifications-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 16px;
      background: #f8f9fa;
      border-bottom: 1px solid #e9ecef;
    }

    .notifications-header h3 {
      margin: 0;
      font-size: 16px;
      font-weight: 600;
    }

    .connection-status {
      display: flex;
      align-items: center;
      gap: 6px;
      font-size: 12px;
      font-weight: 500;
    }

    .connection-status.connected {
      color: #28a745;
    }

    .connection-status.disconnected {
      color: #dc3545;
    }

    .connection-status i {
      font-size: 8px;
    }

    .clear-btn {
      background: none;
      border: none;
      color: #6c757d;
      cursor: pointer;
      padding: 4px 8px;
      border-radius: 4px;
      font-size: 12px;
    }

    .clear-btn:hover:not(:disabled) {
      background: #e9ecef;
    }

    .clear-btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .notifications-list {
      max-height: 400px;
      overflow-y: auto;
    }

    .notification-item {
      display: flex;
      align-items: flex-start;
      padding: 12px 16px;
      border-bottom: 1px solid #f1f3f4;
      transition: background-color 0.2s;
      animation: slideIn 0.3s ease-out;
    }

    .notification-item:hover {
      background: #f8f9fa;
    }

    .notification-item.success {
      border-left: 4px solid #28a745;
    }

    .notification-item.warning {
      border-left: 4px solid #ffc107;
    }

    .notification-item.error {
      border-left: 4px solid #dc3545;
    }

    .notification-item.info {
      border-left: 4px solid #17a2b8;
    }

    .notification-icon {
      margin-right: 12px;
      margin-top: 2px;
    }

    .notification-icon i {
      font-size: 16px;
    }

    .notification-item.success .notification-icon i {
      color: #28a745;
    }

    .notification-item.warning .notification-icon i {
      color: #ffc107;
    }

    .notification-item.error .notification-icon i {
      color: #dc3545;
    }

    .notification-item.info .notification-icon i {
      color: #17a2b8;
    }

    .notification-content {
      flex: 1;
      min-width: 0;
    }

    .notification-title {
      font-weight: 600;
      font-size: 14px;
      margin-bottom: 4px;
      color: #212529;
    }

    .notification-message {
      font-size: 12px;
      color: #6c757d;
      margin-bottom: 4px;
      line-height: 1.4;
    }

    .notification-time {
      font-size: 11px;
      color: #adb5bd;
    }

    .close-btn {
      background: none;
      border: none;
      color: #adb5bd;
      cursor: pointer;
      padding: 4px;
      border-radius: 4px;
      margin-left: 8px;
    }

    .close-btn:hover {
      background: #e9ecef;
      color: #6c757d;
    }

    .no-notifications {
      text-align: center;
      padding: 40px 20px;
      color: #6c757d;
    }

    .no-notifications i {
      font-size: 48px;
      margin-bottom: 16px;
      opacity: 0.5;
    }

    .no-notifications p {
      margin: 0 0 8px 0;
      font-weight: 500;
    }

    .no-notifications small {
      font-size: 12px;
      opacity: 0.7;
    }

    @keyframes slideIn {
      from {
        opacity: 0;
        transform: translateX(-20px);
      }
      to {
        opacity: 1;
        transform: translateX(0);
      }
    }
  `],
  animations: [
    // Add animations here if needed
  ]
})
export class OrderNotificationsComponent implements OnInit, OnDestroy {
  notifications: NotificationItem[] = [];
  isConnected = false;
  private subscriptions: Subscription[] = [];

  constructor(private orderRealtimeService: OrderRealtimeService) {}

  ngOnInit(): void {
    this.setupSubscriptions();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  private setupSubscriptions(): void {
    // Connection status
    this.subscriptions.push(
      this.orderRealtimeService.connectionStatus$.subscribe(
        connected => this.isConnected = connected
      )
    );

    // Order events
    this.subscriptions.push(
      this.orderRealtimeService.orderCreated$.subscribe(
        event => this.addNotification({
          type: 'success',
          title: 'New Order Created',
          message: event.message,
          icon: 'fas fa-shopping-cart'
        })
      )
    );

    this.subscriptions.push(
      this.orderRealtimeService.orderStatusChanged$.subscribe(
        event => this.addNotification({
          type: 'info',
          title: 'Order Status Updated',
          message: event.message,
          icon: 'fas fa-sync-alt'
        })
      )
    );

    this.subscriptions.push(
      this.orderRealtimeService.orderCancelled$.subscribe(
        event => this.addNotification({
          type: 'warning',
          title: 'Order Cancelled',
          message: event.message,
          icon: 'fas fa-ban'
        })
      )
    );

    // Inventory events
    this.subscriptions.push(
      this.orderRealtimeService.inventoryChanged$.subscribe(
        event => this.addNotification({
          type: 'info',
          title: 'Inventory Updated',
          message: event.message,
          icon: 'fas fa-boxes'
        })
      )
    );

    this.subscriptions.push(
      this.orderRealtimeService.lowInventory$.subscribe(
        event => this.addNotification({
          type: 'warning',
          title: 'Low Inventory Alert',
          message: event.message,
          icon: 'fas fa-exclamation-triangle'
        })
      )
    );

    // System events
    this.subscriptions.push(
      this.orderRealtimeService.systemAlert$.subscribe(
        event => this.addNotification({
          type: event.severity === 'Error' ? 'error' : 'info',
          title: 'System Alert',
          message: event.message,
          icon: 'fas fa-bell'
        })
      )
    );

    this.subscriptions.push(
      this.orderRealtimeService.concurrencyConflict$.subscribe(
        event => this.addNotification({
          type: 'error',
          title: 'Concurrency Conflict',
          message: event.message,
          icon: 'fas fa-exclamation-circle'
        })
      )
    );
  }

  private addNotification(notification: Omit<NotificationItem, 'id' | 'timestamp'>): void {
    const newNotification: NotificationItem = {
      ...notification,
      id: this.generateId(),
      timestamp: new Date()
    };

    this.notifications.unshift(newNotification);

    // Keep only last 50 notifications
    if (this.notifications.length > 50) {
      this.notifications = this.notifications.slice(0, 50);
    }

    // Auto-remove notifications after 30 seconds
    setTimeout(() => {
      this.removeNotification(newNotification.id);
    }, 30000);
  }

  private generateId(): string {
    return Math.random().toString(36).substr(2, 9);
  }

  removeNotification(id: string): void {
    this.notifications = this.notifications.filter(n => n.id !== id);
  }

  clearNotifications(): void {
    this.notifications = [];
  }
} 