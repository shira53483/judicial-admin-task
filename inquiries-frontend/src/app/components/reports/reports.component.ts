import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { MonthlyReport } from '../../models/inquiry.model';
import { MessageService } from 'primeng/api';
import { Chart, ChartConfiguration, ChartData, ChartType, registerables } from 'chart.js';

// PrimeNG Imports
import { CardModule } from 'primeng/card';
import { DropdownModule } from 'primeng/dropdown';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ChartModule } from 'primeng/chart';
import { ToastModule } from 'primeng/toast';

Chart.register(...registerables);

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    CardModule,
    DropdownModule,
    TableModule,
    TagModule,
    ProgressSpinnerModule,
    ChartModule,
    ToastModule
  ],
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.css']
})
export class ReportsComponent implements OnInit {
  reports: MonthlyReport[] = [];
  selectedYear = new Date().getFullYear();
  selectedMonth = new Date().getMonth() + 1;
  isLoading = false;

  // Chart configuration
  chartData: ChartData<'bar'> = {
    labels: [],
    datasets: []
  };

  chartOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: {
      legend: {
        position: 'top',
      },
      title: {
        display: true,
        text: 'השוואת פניות לפי מחלקות'
      }
    },
    scales: {
      y: {
        beginAtZero: true
      }
    }
  };

  years: any[] = [];
  months: any[] = [
    { label: 'ינואר', value: 1 },
    { label: 'פברואר', value: 2 },
    { label: 'מרץ', value: 3 },
    { label: 'אפריל', value: 4 },
    { label: 'מאי', value: 5 },
    { label: 'יוני', value: 6 },
    { label: 'יולי', value: 7 },
    { label: 'אוגוסט', value: 8 },
    { label: 'ספטמבר', value: 9 },
    { label: 'אוקטובר', value: 10 },
    { label: 'נובמבר', value: 11 },
    { label: 'דצמבר', value: 12 }
  ];

  constructor(
    private apiService: ApiService,
    private messageService: MessageService
  ) {
    // Generate years array
    const currentYear = new Date().getFullYear();
    for (let i = 0; i < 5; i++) {
      this.years.push({ label: (currentYear - i).toString(), value: currentYear - i });
    }
  }

  ngOnInit(): void {
    this.loadReport();
  }

  loadReport(): void {
    this.isLoading = true;
    
    this.apiService.getMonthlyReport(this.selectedYear, this.selectedMonth).subscribe({
      next: (reports) => {
        this.reports = reports;
        this.updateChart();
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading report:', error);
        this.messageService.add({
          severity: 'error',
          summary: 'שגיאה',
          detail: 'שגיאה בטעינת הדוח'
        });
        this.isLoading = false;
      }
    });
  }

  onDateChange(): void {
    this.loadReport();
  }

  private updateChart(): void {
    this.chartData = {
      labels: this.reports.map(r => r.departmentName),
      datasets: [
        {
          data: this.reports.map(r => r.currentMonthCount),
          label: 'חודש נוכחי',
          backgroundColor: '#3B82F6',
          borderColor: '#3B82F6',
          borderWidth: 1
        },
        {
          data: this.reports.map(r => r.previousMonthCount),
          label: 'חודש קודם',
          backgroundColor: '#F59E0B',
          borderColor: '#F59E0B',
          borderWidth: 1
        },
        {
          data: this.reports.map(r => r.lastYearCount),
          label: 'שנה שעברה',
          backgroundColor: '#10B981',
          borderColor: '#10B981',
          borderWidth: 1
        }
      ]
    };
  }

  getChangeClass(value: number): string {
    if (value > 0) return 'positive-change';
    if (value < 0) return 'negative-change';
    return 'no-change';
  }

  getChangeIcon(value: number): string {
    if (value > 0) return 'pi pi-arrow-up';
    if (value < 0) return 'pi pi-arrow-down';
    return 'pi pi-minus';
  }
}