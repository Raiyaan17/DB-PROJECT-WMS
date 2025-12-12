using System;

namespace api.Models.DTOs
{
    public class CartItemDto
    {
        public string StudentId { get; set; } = null!;
        public string CourseCode { get; set; } = null!;
        public string CourseTitle { get; set; } = null!;
        public byte TotalCredits { get; set; }
        public short GraduationYear { get; set; }
    }
}
