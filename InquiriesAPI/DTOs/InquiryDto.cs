using System;
using System.ComponentModel.DataAnnotations;

namespace InquiriesAPI.DTOs
{
    public class InquiryCreateDto
    {
        [Required(ErrorMessage = "שם מלא הוא שדה חובה")]
        [StringLength(100, ErrorMessage = "שם מלא יכול להכיל עד 100 תווים")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "טלפון הוא שדה חובה")]
        [StringLength(20, ErrorMessage = "טלפון יכול להכיל עד 20 תווים")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "אימייל הוא שדה חובה")]
        [EmailAddress(ErrorMessage = "פורמט האימייל אינו תקין")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "יש לבחור מחלקה")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "תיאור הפנייה הוא שדה חובה")]
        public string Description { get; set; } = string.Empty;
    }

    public class InquiryResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class MonthlyReportDto
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int CurrentMonthCount { get; set; }
        public int PreviousMonthCount { get; set; }
        public int LastYearCount { get; set; }
        public int DiffFromPreviousMonth { get; set; }
        public int DiffFromLastYear { get; set; }
        public decimal? PercentChangeFromPreviousMonth { get; set; }
        public decimal? PercentChangeFromLastYear { get; set; }
    }
}