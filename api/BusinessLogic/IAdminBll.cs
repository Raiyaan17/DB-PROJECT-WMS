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

        Task DeleteStudentAsync(string studentId);
        Task DeleteInstructorAsync(string instructorId);
        Task DeleteCourseAsync(string courseCode);
        Task UpdateStudentAsync(string studentId, UpdateStudentRequest request);
        Task UpdateInstructorAsync(string instructorId, UpdateInstructorRequest request);
        Task UpdateCourseAsync(string courseCode, UpdateCourseRequest request);

        Task<IEnumerable<StudentSummaryDto>> GetStudentsAsync();
        Task<StudentSummaryDto?> GetStudentAsync(string studentId);
        Task<IEnumerable<InstructorSummaryDto>> GetInstructorsAsync();
        Task<InstructorSummaryDto?> GetInstructorAsync(string instructorId);
        Task<IEnumerable<Course>> GetCoursesAsync();
        Task<Course?> GetCourseAsync(string courseCode);
    }
}
