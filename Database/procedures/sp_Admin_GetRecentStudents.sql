-- =============================================
-- Description: Get Top 25 Recent Students (Freshman-Senior)
-- =============================================
CREATE OR ALTER PROCEDURE sp_Admin_GetRecentStudents
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP(25)
        s.StudentID,
        s.FName,
        s.LName,
        s.Email,
        s.SchoolID,
        s.DepartmentID,
        s.GraduationYear,
        s.CurrentAcademicYear,
        s.PasswordHash
    FROM Std.Student s
    WHERE UPPER(s.CurrentAcademicYear) IN ('FRESHMAN', 'SOPHOMORE', 'JUNIOR', 'SENIOR')
    ORDER BY s.StudentID DESC;
END;
GO
