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
                GraduationYear = CalculateGraduationYear(request.CurrentAcademicYear),
                CurrentAcademicYear = request.CurrentAcademicYear
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();
        }

        private static short CalculateGraduationYear(string academicYear)
        {
            int currentYear = DateTime.Now.Year;
            // Assuming current year is the START of the academic year (e.g. Fall 2025)
            // Freshman (enrolled 2025) -> Grad 2029 (+4)
            // Sophomore (enrolled 2024, now 2nd year) -> Grad 2028 (+3)
            // Junior (enrolled 2023, now 3rd year) -> Grad 2027 (+2)
            // Senior (enrolled 2022, now 4th year) -> Grad 2026 (+1)

            return academicYear.ToUpper() switch
            {
                "FRESHMAN" => (short)(currentYear + 4),
                "SOPHOMORE" => (short)(currentYear + 3),
                "JUNIOR" => (short)(currentYear + 2),
                "SENIOR" => (short)(currentYear + 1),
                _ => (short)(currentYear + 4) // Default to Freshman logic if unknown
            };
        }

        public async Task AddInstructorAsync(AddInstructorRequest request)
        {
            if (await _context.Instructors.AnyAsync(i => i.Email == request.Email))
                throw new InvalidOperationException("Instructor with this email already exists.");

            var instructor = new Instructor
            {
                InstructorId = "PENDING", // Placeholder, Trigger will generate actual ID
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

            // Check for time conflicts
            var targetCourse = await _context.Courses
                .Select(c => new { c.CourseCode, c.DayOfWeek, c.StartTime, c.EndTime })
                .FirstAsync(c => c.CourseCode == request.CourseCode);

            if (targetCourse.DayOfWeek != null && targetCourse.StartTime != null && targetCourse.EndTime != null)
            {
                var conflictingCourse = await _context.Enrollments
                    .Where(e => e.StudentId == request.StudentId && !e.Completed)
                    .Select(e => e.CourseCodeNavigation)
                    .Where(c => c.DayOfWeek == targetCourse.DayOfWeek &&
                                c.StartTime < targetCourse.EndTime &&
                                targetCourse.StartTime < c.EndTime)
                    .Select(c => c.CourseCode)
                    .FirstOrDefaultAsync();

                if (conflictingCourse != null)
                {
                    throw new InvalidOperationException($"Time conflict detected with existing course: {conflictingCourse}");
                }
            }

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
            var studentId = request.StudentId.Trim();
            var courseCode = request.CourseCode.Trim();

            var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e =>
                e.StudentId == studentId && e.CourseCode == courseCode)
                ?? throw new InvalidOperationException("Enrollment does not exist for the given student and course.");

            enrollment.Completed = request.Completed;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<StudentSummaryDto>> GetStudentsAsync(string? departmentId = null, bool sortByGradYear = false)
        {
            var validYears = new[] { "FRESHMAN", "SOPHOMORE", "JUNIOR", "SENIOR" };
            var query = _context.Students.AsNoTracking()
                .Where(s => validYears.Contains(s.CurrentAcademicYear.ToUpper()));

            if (!string.IsNullOrEmpty(departmentId))
            {
                query = query.Where(s => s.DepartmentId == departmentId);
            }

            if (sortByGradYear)
            {
                query = query.OrderBy(s => s.GraduationYear);
            }
            else
            {
                query = query.OrderByDescending(s => s.StudentId);
            }

            var students = await query.Take(50).ToListAsync();
            return students.Select(MapStudent);
        }

        public async Task<int> GetTotalEnrolledStudentCountAsync()
        {
            var validYears = new[] { "FRESHMAN", "SOPHOMORE", "JUNIOR", "SENIOR" };
            return await _context.Students
                .CountAsync(s => validYears.Contains(s.CurrentAcademicYear.ToUpper()));
        }

        public async Task<StudentSummaryDto?> GetStudentAsync(string studentId)
        {
            var student = await _context.Students.AsNoTracking()
                .FirstOrDefaultAsync(s => s.StudentId == studentId);
            return student == null ? null : MapStudent(student);
        }

        public async Task<IEnumerable<int>> GetArchivedYearsAsync()
        {
            return await _context.Students.AsNoTracking()
                .Where(s => s.CurrentAcademicYear == "Alumni" && s.GraduationYear <= 2020)
                .Select(s => (int)s.GraduationYear)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentSummaryDto>> GetArchivedStudentsByYearAsync(int year)
        {
            var students = await _context.Students.AsNoTracking()
                .Where(s => s.CurrentAcademicYear == "Alumni" && s.GraduationYear == year)
                .OrderByDescending(s => s.StudentId) // Top 50
                .Take(50)
                .ToListAsync();
            return students.Select(MapStudent);
        }

        public async Task<IEnumerable<InstructorSummaryDto>> GetInstructorsAsync(string? departmentId = null)
        {
            var query = _context.Instructors.AsNoTracking();

            if (!string.IsNullOrEmpty(departmentId))
            {
                query = query.Where(i => i.DepartmentId == departmentId);
            }

            var instructors = await query
                .OrderBy(i => i.InstructorId.Length)
                .ThenBy(i => i.InstructorId)
                .ToListAsync();
            return instructors.Select(MapInstructor);
        }

        public async Task<IEnumerable<string>> GetDepartmentsAsync()
        {
            return await _context.Departments.AsNoTracking()
                .Select(d => d.DepartmentId)
                .OrderBy(d => d)
                .ToListAsync();
        }

        public async Task<InstructorSummaryDto?> GetInstructorAsync(string instructorId)
        {
            var instructor = await _context.Instructors.AsNoTracking()
                .FirstOrDefaultAsync(i => i.InstructorId == instructorId);
            return instructor == null ? null : MapInstructor(instructor);
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync(string? departmentId = null)
        {
            var query = _context.Courses.AsNoTracking();

            if (!string.IsNullOrEmpty(departmentId))
            {
                // Filter courses that are in the specified department
                var courseCodes = _context.DepartmentCourses.AsNoTracking()
                    .Where(dc => dc.DepartmentId == departmentId)
                    .Select(dc => dc.CourseCode);

                query = query.Where(c => courseCodes.Contains(c.CourseCode));
            }

            return await query.ToListAsync();
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
