using api.Models;
using api.Models.DTOs;

namespace api.BusinessLogic
{
    public interface IAdminBll
    {
        Task AddStudentAsync(AddStudentRequest request);
        Task AddInstructorAsync(AddInstructorRequest request);
        Task AddCourseAsync(AddCourseRequest request);
        Task AdminWaitlistStudentAsync(AdminWaitlistRequest request);
        Task ForceEnrollAsync(ForceEnrollRequest request);
        Task UpdateCapacityAsync(string courseCode, short capacity);
        Task UpdateCourseStatusAsync(string courseCode, bool isActive);
        Task UpdateEnrollmentCompletionAsync(UpdateEnrollmentCompletionRequest request);

        Task<IEnumerable<StudentSummaryDto>> GetStudentsAsync(string? departmentId = null, bool sortByGradYear = false);
        Task<StudentSummaryDto?> GetStudentAsync(string studentId);
        Task<int> GetTotalEnrolledStudentCountAsync();
        Task<IEnumerable<int>> GetArchivedYearsAsync();
        Task<IEnumerable<StudentSummaryDto>> GetArchivedStudentsByYearAsync(int year);
        Task<IEnumerable<InstructorSummaryDto>> GetInstructorsAsync(string? departmentId = null);
        Task<InstructorSummaryDto?> GetInstructorAsync(string instructorId);
        Task<IEnumerable<string>> GetDepartmentsAsync();
        Task<IEnumerable<Course>> GetCoursesAsync(string? departmentId = null);
        Task<Course?> GetCourseAsync(string courseCode);
    }
}
