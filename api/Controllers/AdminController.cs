using api.BusinessLogic;
using api.Models;
using api.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
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

        [HttpDelete("students/{studentId}")]
        public async Task<IActionResult> DeleteStudent(string studentId)
        {
            try
            {
                await _adminBll.DeleteStudentAsync(studentId);
                return Ok("Student deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("instructors/{instructorId}")]
        public async Task<IActionResult> DeleteInstructor(string instructorId)
        {
            try
            {
                await _adminBll.DeleteInstructorAsync(instructorId);
                return Ok("Instructor deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("courses/{courseCode}")]
        public async Task<IActionResult> DeleteCourse(string courseCode)
        {
            try
            {
                await _adminBll.DeleteCourseAsync(courseCode);
                return Ok("Course deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("students/{studentId}")]
        public async Task<IActionResult> UpdateStudent(string studentId, UpdateStudentRequest request)
        {
            try
            {
                await _adminBll.UpdateStudentAsync(studentId, request);
                return Ok("Student updated.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("instructors/{instructorId}")]
        public async Task<IActionResult> UpdateInstructor(string instructorId, UpdateInstructorRequest request)
        {
            try
            {
                await _adminBll.UpdateInstructorAsync(instructorId, request);
                return Ok("Instructor updated.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("courses/{courseCode}")]
        public async Task<IActionResult> UpdateCourse(string courseCode, UpdateCourseRequest request)
        {
            try
            {
                await _adminBll.UpdateCourseAsync(courseCode, request);
                return Ok("Course updated.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("students")]
        public async Task<IEnumerable<StudentSummaryDto>> GetStudents()
        {
            return await _adminBll.GetStudentsAsync();
        }

        [HttpGet("students/{studentId}")]
        public async Task<ActionResult<StudentSummaryDto>> GetStudent(string studentId)
        {
            var student = await _adminBll.GetStudentAsync(studentId);
            return student == null ? NotFound() : Ok(student);
        }

        [HttpGet("instructors")]
        public async Task<IEnumerable<InstructorSummaryDto>> GetInstructors()
        {
            return await _adminBll.GetInstructorsAsync();
        }

        [HttpGet("instructors/{instructorId}")]
        public async Task<ActionResult<InstructorSummaryDto>> GetInstructor(string instructorId)
        {
            var instructor = await _adminBll.GetInstructorAsync(instructorId);
            return instructor == null ? NotFound() : Ok(instructor);
        }

        [HttpGet("courses")]
        public async Task<IEnumerable<Course>> GetCourses()
        {
            return await _adminBll.GetCoursesAsync();
        }

        [HttpGet("courses/{courseCode}")]
        public async Task<ActionResult<Course>> GetCourse(string courseCode)
        {
            var course = await _adminBll.GetCourseAsync(courseCode);
            return course == null ? NotFound() : Ok(course);
        }
    }
}
