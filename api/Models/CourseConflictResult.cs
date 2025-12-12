namespace api.Models
{
    public class CourseConflictResult
    {
        public string? ConflictingCourse1Code { get; set; }
        public string? ConflictingCourse1Title { get; set; }
        public string? ConflictingCourse2Code { get; set; }
        public string? ConflictingCourse2Title { get; set; }
        public string? ConflictDay { get; set; }
        public System.TimeOnly ConflictStartTime { get; set; }
        public System.TimeOnly ConflictEndTime { get; set; }
    }
}
