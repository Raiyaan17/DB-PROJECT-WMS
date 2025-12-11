using api.Models; // Added for VwAvailableCourse
using api.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.BusinessLogic
{
    public interface ICourseBll
    {
        Task<IEnumerable<VwAvailableCourse>> GetAvailableCoursesAsync();
        Task<IEnumerable<VwAvailableCourse>> GetAvailableCoursesByDepartmentAsync(string departmentCode);
        Task<CourseDetailDto?> GetCourseDetailsAsync(string courseCode);
    }
}
