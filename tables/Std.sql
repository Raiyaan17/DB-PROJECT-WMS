-- StudentID helper table
-- StudentID = year_joined + DepartmentID + last_number;
-- e.g. '2023CS1', '2023CS2', '2024MGS1', '2021ACF13'
CREATE TABLE Std.StudentIdSequence (
    YearJoined SMALLINT    NOT NULL,
    DepartmentID VARCHAR(30) NOT NULL,
    LastNumber INT         NOT NULL,
    CONSTRAINT pk_StudentIdSequence
        PRIMARY KEY (YearJoined, DepartmentID),
    CONSTRAINT fk_sis_department
        FOREIGN KEY (DepartmentID) REFERENCES Dept.Department(DepartmentID)
);
GO

-- ENROLLMENTS (Student <-> Course)
-- this will also keep track of each students' course histories --
CREATE TABLE Std.Enrollment (
    StudentID             VARCHAR(30) NOT NULL,
    GraduationYear        SMALLINT  NOT NULL,
    CourseCode            VARCHAR(10) NOT NULL,
    Completed             BIT NOT NULL DEFAULT 0,

    CONSTRAINT pk_enrollment
        PRIMARY KEY (StudentID, CourseCode),

    CONSTRAINT fk_enrollment_student
        FOREIGN KEY (GraduationYear, StudentID)  REFERENCES Std.Student(GraduationYear, StudentID),
    CONSTRAINT fk_enrollment_course
        FOREIGN KEY (CourseCode) REFERENCES Course.Course(CourseCode)
);
GO
