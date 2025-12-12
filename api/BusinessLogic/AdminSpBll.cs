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

        public async Task DeleteStudentAsync(string studentId)
        {
            // Inline SQL for Delete
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM Students WHERE StudentId = {0}", studentId);
        }

        public async Task DeleteInstructorAsync(string instructorId)
        {
            // Inline SQL for Delete
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM Instructors WHERE InstructorId = {0}", instructorId);
        }

        public async Task DeleteCourseAsync(string courseCode)
        {
            // Inline SQL for Delete
            // Note: This might fail if there are FK constraints not handled by CASCADE DELETE in DB. 
            // But relying on DB configuration is standard for raw SQL.
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM Courses WHERE CourseCode = {0}", courseCode);
        }

        public async Task UpdateStudentAsync(string studentId, UpdateStudentRequest request)
        {
            // Fallback to EF Core for complex dynamic updates
             var student = await _context.Students.FindAsync(studentId);
            if (student == null) throw new InvalidOperationException("Student not found.");

            if (request.FName != null) student.Fname = request.FName;
            if (request.LName != null) student.Lname = request.LName;
            if (request.Email != null) student.Email = request.Email;
            if (request.SchoolId != null) student.SchoolId = request.SchoolId;
            if (request.DepartmentId != null) student.DepartmentId = request.DepartmentId;
            if (request.GraduationYear.HasValue) student.GraduationYear = request.GraduationYear.Value;
            if (request.CurrentAcademicYear != null) student.CurrentAcademicYear = request.CurrentAcademicYear;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateInstructorAsync(string instructorId, UpdateInstructorRequest request)
        {
             // Fallback to EF Core
            var instructor = await _context.Instructors.FindAsync(instructorId);
            if (instructor == null) throw new InvalidOperationException("Instructor not found.");

            if (request.FName != null) instructor.Fname = request.FName;
            if (request.LName != null) instructor.Lname = request.LName;
            if (request.Email != null) instructor.Email = request.Email;
            if (request.DepartmentId != null) instructor.DepartmentId = request.DepartmentId;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateCourseAsync(string courseCode, UpdateCourseRequest request)
        {
             // Fallback to EF Core
            var course = await _context.Courses.Include(c => c.Instructors).FirstOrDefaultAsync(c => c.CourseCode == courseCode);
            if (course == null) throw new InvalidOperationException("Course not found.");

            if (request.CourseTitle != null) course.CourseTitle = request.CourseTitle;
            if (request.TotalCredits.HasValue) course.TotalCredits = request.TotalCredits.Value;
            if (request.Capacity.HasValue) course.Capacity = request.Capacity.Value;
            if (request.Venue != null) course.Venue = request.Venue;
            if (request.DayOfWeek != null) course.DayOfWeek = request.DayOfWeek;
            if (request.StartTime.HasValue) course.StartTime = request.StartTime.Value;
            if (request.EndTime.HasValue) course.EndTime = request.EndTime.Value;
            if (request.IsActive.HasValue) course.IsActive = request.IsActive.Value;
            
            if (request.DepartmentId != null)
            {
                 var deptCourse = await _context.DepartmentCourses.FirstOrDefaultAsync(dc => dc.CourseCode == courseCode);
                 if (deptCourse != null)
                 {
                     deptCourse.DepartmentId = request.DepartmentId;
                 }
            }

            if (request.InstructorId != null)
            {
                course.Instructors.Clear();
                var instructor = await _context.Instructors.FindAsync(request.InstructorId);
                if (instructor != null)
                {
                    course.Instructors.Add(instructor);
                }
            }

            await _context.SaveChangesAsync();
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
