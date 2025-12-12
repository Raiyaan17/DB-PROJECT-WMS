USE WheresMyScheduleDB;
GO

-- ENROLLMENTS (Student <-> Course)
-- this will also keep track of each students' course histories --
CREATE TABLE Std.Enrollment (
    StudentID         VARCHAR(30) NOT NULL,
    GraduationYear    SMALLINT    NOT NULL,
    CourseCode        VARCHAR(10) NOT NULL,
    Completed         BIT         NOT NULL DEFAULT 0,
    IsForced          BIT         NOT NULL DEFAULT 0,
    EnrollmentDate    DATETIME    NOT NULL DEFAULT GETDATE(),

    CONSTRAINT pk_enrollment
        PRIMARY KEY (StudentID, CourseCode),

    CONSTRAINT fk_enrollment_student
        FOREIGN KEY (GraduationYear, StudentID)  REFERENCES Std.Student(GraduationYear, StudentID),
    CONSTRAINT fk_enrollment_course
        FOREIGN KEY (CourseCode) REFERENCES Course.Course(CourseCode)
);
GO
