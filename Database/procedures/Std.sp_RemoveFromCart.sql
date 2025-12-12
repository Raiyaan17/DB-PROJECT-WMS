-- =============================================
-- Description: Removes a course from a student's cart.
-- =============================================
CREATE PROCEDURE Std.sp_RemoveFromCart
    @studentId VARCHAR(30),
    @courseCode VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if the item exists in the cart before deleting
    IF NOT EXISTS (SELECT 1 FROM Std.CourseCart WHERE StudentID = @studentId AND CourseCode = @courseCode)
    BEGIN
        RAISERROR('Course not found in the student''s cart.', 16, 1);
        RETURN;
    END

    DELETE FROM Std.CourseCart
    WHERE StudentID = @studentId AND CourseCode = @courseCode;

    PRINT 'Course ' + @courseCode + ' removed from cart for student ' + @studentId;
END
GO
