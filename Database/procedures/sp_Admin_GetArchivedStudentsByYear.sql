-- =============================================
-- Description: Get Archived Students by Year (Top 50)
-- =============================================
CREATE OR ALTER PROCEDURE sp_Admin_GetArchivedStudentsByYear
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP(50)
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
    WHERE s.CurrentAcademicYear = 'Alumni' 
      AND s.GraduationYear = @Year
    ORDER BY s.StudentID DESC;
END;
GO
