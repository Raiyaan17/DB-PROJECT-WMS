USE WheresMyScheduleDB;
GO

CREATE OR ALTER PROCEDURE Course.sp_AddCourse
    @CourseCode   VARCHAR(10),
    @CourseTitle  VARCHAR(100),
    @TotalCredits TINYINT,
    @Capacity     SMALLINT,
    @DepartmentID VARCHAR(30),
    @InstructorID VARCHAR(30) = NULL,
    @Venue        VARCHAR(50) = NULL,
    @DayOfWeek    VARCHAR(10) = NULL,
    @StartTime    TIME = NULL,
    @EndTime      TIME = NULL,
    @IsActive     BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Course.Course WHERE CourseCode = @CourseCode)
        RAISERROR('Course already exists.', 16, 1);

    IF NOT EXISTS (SELECT 1 FROM Dept.Department WHERE DepartmentID = @DepartmentID)
        RAISERROR('Department does not exist.', 16, 1);

    IF @InstructorID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Inst.Instructor WHERE InstructorID = @InstructorID)
        RAISERROR('Instructor does not exist.', 16, 1);

    INSERT INTO Course.Course (CourseCode, CourseTitle, TotalCredits, Capacity, Venue, DayOfWeek, StartTime, EndTime, IsActive)
    VALUES (@CourseCode, @CourseTitle, @TotalCredits, @Capacity, @Venue, @DayOfWeek, @StartTime, @EndTime, @IsActive);

    INSERT INTO Dept.Courses (CourseCode, DepartmentID)
    VALUES (@CourseCode, @DepartmentID);

    IF @InstructorID IS NOT NULL
    BEGIN
        INSERT INTO Inst.TeachingAssignment (CourseCode, InstructorID)
        VALUES (@CourseCode, @InstructorID);
    END
END
GO
