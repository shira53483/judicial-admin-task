import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Department, InquiryCreate, InquiryResponse, MonthlyReport } from '../models/inquiry.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly baseUrl = 'http://localhost:5067/api';

  constructor(private http: HttpClient) {}

  // Departments
  getDepartments(): Observable<Department[]> {
    return this.http.get<Department[]>(`${this.baseUrl}/departments`);
  }

  // Inquiries
  getInquiries(): Observable<InquiryResponse[]> {
    return this.http.get<InquiryResponse[]>(`${this.baseUrl}/inquiries`);
  }

  getInquiry(id: number): Observable<InquiryResponse> {
    return this.http.get<InquiryResponse>(`${this.baseUrl}/inquiries/${id}`);
  }

  createInquiry(inquiry: InquiryCreate): Observable<InquiryResponse> {
    return this.http.post<InquiryResponse>(`${this.baseUrl}/inquiries`, inquiry);
  }

  updateInquiry(id: number, inquiry: InquiryCreate): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/inquiries/${id}`, inquiry);
  }

  deleteInquiry(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/inquiries/${id}`);
  }

  // Reports
  getMonthlyReport(year: number, month: number): Observable<MonthlyReport[]> {
    const params = new HttpParams()
      .set('year', year.toString())
      .set('month', month.toString());
    
    return this.http.get<MonthlyReport[]>(`${this.baseUrl}/reports/monthly`, { params });
  }
}