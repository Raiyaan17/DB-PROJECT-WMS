using api.BusinessLogic;
using api.Models;
using api.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminBll _adminBll;

        public AdminController(IAdminBll adminBll)
        {
            _adminBll = adminBll;
        }

        [HttpPost("students")]
        public async Task<IActionResult> AddStudent(AddStudentRequest request)
        {
            try
            {
                await _adminBll.AddStudentAsync(request);
                return Ok("Student added.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("instructors")]
        public async Task<IActionResult> AddInstructor(AddInstructorRequest request)
        {
            try
            {
                await _adminBll.AddInstructorAsync(request);
                return Ok("Instructor added.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("courses")]
        public async Task<IActionResult> AddCourse(AddCourseRequest request)
        {
            try
            {
                await _adminBll.AddCourseAsync(request);
                return Ok("Course added.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("waitlist")]
        public async Task<IActionResult> WaitlistStudent(AdminWaitlistRequest request)
        {
            try
            {
                await _adminBll.AdminWaitlistStudentAsync(request);
                return Ok("Student waitlisted.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("force-enroll")]
        public async Task<IActionResult> ForceEnroll(ForceEnrollRequest request)
        {
            try
            {
                await _adminBll.ForceEnrollAsync(request);
                return Ok("Student force-enrolled from waitlist.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("courses/{courseCode}/capacity")]
        public async Task<IActionResult> UpdateCapacity(string courseCode, UpdateCapacityRequest request)
        {
            try
            {
                await _adminBll.UpdateCapacityAsync(courseCode, request.Capacity);
                return Ok("Capacity updated.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("courses/{courseCode}/status")]
        public async Task<IActionResult> UpdateCourseStatus(string courseCode, UpdateCourseStatusRequest request)
        {
            try
            {
                await _adminBll.UpdateCourseStatusAsync(courseCode, request.IsActive);
                return Ok("Course status updated.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("enrollments/completion")]
        public async Task<IActionResult> UpdateEnrollmentCompletion(UpdateEnrollmentCompletionRequest request)
        {
            try
            {
                await _adminBll.UpdateEnrollmentCompletionAsync(request);
                return Ok("Enrollment completion updated.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("archive/years")]
        public async Task<IEnumerable<int>> GetArchivedYears()
        {
            return await _adminBll.GetArchivedYearsAsync();
        }

        [HttpGet("archive/students/{year}")]
        public async Task<IEnumerable<StudentSummaryDto>> GetArchivedStudents(int year)
        {
            return await _adminBll.GetArchivedStudentsByYearAsync(year);
        }

        [HttpGet("students/count")]
        public async Task<ActionResult<int>> GetStudentCount()
        {
            var count = await _adminBll.GetTotalEnrolledStudentCountAsync();
            return Ok(count);
        }


        [HttpGet("students")]
        public async Task<IEnumerable<StudentSummaryDto>> GetStudents([FromQuery] string? departmentId = null, [FromQuery] bool sortByGradYear = false)
        {
            return await _adminBll.GetStudentsAsync(departmentId, sortByGradYear);
        }

        [HttpGet("students/{studentId}")]
        public async Task<ActionResult<StudentSummaryDto>> GetStudent(string studentId)
        {
            var student = await _adminBll.GetStudentAsync(studentId);
            return student == null ? NotFound() : Ok(student);
        }

        [HttpGet("departments")]
        public async Task<IEnumerable<string>> GetDepartments()
        {
            return await _adminBll.GetDepartmentsAsync();
        }

        [HttpGet("instructors")]
        public async Task<IEnumerable<InstructorSummaryDto>> GetInstructors([FromQuery] string? departmentId = null)
        {
            return await _adminBll.GetInstructorsAsync(departmentId);
        }

        [HttpGet("instructors/{instructorId}")]
        public async Task<ActionResult<InstructorSummaryDto>> GetInstructor(string instructorId)
        {
            var instructor = await _adminBll.GetInstructorAsync(instructorId);
            return instructor == null ? NotFound() : Ok(instructor);
        }



        [HttpGet("courses")]
        public async Task<IEnumerable<Course>> GetCourses([FromQuery] string? departmentId = null)
        {
            return await _adminBll.GetCoursesAsync(departmentId);
        }

        [HttpGet("courses/{courseCode}")]
        public async Task<ActionResult<Course>> GetCourse(string courseCode)
        {
            var course = await _adminBll.GetCourseAsync(courseCode);
            return course == null ? NotFound() : Ok(course);
        }
    }
}
