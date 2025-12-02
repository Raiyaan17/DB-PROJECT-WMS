USE WheresMyScheduleDB;
GO

-----------------------------------------------------------
-- VIEW 5: vw_StudentTranscript
-- Shows a student's completed & ongoing courses.
-----------------------------------------------------------
CREATE OR ALTER VIEW Std.vw_StudentTranscript AS
SELECT 
    e.StudentID,
    e.CourseCode,
    c.CourseTitle,
    c.TotalCredits,
    e.Completed,
    e.EnrollmentDate
FROM Std.Enrollment e
JOIN Course.Course c ON e.CourseCode = c.CourseCode;
GO
