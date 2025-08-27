import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { Department, InquiryCreate } from '../../models/inquiry.model';
import { MessageService } from 'primeng/api';

// PrimeNG Imports
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-inquiry-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    CardModule,
    InputTextModule,
    DropdownModule,
    InputTextareaModule,
    ButtonModule,
    ToastModule
  ],
  templateUrl: './inquiry-form.component.html',
  styleUrls: ['./inquiry-form.component.css']
})
export class InquiryFormComponent implements OnInit {
  inquiryForm: FormGroup;
  departments: Department[] = [];
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService,
    private messageService: MessageService
  ) {
    this.inquiryForm = this.fb.group({
      fullName: ['', [Validators.required, Validators.maxLength(100)]],
      phone: ['', [Validators.required, Validators.pattern(/^[0-9-+()[\] ]{7,20}$/)]],
      email: ['', [Validators.required, Validators.email]],
      departmentId: ['', Validators.required],
      description: ['', [Validators.required, Validators.minLength(10)]]
    });
  }

  ngOnInit(): void {
    this.loadDepartments();
  }

  loadDepartments(): void {
    this.apiService.getDepartments().subscribe({
      next: (departments) => {
        this.departments = departments;
      },
      error: (error) => {
        console.error('Error loading departments:', error);
        this.messageService.add({
          severity: 'error',
          summary: 'שגיאה',
          detail: 'שגיאה בטעינת המחלקות'
        });
      }
    });
  }

  onSubmit(): void {
    if (this.inquiryForm.valid) {
      this.isLoading = true;
      const inquiry: InquiryCreate = this.inquiryForm.value;
      
      this.apiService.createInquiry(inquiry).subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'הצלחה',
            detail: 'הפנייה נשלחה בהצלחה!'
          });
          this.inquiryForm.reset();
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error creating inquiry:', error);
          this.messageService.add({
            severity: 'error',
            summary: 'שגיאה',
            detail: 'שגיאה בשליחת הפנייה. אנא נסו שוב.'
          });
          this.isLoading = false;
        }
      });
    } else {
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.inquiryForm.controls).forEach(key => {
      const control = this.inquiryForm.get(key);
      control?.markAsTouched();
    });
  }

  getErrorMessage(fieldName: string): string {
    const control = this.inquiryForm.get(fieldName);
    if (control?.hasError('required')) {
      return `${this.getFieldDisplayName(fieldName)} הוא שדה חובה`;
    }
    if (control?.hasError('email')) {
      return 'כתובת האימייל אינה תקינה';
    }
    if (control?.hasError('pattern')) {
      return 'מספר הטלפון אינו תקין';
    }
    if (control?.hasError('maxlength')) {
      return `${this.getFieldDisplayName(fieldName)} ארוך מדי`;
    }
    if (control?.hasError('minlength')) {
      return `${this.getFieldDisplayName(fieldName)} קצר מדי (מינימום 10 תווים)`;
    }
    return '';
  }

  private getFieldDisplayName(fieldName: string): string {
    const names: { [key: string]: string } = {
      fullName: 'שם מלא',
      phone: 'טלפון',
      email: 'אימייל',
      departmentId: 'מחלקה',
      description: 'תיאור הפנייה'
    };
    return names[fieldName] || fieldName;
  }
}