USE WheresMyScheduleDB;
GO

-- This table tracks students who are waiting to enroll in a course that is currently full.
-- The WaitlistTimestamp ensures a first-in, first-out (FIFO) queue.
CREATE TABLE Course.Waitlist (
    CourseCode        VARCHAR(10) NOT NULL,
    StudentID         VARCHAR(30) NOT NULL,
    GraduationYear    SMALLINT    NOT NULL,
    WaitlistTimestamp DATETIME    NOT NULL DEFAULT GETDATE(),

    CONSTRAINT pk_waitlist PRIMARY KEY (CourseCode, StudentID),

    CONSTRAINT fk_waitlist_course
        FOREIGN KEY (CourseCode) REFERENCES Course.Course(CourseCode)
        ON DELETE CASCADE, -- If a course is deleted, its waitlist is cleared.

    CONSTRAINT fk_waitlist_student
        FOREIGN KEY (GraduationYear, StudentID) REFERENCES Std.Student(GraduationYear, StudentID)
        ON DELETE CASCADE -- If a student is deleted, they are removed from all waitlists.
);
GO
