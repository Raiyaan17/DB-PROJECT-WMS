using api.Models;
using api.Models.DTOs;
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
                return errors;
            }

            // Get all courses in the student's cart with details and prerequisites
            var cartCourses = await _context.CourseCarts
                                            .Where(cc => cc.StudentId == studentId)
                                            .Include(cc => cc.CourseCodeNavigation)
                                                .ThenInclude(c => c.Prerequisites)
                                            .ToListAsync();

            if (!cartCourses.Any())
            {
                return errors;
            }

            // 2. Prerequisite Check
            // Get list of completed course codes for the student
            var completedCourseCodes = await _context.Enrollments
                                                     .Where(e => e.StudentId == studentId && e.Completed)
                                                     .Select(e => e.CourseCode)
                                                     .ToListAsync();
            var completedCourseSet = new HashSet<string>(completedCourseCodes);

            foreach (var cartItem in cartCourses)
            {
                var course = cartItem.CourseCodeNavigation;
                foreach (var prereq in course.Prerequisites)
                {
                    if (!completedCourseSet.Contains(prereq.CourseCode))
                    {
                        errors.Add($"Prerequisites not met for course: {course.CourseCode}");
                        // Break after first missing prereq for this course to avoid duplicate messages per course
                        break; 
                    }
                }
            }

            // 3. Time Overlap Check (In-Memory)
            // 3a. Check within cart
            for (int i = 0; i < cartCourses.Count; i++)
            {
                var course1 = cartCourses[i].CourseCodeNavigation;
                for (int j = i + 1; j < cartCourses.Count; j++)
                {
                    var course2 = cartCourses[j].CourseCodeNavigation;

                    if (course1.DayOfWeek == course2.DayOfWeek)
                    {
                        // Check for overlap: Start1 < End2 AND Start2 < End1
                        if (course1.StartTime < course2.EndTime && course2.StartTime < course1.EndTime)
                        {
                            // Ensure we report the conflict in a consistent order (e.g., lexical) or just as found
                            // The SQL function ordered by CourseCode, let's mimic that loosely by just reporting the pair
                             errors.Add($"Time conflict between {course1.CourseCode} and {course2.CourseCode} on {course1.DayOfWeek}.");
                        }
                    }
                }
            }

            // 3b. Check against active enrollments
            var activeEnrollments = await _context.Enrollments
                                                  .Where(e => e.StudentId == studentId && !e.Completed)
                                                  .Include(e => e.CourseCodeNavigation)
                                                  .ToListAsync();

            foreach (var cartItem in cartCourses)
            {
                var course = cartItem.CourseCodeNavigation;
                foreach (var enrolled in activeEnrollments)
                {
                    var enrolledCourse = enrolled.CourseCodeNavigation;
                    if (course.DayOfWeek == enrolledCourse.DayOfWeek &&
                        course.StartTime < enrolledCourse.EndTime && enrolledCourse.StartTime < course.EndTime)
                    {
                        errors.Add($"Time conflict between {course.CourseCode} and enrolled course {enrolledCourse.CourseCode} on {course.DayOfWeek}.");
                    }
                }
            }

            // 4. Credit Limit Check (Preliminary)
            // Get credits for courses in cart
            var cartCredits = cartCourses.Sum(cc => cc.CourseCodeNavigation.TotalCredits);

            // Get credits for already enrolled courses (not completed)
            var enrolledCredits = activeEnrollments.Sum(e => e.CourseCodeNavigation.TotalCredits); // Use in-memory list

            if (cartCredits + enrolledCredits > MaxCreditLimit)
            {
                errors.Add($"Exceeds maximum credit limit of {MaxCreditLimit}. Total credits: {cartCredits + enrolledCredits}.");
            }

            // Return the list of errors. If empty, the cart is valid.
            return errors;
        }

        public async Task EnrollFromCartAsync(string studentId)
        {
            var failures = new List<EnrollmentFailureResult>();

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. Fetch Cart Items with details
                    var cartCourses = await _context.CourseCarts
                                                    .Where(cc => cc.StudentId == studentId)
                                                    .Include(cc => cc.CourseCodeNavigation)
                                                        .ThenInclude(c => c.Prerequisites)
                                                    .ToListAsync();

                    if (!cartCourses.Any())
                    {
                        // No items to process, just commit (empty transaction) and return
                        await transaction.CommitAsync();
                        return;
                    }

                    // 2. Fetch Completed Enrollments for Prereq Check
                    var completedCourseCodes = await _context.Enrollments
                                                             .Where(e => e.StudentId == studentId && e.Completed)
                                                             .Select(e => e.CourseCode)
                                                             .ToListAsync();
                    var completedCourseSet = new HashSet<string>(completedCourseCodes);

                    // 3. Fetch Active Enrollments for Time Conflict Check
                    var activeEnrollments = await _context.Enrollments
                                                          .Where(e => e.StudentId == studentId && !e.Completed)
                                                          .Include(e => e.CourseCodeNavigation)
                                                          .ToListAsync();

                    var coursesToProcess = new List<CourseCart>();

                    // --- Validation Loop ---
                    foreach (var cartItem in cartCourses)
                    {
                        var course = cartItem.CourseCodeNavigation;
                        bool isValid = true;

                        // A. Prerequisite Check
                        foreach (var prereq in course.Prerequisites)
                        {
                            if (!completedCourseSet.Contains(prereq.CourseCode))
                            {
                                failures.Add(new EnrollmentFailureResult { CourseCode = course.CourseCode, ErrorMessage = "Prerequisite requirements not met." });
                                isValid = false;
                                break; 
                            }
                        }
                        if (!isValid) continue;

                        // B. Time Conflict Check
                        // B1. Conflict with other courses in cart (that are effectively valid so far)
                        foreach (var otherItem in cartCourses)
                        {
                            if (otherItem.CourseCode == course.CourseCode) continue;
                            var other = otherItem.CourseCodeNavigation;
                            
                            if (course.DayOfWeek == other.DayOfWeek && 
                                course.StartTime < other.EndTime && other.StartTime < course.EndTime)
                            {
                                failures.Add(new EnrollmentFailureResult { CourseCode = course.CourseCode, ErrorMessage = $"Time conflict in cart with course: {other.CourseCode}" });
                                isValid = false;
                                break;
                            }
                        }
                        if (!isValid) continue;

                        // B2. Conflict with enrolled courses
                        foreach (var enrolled in activeEnrollments)
                        {
                            var enrolledCourse = enrolled.CourseCodeNavigation;
                            if (course.DayOfWeek == enrolledCourse.DayOfWeek &&
                                course.StartTime < enrolledCourse.EndTime && enrolledCourse.StartTime < course.EndTime)
                            {
                                failures.Add(new EnrollmentFailureResult { CourseCode = course.CourseCode, ErrorMessage = $"Time conflict with enrolled course: {enrolledCourse.CourseCode}" });
                                isValid = false;
                                break;
                            }
                        }

                        if (isValid)
                        {
                            coursesToProcess.Add(cartItem);
                        }
                    }

                    // --- Processing Loop ---
                    foreach (var validItem in coursesToProcess)
                    {
                        var course = validItem.CourseCodeNavigation;

                        if (course.RemainingSeats > 0)
                        {
                            // Enroll
                            _context.Enrollments.Add(new Enrollment
                            {
                                StudentId = studentId,
                                GraduationYear = validItem.GraduationYear,
                                CourseCode = course.CourseCode,
                                Completed = false,
                                IsForced = false,
                                EnrollmentDate = DateTime.UtcNow
                            });
                        }
                        else
                        {
                            // Waitlist
                            _context.Waitlists.Add(new Waitlist
                            {
                                CourseCode = course.CourseCode,
                                StudentId = studentId,
                                GraduationYear = validItem.GraduationYear,
                                WaitlistTimestamp = DateTime.UtcNow
                            });
                        }

                        // Remove from Cart
                        _context.CourseCarts.Remove(validItem);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (DbUpdateException ex)
                {
                    await transaction.RollbackAsync();
                    var innerMessage = ex.InnerException?.Message ?? ex.Message;
                    throw new Exception($"Database update failed: {innerMessage}");
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            // Report failures AFTER the transaction is successfully committed/completed
            if (failures.Any())
            {
                var distinctFailures = failures.GroupBy(f => f.CourseCode).Select(g => g.First()).ToList();
                var errorMessages = string.Join(Environment.NewLine, distinctFailures.Select(f => $"{f.CourseCode}: {f.ErrorMessage}"));
                throw new Exception($"Enrollment failed for some courses:{Environment.NewLine}{errorMessages}");
            }
        }

        public async Task AddToWaitlistAsync(string studentId, string courseCode)
        {
            // 1. Check if student exists
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (student == null)
            {
                throw new Exception("Student does not exist."); // Matching SP error message
            }

            // 2. Check if course exists
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseCode == courseCode);
            if (course == null)
            {
                throw new Exception("Course does not exist."); // Matching SP error message
            }

            // 3. Check if course is full
            if (course.RemainingSeats > 0)
            {
                throw new Exception("This course has open seats. Please enroll directly instead of joining the waitlist.");
            }

            // 4. Check if already on waitlist
            bool onWaitlist = await _context.Waitlists.AnyAsync(w => w.StudentId == studentId && w.CourseCode == courseCode);
            if (onWaitlist)
            {
                throw new Exception("You are already on the waitlist for this course.");
            }

            // 5. Add to waitlist
            var waitlistEntry = new Waitlist
            {
                CourseCode = courseCode,
                StudentId = studentId,
                GraduationYear = student.GraduationYear,
                WaitlistTimestamp = DateTime.UtcNow
            };

            _context.Waitlists.Add(waitlistEntry);
            await _context.SaveChangesAsync();
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
            // 1. Check if student exists
            var studentExists = await _context.Students.AnyAsync(s => s.StudentId == studentId);
            if (!studentExists)
            {
                throw new Exception("Student does not exist.");
            }

            // 2. Check if course exists
            var courseExists = await _context.Courses.AnyAsync(c => c.CourseCode == courseCode);
            if (!courseExists)
            {
                throw new Exception("Course does not exist.");
            }

            // 3. Find Enrollment
            var enrollment = await _context.Enrollments
                                           .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseCode == courseCode);

            if (enrollment == null)
            {
                throw new Exception("You are not enrolled in this course.");
            }

            // 4. Remove Enrollment
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync(); 
            // Trigger trg_UpdateCourseCounts handles seat updates and waitlist promotion
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
        public async Task<IEnumerable<CartItemDto>> GetCartAsync(string studentId)
        {
            return await _context.CourseCarts
                                 .Where(cc => cc.StudentId == studentId)
                                 .Include(cc => cc.CourseCodeNavigation)
                                 .Select(cc => new CartItemDto
                                 {
                                     StudentId = cc.StudentId,
                                     CourseCode = cc.CourseCode,
                                     CourseTitle = cc.CourseCodeNavigation.CourseTitle,
                                     TotalCredits = cc.CourseCodeNavigation.TotalCredits,
                                     GraduationYear = cc.GraduationYear
                                 })
                                 .ToListAsync();
        }
        public async Task<StudentDto?> GetStudentAsync(string studentId)
        {
            var student = await _context.Students
                                        .Include(s => s.Department)
                                        .Include(s => s.School)
                                        .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null) return null;

            return new StudentDto
            {
                StudentId = student.StudentId,
                Fname = student.Fname,
                Lname = student.Lname,
                Email = student.Email,
                DepartmentId = student.DepartmentId,
                GraduationYear = student.GraduationYear,
                SchoolName = student.School?.SchoolName ?? "Unknown School"
            };
        }
    }
}
