-- =============================================
-- Description: Get Archived Years (Alumni <= 2020)
-- =============================================
CREATE OR ALTER PROCEDURE sp_Admin_GetArchivedYears
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT GraduationYear
    FROM Std.Student s
    WHERE s.CurrentAcademicYear = 'Alumni' 
      AND s.GraduationYear <= 2020
    ORDER BY GraduationYear DESC;
END;
GO
