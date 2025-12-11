-- =============================================
-- Description: Adds a course to a student's cart with validation.
-- =============================================
CREATE PROCEDURE Std.sp_AddToCart
    @studentId VARCHAR(30),
    @courseCode VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Check if the student exists
    IF NOT EXISTS (SELECT 1 FROM Std.Student WHERE StudentID = @studentId)
    BEGIN
        RAISERROR('Student not found.', 16, 1);
        RETURN;
    END

    -- 2. Check if the course exists and is active
    IF NOT EXISTS (SELECT 1 FROM Course.Course WHERE CourseCode = @courseCode AND IsActive = 1)
    BEGIN
        RAISERROR('Course not found or is not active.', 16, 1);
        RETURN;
    END

    -- 3. Check if the student is already enrolled in the course (and not completed)
    IF EXISTS (SELECT 1 FROM Std.Enrollment WHERE StudentID = @studentId AND CourseCode = @courseCode AND Completed = 0)
    BEGIN
        RAISERROR('Student is already enrolled in this course.', 16, 1);
        RETURN;
    END

    -- 4. Check if the course is already in the cart
    IF EXISTS (SELECT 1 FROM Std.CourseCart WHERE StudentID = @studentId AND CourseCode = @courseCode)
    BEGIN
        RAISERROR('Course is already in the cart.', 16, 1);
        RETURN;
    END

    -- 5. Get student's graduation year
    DECLARE @gradYear INT;
    SELECT @gradYear = GraduationYear FROM Std.Student WHERE StudentID = @studentId;

    -- 6. Add to cart
    INSERT INTO Std.CourseCart (StudentID, CourseCode, GraduationYear)
    VALUES (@studentId, @courseCode, @gradYear);

    PRINT 'Course ' + @courseCode + ' added to cart for student ' + @studentId;
END
GO
