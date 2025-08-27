using InquiriesAPI.Data;
using InquiriesAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InquiriesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/reports/monthly?year=2024&month=12
        [HttpGet("monthly")]
        public async Task<ActionResult<IEnumerable<MonthlyReportDto>>> GetMonthlyReport(int year, int month)
        {
            if (year < 2020 || year > DateTime.Now.Year + 1)
                return BadRequest("שנה לא תקינה");

            if (month < 1 || month > 12)
                return BadRequest("חודש לא תקין");

            // Get all departments
            var departments = await _context.Departments.ToListAsync();
            var reports = new List<MonthlyReportDto>();

            foreach (var department in departments)
            {
                // Current month data
                var currentMonthStart = new DateTime(year, month, 1);
                var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);

                var currentMonthCount = await _context.Inquiries
                    .Where(i => i.DepartmentId == department.Id &&
                               i.CreatedAt >= currentMonthStart &&
                               i.CreatedAt <= currentMonthEnd)
                    .CountAsync();

                // Previous month data
                var previousMonthStart = currentMonthStart.AddMonths(-1);
                var previousMonthEnd = previousMonthStart.AddMonths(1).AddDays(-1);

                var previousMonthCount = await _context.Inquiries
                    .Where(i => i.DepartmentId == department.Id &&
                               i.CreatedAt >= previousMonthStart &&
                               i.CreatedAt <= previousMonthEnd)
                    .CountAsync();

                // Same month last year
                var lastYearStart = new DateTime(year - 1, month, 1);
                var lastYearEnd = lastYearStart.AddMonths(1).AddDays(-1);

                var lastYearCount = await _context.Inquiries
                    .Where(i => i.DepartmentId == department.Id &&
                               i.CreatedAt >= lastYearStart &&
                               i.CreatedAt <= lastYearEnd)
                    .CountAsync();

                // Calculate differences and percentages
                var diffFromPreviousMonth = currentMonthCount - previousMonthCount;
                var diffFromLastYear = currentMonthCount - lastYearCount;

                decimal? percentChangeFromPreviousMonth = null;
                if (previousMonthCount > 0)
                    percentChangeFromPreviousMonth = (decimal)(diffFromPreviousMonth * 100.0) / previousMonthCount;

                decimal? percentChangeFromLastYear = null;
                if (lastYearCount > 0)
                    percentChangeFromLastYear = (decimal)(diffFromLastYear * 100.0) / lastYearCount;

                reports.Add(new MonthlyReportDto
                {
                    DepartmentName = department.Name,
                    CurrentMonthCount = currentMonthCount,
                    PreviousMonthCount = previousMonthCount,
                    LastYearCount = lastYearCount,
                    DiffFromPreviousMonth = diffFromPreviousMonth,
                    DiffFromLastYear = diffFromLastYear,
                    PercentChangeFromPreviousMonth = percentChangeFromPreviousMonth,
                    PercentChangeFromLastYear = percentChangeFromLastYear
                });
            }

            // Order by current month count descending
            return Ok(reports.OrderByDescending(r => r.CurrentMonthCount));
        }

        // GET: api/reports/seed-data - Helper endpoint to add sample data for testing
        [HttpPost("seed-data")]
        public async Task<IActionResult> SeedSampleData()
        {
            var departments = await _context.Departments.ToListAsync();
            var random = new Random();

            // Add sample inquiries for different months and years
            var sampleInquiries = new List<InquiriesAPI.Models.Inquiry>();

            foreach (var dept in departments)
            {
                // Current month data
                for (int i = 0; i < random.Next(5, 25); i++)
                {
                    sampleInquiries.Add(new InquiriesAPI.Models.Inquiry
                    {
                        FullName = $"משתמש דמה {i + 1}",
                        Phone = $"050-{random.Next(1000000, 9999999)}",
                        Email = $"user{i + 1}@example.com",
                        DepartmentId = dept.Id,
                        Description = $"פנייה דמה למחלקת {dept.Name}",
                        CreatedAt = DateTime.Now.AddDays(-random.Next(0, 28))
                    });
                }

                // Previous month data
                for (int i = 0; i < random.Next(3, 20); i++)
                {
                    sampleInquiries.Add(new InquiriesAPI.Models.Inquiry
                    {
                        FullName = $"משתמש דמה {i + 100}",
                        Phone = $"052-{random.Next(1000000, 9999999)}",
                        Email = $"user{i + 100}@example.com",
                        DepartmentId = dept.Id,
                        Description = $"פנייה דמה מהחודש הקודם למחלקת {dept.Name}",
                        CreatedAt = DateTime.Now.AddMonths(-1).AddDays(-random.Next(0, 28))
                    });
                }

                // Last year data
                for (int i = 0; i < random.Next(2, 15); i++)
                {
                    sampleInquiries.Add(new InquiriesAPI.Models.Inquiry
                    {
                        FullName = $"משתמש דמה {i + 200}",
                        Phone = $"054-{random.Next(1000000, 9999999)}",
                        Email = $"user{i + 200}@example.com",
                        DepartmentId = dept.Id,
                        Description = $"פנייה דמה משנה שעברה למחלקת {dept.Name}",
                        CreatedAt = DateTime.Now.AddYears(-1).AddDays(-random.Next(0, 28))
                    });
                }
            }

            _context.Inquiries.AddRange(sampleInquiries);
            await _context.SaveChangesAsync();

            return Ok($"נוספו {sampleInquiries.Count} פניות דמה למסד הנתונים");
        }
    }
}