using api.Models; // Added for VwAvailableCourse and WheresMyScheduleDbContext
using api.Models.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace api.BusinessLogic
{
    public class CourseSpBll : ICourseBll
    {
        private readonly WheresMyScheduleDbContext _context;

        public CourseSpBll(WheresMyScheduleDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VwAvailableCourse>> GetAvailableCoursesAsync()
        {
            // Using FromSqlRaw to query the view, simulating stored procedure like behavior for data retrieval
            return await _context.VwAvailableCourses.FromSqlRaw("SELECT * FROM Course.vw_AvailableCourses").ToListAsync();
        }

        public async Task<IEnumerable<VwAvailableCourse>> GetAvailableCoursesByDepartmentAsync(string departmentCode)
        {
            // Using FromSqlRaw with parameters for filtering
            return await _context.VwAvailableCourses
                                 .FromSqlRaw("SELECT * FROM Course.vw_AvailableCourses WHERE DepartmentName = {0}", departmentCode)
                                 .ToListAsync();
        }

        public Task<CourseDetailDto?> GetCourseDetailsAsync(string courseCode)
        {
            // This would require a stored procedure that returns the flattened DTO structure,
            // or logic to assemble it from multiple result sets.
            throw new System.NotImplementedException();
        }
    }
}
