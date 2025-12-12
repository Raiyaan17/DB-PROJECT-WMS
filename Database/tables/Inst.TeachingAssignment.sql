USE WheresMyScheduleDB;
GO

-- TEACHING ASSIGNMENTS (Instructor <-> Course)
CREATE TABLE Inst.TeachingAssignment (
    CourseCode   VARCHAR(10) NOT NULL,
    InstructorID VARCHAR(30) NOT NULL,

    CONSTRAINT pk_teachingassignment
        PRIMARY KEY (CourseCode, InstructorID),

    CONSTRAINT fk_ta_instructor
        FOREIGN KEY (InstructorID) REFERENCES Inst.Instructor(InstructorID),
    CONSTRAINT fk_ta_course
        FOREIGN KEY (CourseCode)   REFERENCES Course.Course(CourseCode)
);
GO
