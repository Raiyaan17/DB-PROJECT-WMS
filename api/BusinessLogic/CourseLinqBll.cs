using api.Models;
using api.Models.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace api.BusinessLogic
{
    public class CourseLinqBll : ICourseBll
    {
        private readonly WheresMyScheduleDbContext _context;

        public CourseLinqBll(WheresMyScheduleDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VwAvailableCourse>> GetAvailableCoursesAsync()
        {
            return await _context.VwAvailableCourses.ToListAsync();
        }

        public async Task<IEnumerable<VwAvailableCourse>> GetAvailableCoursesByDepartmentAsync(string departmentCode)
        {
            return await _context.VwAvailableCourses
                                 .Where(c => c.DepartmentName == departmentCode)
                                 .ToListAsync();
        }

        public async Task<CourseDetailDto?> GetCourseDetailsAsync(string courseCode)
        {
            var course = await _context.Courses
                                       .Include(c => c.Prerequisites)
                                       .FirstOrDefaultAsync(c => c.CourseCode == courseCode);

            if (course == null)
            {
                return null;
            }

            var courseDetailDto = new CourseDetailDto
            {
                CourseCode = course.CourseCode,
                CourseTitle = course.CourseTitle,
                TotalCredits = course.TotalCredits,
                Capacity = course.Capacity,
                EnrolledCount = course.EnrolledCount,
                RemainingSeats = course.RemainingSeats,
                Venue = course.Venue,
                DayOfWeek = course.DayOfWeek,
                StartTime = course.StartTime,
                EndTime = course.EndTime,
                IsActive = course.IsActive,
                BllMode = BllMode.Linq.ToString(), // Set BLL mode
                DataSource = "Computed from LINQ query",   // Set data source
                Prerequisites = course.Prerequisites.Select(p => new PrerequisiteDto
                {
                    CourseCode = p.CourseCode,
                    CourseTitle = p.CourseTitle
                }).ToList()
            };

            return courseDetailDto;
        }
        public async Task<IEnumerable<Department>> GetDepartmentsAsync()
        {
            return await _context.Departments.ToListAsync();
        }
        public async Task<IEnumerable<VwAvailableCourse>> SearchCoursesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return await GetAvailableCoursesAsync();
            }

            query = query.ToLower();
            return await _context.VwAvailableCourses
                                 .Where(c => c.CourseCode.ToLower().Contains(query) || 
                                             c.CourseTitle.ToLower().Contains(query))
                                 .ToListAsync();
        }
    }
}
