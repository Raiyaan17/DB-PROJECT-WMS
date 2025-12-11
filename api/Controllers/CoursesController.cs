using api.BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using api.Models;
using api.Models.DTOs; // Added for DTOs

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseBll _courseBll;

        public CoursesController(ICourseBll courseBll)
        {
            _courseBll = courseBll;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VwAvailableCourse>>> GetAvailableCourses()
        {
            var courses = await _courseBll.GetAvailableCoursesAsync();
            return Ok(courses);
        }

        [HttpGet("by-department/{departmentCode}")]
        public async Task<ActionResult<IEnumerable<VwAvailableCourse>>> GetAvailableCoursesByDepartment(string departmentCode)
        {
            var courses = await _courseBll.GetAvailableCoursesByDepartmentAsync(departmentCode);
            return Ok(courses);
        }

        [HttpGet("{courseCode}")]
        public async Task<ActionResult<CourseDetailDto>> GetCourseDetails(string courseCode)
        {
            var course = await _courseBll.GetCourseDetailsAsync(courseCode);
            if (course == null)
            {
                return NotFound();
            }
            return Ok(course);
        }
    }
}
