-- =============================================
-- Description: Validates a student's cart for prerequisites, time conflicts, and credit limits.
-- =============================================
CREATE PROCEDURE Std.sp_ValidateCart
    @studentId VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;

    -- Temporary table to hold validation errors
    CREATE TABLE #ValidationErrors (
        ErrorMessage NVARCHAR(255)
    );

    -- 1. Check if student exists
    IF NOT EXISTS (SELECT 1 FROM Std.Student WHERE StudentID = @studentId)
    BEGIN
        INSERT INTO #ValidationErrors (ErrorMessage) VALUES ('Student not found.');
        -- Go directly to the end to return errors
        GOTO Finalize;
    END

    -- Get all courses in the student's cart into a temp table
    SELECT CourseCode INTO #CartCourses FROM Std.CourseCart WHERE StudentID = @studentId;

    -- If cart is empty, there's nothing to validate.
    IF NOT EXISTS (SELECT 1 FROM #CartCourses)
    BEGIN
        GOTO Finalize;
    END

    -- 2. Prerequisite Check (Set-based)
    INSERT INTO #ValidationErrors (ErrorMessage)
    SELECT 'Prerequisites not met for course: ' + cc.CourseCode
    FROM #CartCourses cc
    WHERE Course.fn_CheckPrerequisiteStatus(@studentId, cc.CourseCode) = 0;

    -- 3. Time Overlap Check
    INSERT INTO #ValidationErrors (ErrorMessage)
    SELECT 
        'Time conflict between ' + ConflictingCourse1Code + ' and ' + ConflictingCourse2Code + ' on ' + ConflictDay + '.'
    FROM Course.fn_GetCartTimeConflicts(@studentId);

    -- 4. Credit Limit Check
    DECLARE @MaxCreditLimit INT = 20; -- This could be a configuration value
    DECLARE @cartCredits INT;
    DECLARE @enrolledCredits INT;

    SELECT @cartCredits = SUM(c.TotalCredits)
    FROM Course.Course c
    JOIN #CartCourses cc ON c.CourseCode = cc.CourseCode;

    SELECT @enrolledCredits = SUM(c.TotalCredits)
    FROM Course.Course c
    JOIN Std.Enrollment e ON c.CourseCode = e.CourseCode
    WHERE e.StudentID = @studentId AND e.Completed = 0;

    IF (ISNULL(@cartCredits, 0) + ISNULL(@enrolledCredits, 0) > @MaxCreditLimit)
    BEGIN
        INSERT INTO #ValidationErrors (ErrorMessage)
        VALUES ('Exceeds maximum credit limit of ' + CAST(@MaxCreditLimit AS VARCHAR) + '.');
    END

Finalize:
    -- Return all found errors
    SELECT ErrorMessage FROM #ValidationErrors;

    -- Cleanup
    IF OBJECT_ID('tempdb..#CartCourses') IS NOT NULL
        DROP TABLE #CartCourses;
    
    IF OBJECT_ID('tempdb..#ValidationErrors') IS NOT NULL
        DROP TABLE #ValidationErrors;
END
GO
