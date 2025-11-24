-- INSTRUCTORS
CREATE TABLE Inst.Instructor (
    InstructorID VARCHAR(30) NOT NULL PRIMARY KEY, -- eg. CS1, MGS10
    FName        VARCHAR(50) NOT NULL,
    LName        VARCHAR(50) NOT NULL,
    Email        VARCHAR(100) NOT NULL UNIQUE,     -- eg. fname.lname.InstructorID@lums.edu.pk
    DepartmentID   VARCHAR(30) NOT NULL,
    CONSTRAINT fk_instructor_dept
        FOREIGN KEY (DepartmentID) REFERENCES Dept.Department(DepartmentID)
);
GO

-- InstructorID helper table
CREATE TABLE Inst.InstructorIdSequence (
    DepartmentID VARCHAR(30) NOT NULL PRIMARY KEY,
    LastNumber INT         NOT NULL,
    CONSTRAINT fk_iis_department
        FOREIGN KEY (DepartmentID) REFERENCES Dept.Department(DepartmentID)
);
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
