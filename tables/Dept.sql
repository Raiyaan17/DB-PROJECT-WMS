-- DEPARTMENT
CREATE TABLE Dept.Department (
    DepartmentID VARCHAR(30) NOT NULL PRIMARY KEY,
    DepartmentName VARCHAR(50) NOT NULL
);
GO

CREATE TABLE Dept.Courses (
    CourseCode VARCHAR(10) NOT NULL PRIMARY KEY,
    DepartmentID VARCHAR(30) NOT NULL,

    CONSTRAINT fk_c_dept
        FOREIGN KEY (DepartmentID) REFERENCES Dept.Department(DepartmentID)
);
GO

-- DEGREE CORE COURSES (Department <-> Course)
CREATE TABLE Dept.DegreeCoreCourse (
    DepartmentID VARCHAR(30) NOT NULL,
    CourseCode VARCHAR(10) NOT NULL,
    CONSTRAINT pk_dcc PRIMARY KEY (DepartmentID, CourseCode), -- Composite key
    CONSTRAINT fk_dcc_degree
        FOREIGN KEY (DepartmentID) REFERENCES Dept.Department(DepartmentID),
    CONSTRAINT fk_dcc_course
        FOREIGN KEY (CourseCode) REFERENCES Course.Course(CourseCode)
);
GO

-- DEGREE ELECTIVE COURSES (Department <-> Course)
CREATE TABLE Dept.DegreeElectiveCourse (
    DepartmentID VARCHAR(30) NOT NULL,
    CourseCode VARCHAR(10) NOT NULL,
    CONSTRAINT pk_dec PRIMARY KEY (DepartmentID, CourseCode), -- Composite key
    CONSTRAINT fk_dec_degree
        FOREIGN KEY (DepartmentID) REFERENCES Dept.Department(DepartmentID),
    CONSTRAINT fk_dec_course
        FOREIGN KEY (CourseCode) REFERENCES Course.Course(CourseCode)
);
GO
