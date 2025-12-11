using api.Models;
using api.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.Data.SqlClient; // Required for SqlException

namespace api.BusinessLogic
{
    public class StudentSpBll : IStudentBll
    {
        private readonly WheresMyScheduleDbContext _context;

        public StudentSpBll(WheresMyScheduleDbContext context)
        {
            _context = context;
        }

        public async Task AddToCartAsync(string studentId, string courseCode)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC Std.sp_AddToCart @studentId = {0}, @courseCode = {1}",
                    studentId, courseCode
                );
            }
            catch (SqlException ex)
            {
                // Rethrow as a generic exception, preserving the SQL error message
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<string>> ValidateCartAsync(string studentId)
        {
            try
            {
                var errors = await _context.Set<ValidationErrorMessage>()
                                           .FromSqlRaw("EXEC Std.sp_ValidateCart @studentId = {0}", studentId)
                                           .ToListAsync();
                return errors.Select(e => e.ErrorMessage).ToList();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message, ex);
            }
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
            try
            {
                // Call the stored procedure
                // The stored procedure handles all validation and error throwing
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC Course.usp_AddToWaitlist @studentId = {0}, @courseCode = {1}",
                    studentId, courseCode
                );
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<IEnumerable<VwStudentSchedule>> GetScheduleAsync(string studentId)
        {
            var schedule = await _context.VwStudentSchedules
                                 .FromSqlRaw("EXEC Std.sp_GetSchedule @studentId = {0}", studentId)
                                 .ToListAsync();

            // The SP already checks for student existence and throws an error,
            // but if for some reason it returns empty and no error, we can ensure consistency here.
            // However, since the SP itself validates student existence and RAISERRORs,
            // we primarily rely on the catch block for actual errors.
            // If the SP simply returns an empty set for an invalid student (which it shouldn't, due to RAISERROR),
            // the LINQ BLL would throw "Student not found." - let's keep consistency.
            if (!schedule.Any())
            {
                // Attempt to confirm student doesn't exist if SP didn't error but returned empty
                // This check is a fallback if the SP's RAISERROR isn't always caught as expected or is bypassed
                var studentExists = await _context.Students.AnyAsync(s => s.StudentId == studentId);
                if (!studentExists)
                {
                    throw new Exception("Student not found.");
                }
                // If student exists but schedule is empty, return empty schedule.
            }
            return schedule;
        }

        public async Task DropCourseAsync(string studentId, string courseCode)
        {
            try
            {
                // Call the stored procedure
                // The stored procedure handles all validation and error throwing
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC Std.sp_DropCourse @studentId = {0}, @courseCode = {1}",
                    studentId, courseCode
                );
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task RemoveFromCartAsync(string studentId, string courseCode)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC Std.sp_RemoveFromCart @studentId = {0}, @courseCode = {1}",
                    studentId, courseCode
                );
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
