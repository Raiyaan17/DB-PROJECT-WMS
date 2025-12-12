-- =============================================
-- Description: Get All Departments
-- =============================================
CREATE OR ALTER PROCEDURE sp_Admin_GetDepartments
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DepartmentID
    FROM Dept.Department
    ORDER BY DepartmentID;
END;
GO
