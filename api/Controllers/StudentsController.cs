using api.BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic; // Required for IEnumerable
using api.Models;
using System; // Required for Exception

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentBll _studentBll;

        public StudentsController(IStudentBll studentBll)
        {
            _studentBll = studentBll;
        }

        [HttpPost("{studentId}/cart/{courseCode}")]
        public async Task<IActionResult> AddToCart(string studentId, string courseCode)
        {
            try
            {
                await _studentBll.AddToCartAsync(studentId, courseCode);
                return Ok($"Course {courseCode} added to cart for student {studentId}.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{studentId}/cart/{courseCode}")]
        public async Task<IActionResult> RemoveFromCart(string studentId, string courseCode)
        {
            try
            {
                await _studentBll.RemoveFromCartAsync(studentId, courseCode);
                return Ok($"Course {courseCode} removed from cart for student {studentId}.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{studentId}/cart/validate")]
        public async Task<ActionResult<List<string>>> ValidateCart(string studentId)
        {
            try
            {
                var errors = await _studentBll.ValidateCartAsync(studentId);
                return Ok(errors);
            }
            catch (Exception ex)
            {
                // If the validation itself throws an exception (e.g., student not found),
                // return BadRequest.
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{studentId}/enroll")]
        public async Task<IActionResult> EnrollFromCart(string studentId)
        {
            try
            {
                await _studentBll.EnrollFromCartAsync(studentId);
                return Ok($"Enrollment process completed for student {studentId}. Check cart for any remaining failed courses.");
            }
            catch (Exception ex)
            {
                // The stored procedure will throw an exception with combined error messages
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{studentId}/waitlist/{courseCode}")]
        public async Task<IActionResult> AddToWaitlist(string studentId, string courseCode)
        {
            try
            {
                await _studentBll.AddToWaitlistAsync(studentId, courseCode);
                return Ok($"Student {studentId} added to waitlist for course {courseCode}.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{studentId}/schedule")]
        public async Task<ActionResult<IEnumerable<VwStudentSchedule>>> GetSchedule(string studentId)
        {
            try
            {
                var schedule = await _studentBll.GetScheduleAsync(studentId);
                return Ok(schedule);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{studentId}/enroll/{courseCode}")]
        public async Task<IActionResult> DropCourse(string studentId, string courseCode)
        {
            try
            {
                await _studentBll.DropCourseAsync(studentId, courseCode);
                return Ok($"Course {courseCode} dropped for student {studentId}.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

