-- =============================================
-- Description: Get Students with optional Department Filter and Sort by Grad Year
-- =============================================
CREATE OR ALTER PROCEDURE sp_Admin_GetStudents
    @DepartmentID NVARCHAR(10) = NULL,
    @SortByGradYear BIT = 0
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
    WHERE (@DepartmentID IS NULL OR s.DepartmentID = @DepartmentID)
      AND UPPER(s.CurrentAcademicYear) IN ('FRESHMAN', 'SOPHOMORE', 'JUNIOR', 'SENIOR')
    ORDER BY 
        CASE WHEN @SortByGradYear = 1 THEN s.GraduationYear END ASC,
        CASE WHEN @SortByGradYear = 0 THEN s.StudentID END DESC;
END;
GO
