CREATE DATABASE WheresMyScheduleDB;
GO

CREATE SCHEMA Std;
GO
CREATE SCHEMA Inst;
GO
CREATE SCHEMA Dept;
GO
CREATE SCHEMA Course;
GO

-- STATIC Tables --
-- Dept.Department : stores names and ids of university departments (Computer Science, CS)
-- School : stores names and ids of schools (eg: Suleman Dawood School of Business, SDSB)

-- DEPARTMENT
CREATE TABLE Dept.Department (
    DepartmentID VARCHAR(30) NOT NULL PRIMARY KEY,
    DepartmentName VARCHAR(50) NOT NULL,
);

-- SCHOOL
CREATE TABLE School (
    SchoolID VARCHAR(30) NOT NULL PRIMARY KEY,
    SchoolName VARCHAR(300) NOT NULL,
);


-- DYNAMIC Tables --
-- Course.Course : stores course information (eg: code, title, credits)
-- Std.Student : stores student information
-- Inst.Instructor : stores instructor information

-- Std.StudentIdSequence : helper table for auto-incrementing student IDs
-- Inst.InstructorIdSequence : helper table for auto-incrementing instructor IDs

-- Std.Enrollment : maps students to the courses they are enrolled in
-- Inst.TeachingAssignment : maps instructors to the courses they are teaching

-- Course.CoursePrerequisite : maps courses to their prerequisites

-- Dept.Courses : maps courses to their respective departments
-- Dept.DegreeCoreCourse : maps departments to their core courses
-- Dept.DegreeElectiveCourse : maps departments to their elective courses

-- COURSES
CREATE TABLE Course.Course (
    CourseCode   VARCHAR(10) NOT NULL PRIMARY KEY,      -- e.g. 'CS100'
    CourseTitle        VARCHAR(100) NOT NULL,
    TotalCredits      TINYINT     NOT NULL,
    PreRequisites VARCHAR(10), 
);

-- STUDENTS
CREATE TABLE Std.Student (
    StudentID             VARCHAR(30)  NOT NULL PRIMARY KEY,     -- e.g. '2023CS1'
    fname                 VARCHAR(50)  NOT NULL,
    lname                 VARCHAR(50)  NOT NULL,
    email                 VARCHAR(100) NOT NULL UNIQUE,
    year_joined           SMALLINT     NOT NULL,                 -- store year, e.g. 2023
    School                VARCHAR(15)  NOT NULL,
    Department            VARCHAR(30)  NOT NULL,
    current_academic_year VARCHAR(10)  NULL,                     -- FRESHMAN/SOPH/JUNIOR/SENIOR
    CONSTRAINT fk_student_department
        FOREIGN KEY (Department) REFERENCES Dept.Department(DepartmentID),
    CONSTRAINT chk_student_acad_year
        CHECK (
            current_academic_year IN ('FRESHMAN','SOPHOMORE','JUNIOR','SENIOR')
            OR current_academic_year IS NULL
        )
);

-- StudentID helper table
-- StudentID = year_joined + DepartmentID + last_number;
-- e.g. '2023CS1', '2023CS2', '2024MGS1', '2021ACF13'
CREATE TABLE Std.StudentIdSequence (
    YearJoined SMALLINT    NOT NULL,
    Department VARCHAR(30) NOT NULL,
    LastNumber INT         NOT NULL,
    CONSTRAINT pk_StudentIdSequence
        PRIMARY KEY (YearJoined, Department),
    CONSTRAINT fk_sis_department
        FOREIGN KEY (Department) REFERENCES Dept.Department(DepartmentID)
);

-- INSTRUCTORS
CREATE TABLE Inst.Instructor (
    InstructorID VARCHAR(30) NOT NULL PRIMARY KEY, -- eg. CS1, MGS10
    fname        VARCHAR(50) NOT NULL,
    lname        VARCHAR(50) NOT NULL,
    email        VARCHAR(100) NOT NULL UNIQUE,     -- eg. fname.lname.InstructorID@lums.edu.pk
    Department   VARCHAR(30) NOT NULL,
    CONSTRAINT fk_instructor_dept
        FOREIGN KEY (Department) REFERENCES Dept.Department(DepartmentID)
);

-- InstructorID helper table
CREATE TABLE Inst.InstructorIdSequence (
    Department VARCHAR(30) NOT NULL PRIMARY KEY,
    LastNumber INT         NOT NULL,
    CONSTRAINT fk_iis_department
        FOREIGN KEY (Department) REFERENCES Dept.Department(DepartmentID)
);

-- ENROLLMENTS (Student <-> Course)
-- this will also keep track of each students' course histories --
CREATE TABLE Std.Enrollment (
    StudentID   VARCHAR(30) NOT NULL,
    CourseCode  VARCHAR(10) NOT NULL,
    Completed   BIT NOT NULL DEFAULT 0,

    CONSTRAINT pk_enrollment
        PRIMARY KEY (StudentID, CourseCode),

    CONSTRAINT fk_enrollment_student
        FOREIGN KEY (StudentID)  REFERENCES Std.Student(StudentID),
    CONSTRAINT fk_enrollment_course
        FOREIGN KEY (CourseCode) REFERENCES Course.Course(CourseCode)
);

-- TEACHING ASSIGNMENTS (Instructor <-> Course)
CREATE TABLE Inst.TeachingAssignment (
    InstructorID VARCHAR(30) NOT NULL,
    CourseCode   VARCHAR(10) NOT NULL,

    CONSTRAINT pk_teachingassignment
        PRIMARY KEY (InstructorID, CourseCode),

    CONSTRAINT fk_ta_instructor
        FOREIGN KEY (InstructorID) REFERENCES Inst.Instructor(InstructorID),
    CONSTRAINT fk_ta_course
        FOREIGN KEY (CourseCode)   REFERENCES Course.Course(CourseCode)
);

-- COURSE PREREQUISITES (Course <-> Course)
-- prevent students from enrolling in a course if they haven't completed its pre-req course --
CREATE TABLE Course.CoursePrerequisite (
    CourseCode       VARCHAR(10) NOT NULL,
    PrerequisiteCode VARCHAR(10) NOT NULL,
    CONSTRAINT pk_cp PRIMARY KEY (CourseCode, PrerequisiteCode), -- Composite key
    CONSTRAINT fk_cp_course
        FOREIGN KEY (CourseCode)       REFERENCES Course.Course(CourseCode),
    CONSTRAINT fk_cp_prereq
        FOREIGN KEY (PrerequisiteCode) REFERENCES Course.Course(CourseCode)
);

CREATE TABLE Dept.Courses (
    CourseCode VARCHAR(10) NOT NULL PRIMARY KEY,
    Department VARCHAR(30) NOT NULL,

    CONSTRAINT fk_c_dept
        FOREIGN KEY (Department) REFERENCES Dept.Department(DepartmentID),
)

-- DEGREE CORE COURSES (Department <-> Course)
CREATE TABLE Dept.DegreeCoreCourse (
    Department VARCHAR(30) NOT NULL,
    CourseCode VARCHAR(10) NOT NULL,
    CONSTRAINT pk_dcc PRIMARY KEY (Department, CourseCode), -- Composite key
    CONSTRAINT fk_dcc_degree
        FOREIGN KEY (Department) REFERENCES Dept.Department(DepartmentID),
    CONSTRAINT fk_dcc_course
        FOREIGN KEY (CourseCode) REFERENCES Course.Course(CourseCode)
);

-- DEGREE ELECTIVE COURSES (Department <-> Course)
CREATE TABLE Dept.DegreeElectiveCourse (
    Department VARCHAR(30) NOT NULL,
    CourseCode VARCHAR(10) NOT NULL,
    CONSTRAINT pk_dec PRIMARY KEY (Department, CourseCode), -- Composite key
    CONSTRAINT fk_dec_degree
        FOREIGN KEY (Department) REFERENCES Dept.Department(DepartmentID),
    CONSTRAINT fk_dec_course
        FOREIGN KEY (CourseCode) REFERENCES Course.Course(CourseCode)
);
 