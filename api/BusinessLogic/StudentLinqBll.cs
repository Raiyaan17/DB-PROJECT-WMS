using api.Models;
using Microsoft.EntityFrameworkCore; // Required for ToListAsync, FirstOrDefaultAsync
using System.Collections.Generic;
using System.Linq; // Added for LINQ extension methods like Select, Sum, etc.
using System.Threading.Tasks;
using System;

namespace api.BusinessLogic
{
    public class StudentLinqBll : IStudentBll
    {
        private readonly WheresMyScheduleDbContext _context;
        private const int MaxCreditLimit = 20; // Placeholder default credit limit

        public StudentLinqBll(WheresMyScheduleDbContext context)
        {
            _context = context;
        }

        public async Task AddToCartAsync(string studentId, string courseCode)
        {
            // 1. Check if the student exists
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (student == null)
            {
                throw new Exception("Student not found.");
            }

            // 2. Check if the course exists
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseCode == courseCode);
            if (course == null)
            {
                throw new Exception("Course not found.");
            }

            // 3. Check if the student is already enrolled in the course
            var isEnrolled = await _context.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseCode == courseCode && e.Completed == false);
            if (isEnrolled)
            {
                throw new Exception("Student is already enrolled in this course.");
            }

            // 4. Check if the course is already in the cart
            var isInCart = await _context.CourseCarts.AnyAsync(cc => cc.StudentId == studentId && cc.CourseCode == courseCode);
            if (isInCart)
            {
                throw new Exception("Course is already in the cart.");
            }

            // 5. Add to cart
            var courseCartEntry = new CourseCart
            {
                StudentId = studentId,
                CourseCode = courseCode,
                GraduationYear = student.GraduationYear // Assuming GraduationYear from student is needed for CourseCart
            };

            _context.CourseCarts.Add(courseCartEntry);
            await _context.SaveChangesAsync();
        }

        public async Task<List<string>> ValidateCartAsync(string studentId)
        {
            List<string> errors = new List<string>();

            // 1. Check if student exists
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (student == null)
            {
                errors.Add("Student not found.");
                return errors; // Return errors immediately if student doesn't exist
            }

            // Get all courses in the student's cart
            var cartCourses = await _context.CourseCarts
                                            .Where(cc => cc.StudentId == studentId)
                                            .Include(cc => cc.CourseCodeNavigation) // Eager load course details
                                            .ToListAsync();

            if (!cartCourses.Any())
            {
                // An empty cart is valid in that no issues were found, so return empty list.
                return errors;
            }

            // 2. Prerequisite Check (using DB function)
            foreach (var cartItem in cartCourses)
            {
                var prereqStatus = await _context.Database
                                                .SqlQuery<bool>($"SELECT [Value] = Course.fn_CheckPrerequisiteStatus({studentId}, {cartItem.CourseCode})")
                                                .SingleAsync();
                if (!prereqStatus)
                {
                    errors.Add($"Prerequisites not met for course: {cartItem.CourseCode}");
                }
            }

            // 3. Time Overlap Check (using DB function)
            var timeConflicts = await _context.CourseConflictResults
                                              .FromSqlRaw("SELECT * FROM Course.fn_GetCartTimeConflicts({0})", studentId)
                                              .ToListAsync();
            if (timeConflicts.Any())
            {
                foreach(var conflict in timeConflicts)
                {
                    errors.Add($"Time conflict between {conflict.ConflictingCourse1Code} and {conflict.ConflictingCourse2Code} on {conflict.ConflictDay}.");
                }
            }

            // 4. Credit Limit Check (Preliminary)
            // Get credits for courses in cart
            var cartCredits = cartCourses.Sum(cc => cc.CourseCodeNavigation.TotalCredits);

            // Get credits for already enrolled courses (not completed)
            var enrolledCredits = await _context.Enrollments
                                                .Where(e => e.StudentId == studentId && e.Completed == false)
                                                .Include(e => e.CourseCodeNavigation)
                                                .SumAsync(e => e.CourseCodeNavigation.TotalCredits);

            if (cartCredits + enrolledCredits > MaxCreditLimit)
            {
                errors.Add($"Exceeds maximum credit limit of {MaxCreditLimit}. Total credits: {cartCredits + enrolledCredits}.");
            }

            // Return the list of errors. If empty, the cart is valid.
            return errors;
        }

        public async Task EnrollFromCartAsync(string studentId)
        {
            // Call the stored procedure and capture any returned failures
            var failures = await _context.EnrollmentFailureResults
                                         .FromSqlRaw("EXEC Std.sp_EnrollFromCart @p0", studentId)
                                         .ToListAsync();

            if (failures.Any())
            {
                var errorMessages = string.Join(Environment.NewLine, failures.Select(f => $"{f.CourseCode}: {f.ErrorMessage}"));
                throw new Exception($"Enrollment failed for some courses:{Environment.NewLine}{errorMessages}");
            }
        }

        public async Task AddToWaitlistAsync(string studentId, string courseCode)
        {
            // Call the stored procedure
            // The stored procedure handles all validation and error throwing
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC Course.usp_AddToWaitlist @studentId = {0}, @courseCode = {1}",
                studentId, courseCode
            );
        }

        public async Task<IEnumerable<VwStudentSchedule>> GetScheduleAsync(string studentId)
        {
            // 1. Check if student exists
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (student == null)
            {
                throw new Exception("Student not found.");
            }

            // 2. Query the view for the student's schedule
            return await _context.VwStudentSchedules
                                 .Where(s => s.StudentId == studentId)
                                 .ToListAsync();
        }

        public async Task DropCourseAsync(string studentId, string courseCode)
        {
            // Call the stored procedure
            // The stored procedure handles all validation and error throwing
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC Std.sp_DropCourse @studentId = {0}, @courseCode = {1}",
                studentId, courseCode
            );
        }

        public async Task RemoveFromCartAsync(string studentId, string courseCode)
        {
            // Find the item in the cart
            var cartItem = await _context.CourseCarts
                                         .FirstOrDefaultAsync(cc => cc.StudentId == studentId && cc.CourseCode == courseCode);

            if (cartItem != null)
            {
                _context.CourseCarts.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                // If the item is not found, you could either do nothing or throw an exception
                // depending on the desired behavior.
                throw new Exception("Course not found in the student's cart.");
            }
        }
    }
}
