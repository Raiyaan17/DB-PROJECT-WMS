using api.Models; // Added for VwAvailableCourse and WheresMyScheduleDbContext
using api.Models.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System; // Required for TimeOnly

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
            return await _context.VwAvailableCourses
                                 .FromSqlRaw("EXEC Course.sp_GetAvailableCourses")
                                 .ToListAsync();
        }

        public async Task<IEnumerable<VwAvailableCourse>> GetAvailableCoursesByDepartmentAsync(string departmentCode)
        {
            return await _context.VwAvailableCourses
                                 .FromSqlRaw("EXEC Course.sp_GetAvailableCourses @departmentCode = {0}", departmentCode)
                                 .ToListAsync();
        }

        public async Task<CourseDetailDto?> GetCourseDetailsAsync(string courseCode)
        {
            var spResults = await _context.Set<CourseDetailSpResult>()
                                  .FromSqlRaw("EXEC Course.sp_GetCourseDetails @courseCode = {0}", courseCode)
                                  .ToListAsync();

            if (!spResults.Any())
            {
                return null; // Course not found
            }

            // Assume all results share the same main course details
            var firstResult = spResults.First();
            var courseDetailDto = new CourseDetailDto
            {
                CourseCode = firstResult.CourseCode,
                CourseTitle = firstResult.CourseTitle,
                TotalCredits = firstResult.TotalCredits,
                Capacity = firstResult.Capacity,
                EnrolledCount = firstResult.EnrolledCount,
                RemainingSeats = firstResult.RemainingSeats,
                Venue = firstResult.Venue,
                DayOfWeek = firstResult.DayOfWeek,
                StartTime = firstResult.StartTime,
                EndTime = firstResult.EndTime,
                IsActive = firstResult.IsActive,
                BllMode = BllMode.Sproc.ToString(), // Set BLL mode
                DataSource = "Extracted from Stored Procedure",   // Set data source
                Prerequisites = new List<PrerequisiteDto>()
            };

            // Add prerequisites if they exist. Use a HashSet to avoid duplicate prerequisites
            // in case the SP returns them multiple times due to complex joins (though not expected here).
            var addedPrerequisites = new HashSet<string>();
            foreach (var result in spResults.Where(r => r.PrerequisiteCode != null))
            {
                if (addedPrerequisites.Add(result.PrerequisiteCode!)) // Only add if not already added
                {
                    courseDetailDto.Prerequisites.Add(new PrerequisiteDto
                    {
                        CourseCode = result.PrerequisiteCode!,
                        CourseTitle = result.PrerequisiteTitle!
                    });
                }
            }

            return courseDetailDto;
        }
        public async Task<IEnumerable<Department>> GetDepartmentsAsync()
        {
            return await _context.Departments
                                 .FromSqlRaw("EXEC Course.sp_GetDepartments")
                                 .ToListAsync();
        }
        public async Task<IEnumerable<VwAvailableCourse>> SearchCoursesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return await GetAvailableCoursesAsync();
            }

            return await _context.VwAvailableCourses
                                 .FromSqlRaw("EXEC Course.sp_SearchCourses @query = {0}", query)
                                 .ToListAsync();
        }
    }
}

