import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './auth/login.component';
import { ProductListComponent } from './products/product-list.component';
import { OrderCreateComponent } from './orders/order-create.component';
import { OrderListComponent } from './orders/order-list.component';
import { AuditLogListComponent } from './audit-logs/audit-log-list.component';
import { GreetingComponent } from './greeting/greeting.component';
import { AuthGuard } from './auth/auth.guard';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'greeting', component: GreetingComponent, canActivate: [AuthGuard] },
  { path: 'products', component: ProductListComponent, canActivate: [AuthGuard] },
  { path: 'orders/create', component: OrderCreateComponent, canActivate: [AuthGuard] },
  { path: 'orders', component: OrderListComponent, canActivate: [AuthGuard] },
  { path: 'audit-logs', component: AuditLogListComponent, canActivate: [AuthGuard] },
  { path: '', redirectTo: '/greeting', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
