using api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.BusinessLogic
{
    public class StudentSpBll : IStudentBll
    {
        private readonly WheresMyScheduleDbContext _context;

        public StudentSpBll(WheresMyScheduleDbContext context)
        {
            _context = context;
        }

        public Task AddToCartAsync(string studentId, string courseCode)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<string>> ValidateCartAsync(string studentId)
        {
            throw new System.NotImplementedException();
        }

        public Task EnrollFromCartAsync(string studentId)
        {
            throw new System.NotImplementedException();
        }

        public Task AddToWaitlistAsync(string studentId, string courseCode)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<VwStudentSchedule>> GetScheduleAsync(string studentId)
        {
            throw new System.NotImplementedException();
        }

        public Task DropCourseAsync(string studentId, string courseCode)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveFromCartAsync(string studentId, string courseCode)
        {
            throw new System.NotImplementedException();
        }
    }
}
