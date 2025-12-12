using System;
using api.Models;
using api.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace api.BusinessLogic
{
    public class AdminLinqBll : IAdminBll
    {
        private readonly WheresMyScheduleDbContext _context;

        public AdminLinqBll(WheresMyScheduleDbContext context)
        {
            _context = context;
        }

        public async Task AddStudentAsync(AddStudentRequest request)
        {
            if (await _context.Students.AnyAsync(s => s.StudentId == request.StudentId))
                throw new InvalidOperationException("Student already exists.");

            var student = new Student
            {
                StudentId = request.StudentId,
                Fname = request.FName,
                Lname = request.LName,
                Email = request.Email,
                SchoolId = request.SchoolId,
                DepartmentId = request.DepartmentId,
                GraduationYear = request.GraduationYear,
                CurrentAcademicYear = request.CurrentAcademicYear
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();
        }

        public async Task AddInstructorAsync(AddInstructorRequest request)
        {
            if (await _context.Instructors.AnyAsync(i => i.InstructorId == request.InstructorId))
                throw new InvalidOperationException("Instructor already exists.");

            var instructor = new Instructor
            {
                InstructorId = request.InstructorId,
                Fname = request.FName,
                Lname = request.LName,
                Email = request.Email,
                DepartmentId = request.DepartmentId
            };

            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();
        }

        public async Task AddCourseAsync(AddCourseRequest request)
        {
            if (await _context.Courses.AnyAsync(c => c.CourseCode == request.CourseCode))
                throw new InvalidOperationException("Course already exists.");

            var course = new Course
            {
                CourseCode = request.CourseCode,
                CourseTitle = request.CourseTitle,
                TotalCredits = request.TotalCredits,
                Capacity = request.Capacity,
                Venue = request.Venue,
                DayOfWeek = request.DayOfWeek,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                IsActive = request.IsActive
            };

            await using var transaction = await _context.Database.BeginTransactionAsync();

            _context.Courses.Add(course);
            _context.DepartmentCourses.Add(new DepartmentCourse
            {
                CourseCode = request.CourseCode,
                DepartmentId = request.DepartmentId
            });

            if (!string.IsNullOrWhiteSpace(request.InstructorId))
            {
                var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.InstructorId == request.InstructorId);
                if (instructor == null)
                    throw new InvalidOperationException("Instructor does not exist.");

                course.Instructors.Add(instructor);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public async Task AdminWaitlistStudentAsync(AdminWaitlistRequest request)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == request.StudentId);
            if (student == null)
                throw new InvalidOperationException("Student does not exist.");

            if (!await _context.Courses.AnyAsync(c => c.CourseCode == request.CourseCode))
                throw new InvalidOperationException("Course does not exist.");

            if (await _context.Enrollments.AnyAsync(e => e.StudentId == request.StudentId && e.CourseCode == request.CourseCode))
                throw new InvalidOperationException("Student already enrolled in this course.");

            if (await _context.Waitlists.AnyAsync(w => w.StudentId == request.StudentId && w.CourseCode == request.CourseCode))
                throw new InvalidOperationException("Student already waitlisted for this course.");

            _context.Waitlists.Add(new Waitlist
            {
                CourseCode = request.CourseCode,
                StudentId = request.StudentId,
                GraduationYear = student.GraduationYear
            });

            await _context.SaveChangesAsync();
        }

        public async Task ForceEnrollAsync(ForceEnrollRequest request)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == request.StudentId);
            if (student == null)
                throw new InvalidOperationException("Student does not exist.");

            if (!await _context.Courses.AnyAsync(c => c.CourseCode == request.CourseCode))
                throw new InvalidOperationException("Course does not exist.");

            if (!await _context.Waitlists.AnyAsync(w => w.StudentId == request.StudentId && w.CourseCode == request.CourseCode))
                throw new InvalidOperationException("Student is not on the waitlist for this course.");

            if (await _context.Enrollments.AnyAsync(e => e.StudentId == request.StudentId && e.CourseCode == request.CourseCode))
                throw new InvalidOperationException("Student already enrolled in this course.");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            _context.Enrollments.Add(new Enrollment
            {
                StudentId = request.StudentId,
                GraduationYear = student.GraduationYear,
                CourseCode = request.CourseCode,
                Completed = false,
                IsForced = true
            });

            var waitlist = await _context.Waitlists
                .FirstAsync(w => w.StudentId == request.StudentId && w.CourseCode == request.CourseCode);
            _context.Waitlists.Remove(waitlist);

            _context.AuditLogs.Add(new AuditLog
            {
                AdminId = null,
                StudentId = request.StudentId,
                CourseCode = request.CourseCode,
                ActionDescription = "Forced enrollment from waitlist"
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public async Task UpdateCapacityAsync(string courseCode, short capacity)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseCode == courseCode)
                ?? throw new InvalidOperationException("Course does not exist.");

            if (capacity < course.EnrolledCount)
                throw new InvalidOperationException("New capacity cannot be less than current enrollment.");

            course.Capacity = capacity;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCourseStatusAsync(string courseCode, bool isActive)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseCode == courseCode)
                ?? throw new InvalidOperationException("Course does not exist.");

            course.IsActive = isActive;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEnrollmentCompletionAsync(UpdateEnrollmentCompletionRequest request)
        {
            var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e =>
                e.StudentId == request.StudentId && e.CourseCode == request.CourseCode)
                ?? throw new InvalidOperationException("Enrollment does not exist for the given student and course.");

            enrollment.Completed = request.Completed;
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
