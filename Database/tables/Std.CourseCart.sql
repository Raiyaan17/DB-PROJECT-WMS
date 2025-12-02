USE WheresMyScheduleDB;
GO

-- This table stores the courses a student has added to their cart but has not yet enrolled in.
-- It acts as a temporary holding area before the enrollment is finalized.
CREATE TABLE Std.CourseCart (
    StudentID         VARCHAR(30) NOT NULL,
    GraduationYear    SMALLINT    NOT NULL,
    CourseCode        VARCHAR(10) NOT NULL,

    CONSTRAINT pk_coursecart PRIMARY KEY (StudentID, CourseCode),

    CONSTRAINT fk_cart_student
        FOREIGN KEY (GraduationYear, StudentID) REFERENCES Std.Student(GraduationYear, StudentID)
        ON DELETE CASCADE, -- If a student is deleted, their cart is cleared.

    CONSTRAINT fk_cart_course
        FOREIGN KEY (CourseCode) REFERENCES Course.Course(CourseCode)
        ON DELETE CASCADE -- If a course is deleted, it's removed from all carts.
);
GO
