using System.ComponentModel.DataAnnotations;

namespace InquiriesAPI.Models
{
	public class Department
	{
		public int Id { get; set; }

		[Required]
		[StringLength(100)]
		public string Name { get; set; } = string.Empty;

		public DateTime CreatedAt { get; set; } = DateTime.Now;

		public virtual ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();
	}
}