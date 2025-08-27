import { Routes } from '@angular/router';

export const routes: Routes = [
  { 
    path: '', 
    redirectTo: '/form', 
    pathMatch: 'full' 
  },
  { 
    path: 'form', 
    loadComponent: () => import('./components/inquiry-form/inquiry-form.component').then(c => c.InquiryFormComponent)
  },
  { 
    path: 'reports', 
    loadComponent: () => import('./components/reports/reports.component').then(c => c.ReportsComponent)
  },
  { 
    path: '**', 
    redirectTo: '/form' 
  }
];