using InquiriesAPI.Data;
using InquiriesAPI.DTOs;
using InquiriesAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InquiriesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InquiriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InquiriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/inquiries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InquiryResponseDto>>> GetInquiries()
        {
            var inquiries = await _context.Inquiries
                .Include(i => i.Department)
                .Select(i => new InquiryResponseDto
                {
                    Id = i.Id,
                    FullName = i.FullName,
                    Phone = i.Phone,
                    Email = i.Email,
                    DepartmentName = i.Department.Name,
                    Description = i.Description,
                    CreatedAt = i.CreatedAt
                })
                .ToListAsync();

            return Ok(inquiries);
        }

        // GET: api/inquiries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InquiryResponseDto>> GetInquiry(int id)
        {
            var inquiry = await _context.Inquiries
                .Include(i => i.Department)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inquiry == null)
                return NotFound($"פנייה עם מספר {id} לא נמצאה");

            var result = new InquiryResponseDto
            {
                Id = inquiry.Id,
                FullName = inquiry.FullName,
                Phone = inquiry.Phone,
                Email = inquiry.Email,
                DepartmentName = inquiry.Department.Name,
                Description = inquiry.Description,
                CreatedAt = inquiry.CreatedAt
            };

            return Ok(result);
        }

        // POST: api/inquiries
        [HttpPost]
        public async Task<ActionResult<InquiryResponseDto>> CreateInquiry(InquiryCreateDto inquiryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if department exists
            var department = await _context.Departments.FindAsync(inquiryDto.DepartmentId);
            if (department == null)
                return BadRequest("המחלקה שנבחרה אינה קיימת במערכת");

            var inquiry = new Inquiry
            {
                FullName = inquiryDto.FullName,
                Phone = inquiryDto.Phone,
                Email = inquiryDto.Email,
                DepartmentId = inquiryDto.DepartmentId,
                Description = inquiryDto.Description
            };

            _context.Inquiries.Add(inquiry);
            await _context.SaveChangesAsync();

            // Return the created inquiry
            inquiry = await _context.Inquiries
                .Include(i => i.Department)
                .FirstAsync(i => i.Id == inquiry.Id);

            var result = new InquiryResponseDto
            {
                Id = inquiry.Id,
                FullName = inquiry.FullName,
                Phone = inquiry.Phone,
                Email = inquiry.Email,
                DepartmentName = inquiry.Department.Name,
                Description = inquiry.Description,
                CreatedAt = inquiry.CreatedAt
            };

            return CreatedAtAction(nameof(GetInquiry), new { id = inquiry.Id }, result);
        }

        // PUT: api/inquiries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInquiry(int id, InquiryCreateDto inquiryDto)
        {
            var inquiry = await _context.Inquiries.FindAsync(id);
            if (inquiry == null)
                return NotFound($"פנייה עם מספר {id} לא נמצאה");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if department exists
            var department = await _context.Departments.FindAsync(inquiryDto.DepartmentId);
            if (department == null)
                return BadRequest("המחלקה שנבחרה אינה קיימת במערכת");

            inquiry.FullName = inquiryDto.FullName;
            inquiry.Phone = inquiryDto.Phone;
            inquiry.Email = inquiryDto.Email;
            inquiry.DepartmentId = inquiryDto.DepartmentId;
            inquiry.Description = inquiryDto.Description;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/inquiries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInquiry(int id)
        {
            var inquiry = await _context.Inquiries.FindAsync(id);
            if (inquiry == null)
                return NotFound($"פנייה עם מספר {id} לא נמצאה");

            _context.Inquiries.Remove(inquiry);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}