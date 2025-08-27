import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';

// PrimeNG Imports
import { ToolbarModule } from 'primeng/toolbar';
import { ButtonModule } from 'primeng/button';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ToolbarModule,
    ButtonModule
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'מערכת פניות ציבור - הנהלת בתי המשפט';
  
  menuItems: MenuItem[] = [
    {
      label: 'פנייה חדשה',
      icon: 'pi pi-plus',
      routerLink: '/form'
    },
    {
      label: 'דוחות',
      icon: 'pi pi-chart-bar',
      routerLink: '/reports'
    }
  ];

  constructor(private router: Router) {}

  getCurrentYear(): number {
    return new Date().getFullYear();
  }
}