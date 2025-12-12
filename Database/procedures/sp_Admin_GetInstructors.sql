-- =============================================
-- Description: Get Instructors (Optional Dept Filter)
-- =============================================
CREATE OR ALTER PROCEDURE sp_Admin_GetInstructors
    @DepartmentID VARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        InstructorID,
        FName,
        LName,
        Email,
        DepartmentID
    FROM Inst.Instructor
    WHERE (@DepartmentID IS NULL OR DepartmentID = @DepartmentID)
    ORDER BY InstructorID;
END;
GO
