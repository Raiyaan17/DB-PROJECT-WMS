using System;
using api.Models;
using api.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace api.BusinessLogic
{
    public class AdminSpBll : IAdminBll
    {
        private readonly WheresMyScheduleDbContext _context;

        public AdminSpBll(WheresMyScheduleDbContext context)
        {
            _context = context;
        }

        public async Task AddStudentAsync(AddStudentRequest request)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC Std.sp_AddStudent @StudentID={0}, @FName={1}, @LName={2}, @Email={3}, @SchoolID={4}, @DepartmentID={5}, @GraduationYear={6}, @CurrentAcademicYear={7}",
                request.StudentId, request.FName, request.LName, request.Email, request.SchoolId, request.DepartmentId, request.GraduationYear, request.CurrentAcademicYear);
        }

        public async Task AddInstructorAsync(AddInstructorRequest request)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC Inst.sp_AddInstructor @FName={0}, @LName={1}, @Email={2}, @DepartmentID={3}",
                request.FName, request.LName, request.Email, request.DepartmentId);
        }

        public async Task AddCourseAsync(AddCourseRequest request)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC Course.sp_AddCourse @CourseCode={0}, @CourseTitle={1}, @TotalCredits={2}, @Capacity={3}, @DepartmentID={4}, @InstructorID={5}, @Venue={6}, @DayOfWeek={7}, @StartTime={8}, @EndTime={9}, @IsActive={10}",
                request.CourseCode, request.CourseTitle, request.TotalCredits, request.Capacity, request.DepartmentId, request.InstructorId, request.Venue, request.DayOfWeek, request.StartTime, request.EndTime, request.IsActive);
        }

        public async Task AdminWaitlistStudentAsync(AdminWaitlistRequest request)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC Course.sp_AdminWaitlistStudent @StudentID={0}, @CourseCode={1}",
                request.StudentId, request.CourseCode);
        }

        public async Task ForceEnrollAsync(ForceEnrollRequest request)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC Std.sp_ForceEnroll @StudentID={0}, @CourseCode={1}",
                request.StudentId, request.CourseCode);
        }

        public async Task UpdateCapacityAsync(string courseCode, short capacity)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC Course.sp_UpdateCapacity @CourseCode={0}, @Capacity={1}",
                courseCode, capacity);
        }

        public async Task UpdateCourseStatusAsync(string courseCode, bool isActive)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC Course.sp_SetCourseStatus @CourseCode={0}, @IsActive={1}",
                courseCode, isActive);
        }

        public async Task UpdateEnrollmentCompletionAsync(UpdateEnrollmentCompletionRequest request)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC Std.sp_SetEnrollmentCompletion @StudentID={0}, @CourseCode={1}, @Completed={2}",
                request.StudentId, request.CourseCode, request.Completed);
        }

        public async Task<IEnumerable<StudentSummaryDto>> GetStudentsAsync()
        {
            var students = await _context.Students.FromSqlRaw("EXEC sp_Admin_GetRecentStudents").ToListAsync();
            return students.Select(MapStudent);
        }

        public async Task<int> GetTotalEnrolledStudentCountAsync()
        {
            var countParam = new Microsoft.Data.SqlClient.SqlParameter("@Count", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC sp_Admin_GetTotalStudentCount @Count OUT", countParam);

            return (int)countParam.Value;
        }

        public async Task<StudentSummaryDto?> GetStudentAsync(string studentId)
        {
            var student = await _context.Students.AsNoTracking()
                .FirstOrDefaultAsync(s => s.StudentId == studentId);
            return student == null ? null : MapStudent(student);
        }

        public async Task<IEnumerable<int>> GetArchivedYearsAsync()
        {
            // Since this returns a list of simple types (int), we cannot use FromSqlRaw on DbSet<Student> directly to map it 
            // unless we map back to an entity. However, EF Core 8+ supports primitive mapping, but let's be safe.
            // A concise way using FromSqlRaw is usually on a Keyless Entity.
            // But actually, we can just execute the command and map.
            // Or better, let's use the Database.SqlQuery (EF Core 8 feature) or execute raw reader. 
            // Given earlier code style, I'll use a raw command to be safe and compatible.

            var years = new List<int>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC sp_Admin_GetArchivedYears";
                command.CommandType = System.Data.CommandType.Text;
                _context.Database.OpenConnection();

                using (var result = await command.ExecuteReaderAsync())
                {
                    while (await result.ReadAsync())
                    {
                        if (!result.IsDBNull(0))
                        {
                            years.Add(result.GetInt16(0)); // GraduationYear is SMALLINT (Int16)
                        }
                    }
                }
            }
            return years;
        }

        public async Task<IEnumerable<StudentSummaryDto>> GetArchivedStudentsByYearAsync(int year)
        {
            var students = await _context.Students
                .FromSqlRaw("EXEC sp_Admin_GetArchivedStudentsByYear @Year={0}", year)
                .ToListAsync();
            return students.Select(MapStudent);
        }

        public async Task<IEnumerable<InstructorSummaryDto>> GetInstructorsAsync(string? departmentId = null)
        {
            var instructors = await _context.Instructors
                .FromSqlRaw("EXEC sp_Admin_GetInstructors @DepartmentID={0}", departmentId)
                .ToListAsync();
            return instructors.Select(MapInstructor);
        }

        public async Task<IEnumerable<string>> GetDepartmentsAsync()
        {
            var depts = new List<string>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC sp_Admin_GetDepartments";
                command.CommandType = System.Data.CommandType.Text;
                _context.Database.OpenConnection();

                using (var result = await command.ExecuteReaderAsync())
                {
                    while (await result.ReadAsync())
                    {
                         depts.Add(result.GetString(0));
                    }
                }
            }
            return depts;
        }

        public async Task<InstructorSummaryDto?> GetInstructorAsync(string instructorId)
        {
            var instructor = await _context.Instructors.AsNoTracking()
                .FirstOrDefaultAsync(i => i.InstructorId == instructorId);
            return instructor == null ? null : MapInstructor(instructor);
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync(string? departmentId = null)
        {
            return await _context.Courses
                .FromSqlRaw("EXEC sp_Admin_GetCourses @DepartmentID={0}", departmentId)
                .ToListAsync();
        }

        public async Task<Course?> GetCourseAsync(string courseCode)
        {
            return await _context.Courses.AsNoTracking()
                .FirstOrDefaultAsync(c => c.CourseCode == courseCode);
        }

        private static StudentSummaryDto MapStudent(Student s) => new StudentSummaryDto
        {
            StudentId = s.StudentId,
            FName = s.Fname,
            LName = s.Lname,
            Email = s.Email,
            SchoolId = s.SchoolId,
            DepartmentId = s.DepartmentId,
            GraduationYear = s.GraduationYear,
            CurrentAcademicYear = s.CurrentAcademicYear
        };

        private static InstructorSummaryDto MapInstructor(Instructor i) => new InstructorSummaryDto
        {
            InstructorId = i.InstructorId,
            FName = i.Fname,
            LName = i.Lname,
            Email = i.Email,
            DepartmentId = i.DepartmentId
        };
    }
}
