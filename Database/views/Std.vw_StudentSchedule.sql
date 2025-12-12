USE WheresMyScheduleDB;
GO

-----------------------------------------------------------
-- VIEW 2: vw_StudentSchedule
-- Weekly schedule view for all students.
-- Used to render student calendar UI.
-----------------------------------------------------------
CREATE OR ALTER VIEW Std.vw_StudentSchedule AS
SELECT 
    e.StudentID,
    e.CourseCode,
    c.CourseTitle,
    c.DayOfWeek,
    c.StartTime,
    c.EndTime,
    c.Venue,
    d.DepartmentName,
    CAST(c.TotalCredits AS INT) AS TotalCredits
FROM Std.Enrollment e
JOIN Course.Course c ON e.CourseCode = c.CourseCode
JOIN Dept.Courses dc ON c.CourseCode = dc.CourseCode
JOIN Dept.Department d ON dc.DepartmentID = d.DepartmentID;
GO
