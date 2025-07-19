import { Component } from '@angular/core';

@Component({
  selector: 'app-greeting',
  template: `
    <div class="greeting-container">
      <mat-card class="greeting-card">
        <div class="welcome-content">
          <h1 class="welcome-title">Welcome to Talabeyah Order Management System</h1>
          <p class="welcome-subtitle">Your comprehensive solution for order management and inventory control</p>
          <div class="welcome-features">
            <div class="feature">
              <mat-icon>shopping_cart</mat-icon>
              <span>Order Management</span>
            </div>
            <div class="feature">
              <mat-icon>inventory</mat-icon>
              <span>Inventory Control</span>
            </div>
            <div class="feature">
              <mat-icon>analytics</mat-icon>
              <span>Real-time Analytics</span>
            </div>
            <div class="feature">
              <mat-icon>security</mat-icon>
              <span>Secure Access</span>
            </div>
          </div>
        </div>
      </mat-card>
    </div>
  `,
  styles: [`
    .greeting-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 80vh;
      padding: 20px;
    }
    
    .greeting-card {
      max-width: 800px;
      width: 100%;
      text-align: center;
      padding: 40px;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
    }
    
    .welcome-content {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 20px;
    }
    
    .welcome-title {
      font-size: 2.5rem;
      font-weight: 300;
      margin: 0;
      text-shadow: 2px 2px 4px rgba(0,0,0,0.3);
    }
    
    .welcome-subtitle {
      font-size: 1.2rem;
      margin: 0;
      opacity: 0.9;
    }
    
    .welcome-features {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 20px;
      margin-top: 30px;
      width: 100%;
    }
    
    .feature {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 10px;
      padding: 20px;
      background: rgba(255,255,255,0.1);
      border-radius: 10px;
      backdrop-filter: blur(10px);
    }
    
    .feature mat-icon {
      font-size: 2rem;
      width: 2rem;
      height: 2rem;
    }
    
    .feature span {
      font-size: 1rem;
      font-weight: 500;
    }
    
    @media (max-width: 768px) {
      .welcome-title {
        font-size: 2rem;
      }
      
      .welcome-features {
        grid-template-columns: repeat(2, 1fr);
      }
    }
  `]
})
export class GreetingComponent {
  constructor() {}
} 