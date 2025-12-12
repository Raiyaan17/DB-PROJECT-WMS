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
                "EXEC Inst.sp_AddInstructor @InstructorID={0}, @FName={1}, @LName={2}, @Email={3}, @DepartmentID={4}",
                request.InstructorId, request.FName, request.LName, request.Email, request.DepartmentId);
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
            var students = await _context.Students.AsNoTracking().ToListAsync();
            return students.Select(MapStudent);
        }

        public async Task<StudentSummaryDto?> GetStudentAsync(string studentId)
        {
            var student = await _context.Students.AsNoTracking()
                .FirstOrDefaultAsync(s => s.StudentId == studentId);
            return student == null ? null : MapStudent(student);
        }

        public async Task<IEnumerable<InstructorSummaryDto>> GetInstructorsAsync()
        {
            var instructors = await _context.Instructors.AsNoTracking().ToListAsync();
            return instructors.Select(MapInstructor);
        }

        public async Task<InstructorSummaryDto?> GetInstructorAsync(string instructorId)
        {
            var instructor = await _context.Instructors.AsNoTracking()
                .FirstOrDefaultAsync(i => i.InstructorId == instructorId);
            return instructor == null ? null : MapInstructor(instructor);
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync()
        {
            return await _context.Courses.AsNoTracking().ToListAsync();
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
