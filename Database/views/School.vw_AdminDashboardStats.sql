USE WheresMyScheduleDB;
GO

-----------------------------------------------------------
-- VIEW 3: vw_AdminDashboardStats
-- High-level admin metrics:
-- total students, instructors, courses by department.
-----------------------------------------------------------
CREATE OR ALTER VIEW School.vw_AdminDashboardStats AS
SELECT 
    d.DepartmentName,
    COUNT(DISTINCT s.StudentID) AS TotalStudents,
    COUNT(DISTINCT i.InstructorID) AS TotalInstructors,
    COUNT(DISTINCT dc.CourseCode) AS TotalCourses
FROM Dept.Department d
LEFT JOIN Std.Student s ON s.DepartmentID = d.DepartmentID
LEFT JOIN Inst.Instructor i ON i.DepartmentID = d.DepartmentID
LEFT JOIN Dept.Courses dc ON dc.DepartmentID = d.DepartmentID
GROUP BY d.DepartmentName;
GO
