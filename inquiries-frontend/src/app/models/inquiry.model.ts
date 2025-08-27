export interface Department {
  id: number;
  name: string;
  createdAt?: Date;
}

export interface InquiryCreate {
  fullName: string;
  phone: string;
  email: string;
  departmentId: number;
  description: string;
}

export interface InquiryResponse {
  id: number;
  fullName: string;
  phone: string;
  email: string;
  departmentName: string;
  description: string;
  createdAt: Date;
}

export interface MonthlyReport {
  departmentName: string;
  currentMonthCount: number;
  previousMonthCount: number;
  lastYearCount: number;
  diffFromPreviousMonth: number;
  diffFromLastYear: number;
  percentChangeFromPreviousMonth?: number;
  percentChangeFromLastYear?: number;
}