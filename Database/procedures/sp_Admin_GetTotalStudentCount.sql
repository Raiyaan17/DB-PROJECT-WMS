-- =============================================
-- Description: Get Total Count of Enrolled Students (Freshman-Senior)
-- =============================================
CREATE OR ALTER PROCEDURE sp_Admin_GetTotalStudentCount
    @Count INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT @Count = COUNT(*)
    FROM Std.Student s
    WHERE UPPER(s.CurrentAcademicYear) IN ('FRESHMAN', 'SOPHOMORE', 'JUNIOR', 'SENIOR');
END;
GO
