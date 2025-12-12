-- =============================================
-- Description:   Returns all direct and indirect prerequisite courses for a given CourseCode.
-- Usage:         EXEC Course.usp_GetAllPrerequisites @CourseCode = 'CS300';
-- =============================================
IF OBJECT_ID('Course.usp_GetAllPrerequisites', 'P') IS NOT NULL
    DROP PROCEDURE Course.usp_GetAllPrerequisites;
GO

CREATE PROCEDURE Course.usp_GetAllPrerequisites
    @CourseCode VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    WITH RecursivePrerequisites (CourseCode, PrerequisiteCode, Level) AS (
        SELECT
            cp.CourseCode,
            cp.PrerequisiteCode,
            1 AS Level
        FROM Course.CoursePrerequisite cp
        WHERE cp.CourseCode = @CourseCode

        UNION ALL

        -- Recursive member: Find prerequisites of the prerequisites found so far
        SELECT
            rp.CourseCode,
            cp_inner.PrerequisiteCode,
            rp.Level + 1
        FROM RecursivePrerequisites rp
        JOIN Course.CoursePrerequisite cp_inner
            ON rp.PrerequisiteCode = cp_inner.CourseCode
    )
    SELECT DISTINCT PrerequisiteCode
    FROM RecursivePrerequisites
    OPTION (MAXRECURSION 100);
END;
GO


-- EXEC Course.usp_GetAllPrerequisites @CourseCode = 'ECON536';
