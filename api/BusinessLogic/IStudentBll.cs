using api.Models;
using api.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.BusinessLogic
{
    public interface IStudentBll
    {
        Task AddToCartAsync(string studentId, string courseCode);
        Task<List<string>> ValidateCartAsync(string studentId);
        Task EnrollFromCartAsync(string studentId);
        Task AddToWaitlistAsync(string studentId, string courseCode);
        Task<IEnumerable<VwStudentSchedule>> GetScheduleAsync(string studentId);
        Task DropCourseAsync(string studentId, string courseCode);
        Task RemoveFromCartAsync(string studentId, string courseCode);
        Task<IEnumerable<CartItemDto>> GetCartAsync(string studentId);
        Task<StudentDto?> GetStudentAsync(string studentId);
    }
}
