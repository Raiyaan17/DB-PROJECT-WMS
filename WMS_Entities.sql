-- DEPARTMENT
CREATE TABLE Department (
    DepartmentID VARCHAR(30) NOT NULL PRIMARY KEY -- e.g. CS, MGS, ACF, ANTHRO, SOCIO, LAW, PHYS, CHEM
);
-- COURSES
CREATE TABLE Course (
    CourseCode   VARCHAR(10) NOT NULL PRIMARY KEY,      -- e.g. 'CS100'
    Department   VARCHAR(30) NOT NULL,
    title        VARCHAR(100) NOT NULL,
    credits      TINYINT     NOT NULL,
    level        INT         NULL,                      -- e.g. 100, 200, 300
    capacity     INT         NOT NULL,
    CONSTRAINT fk_course_department
        FOREIGN KEY (Department) REFERENCES Department(DepartmentID)
);

-- STUDENTS
CREATE TABLE Student (
    StudentID             VARCHAR(30)  NOT NULL PRIMARY KEY,     -- e.g. '2023CS1'
    fname                 VARCHAR(50)  NOT NULL,
    lname                 VARCHAR(50)  NOT NULL,
    email                 VARCHAR(100) NOT NULL UNIQUE,
    year_joined           SMALLINT     NOT NULL,                 -- store year, e.g. 2023
    department            VARCHAR(30)  NOT NULL,
    current_academic_year VARCHAR(10)  NULL,                     -- FRESHMAN/SOPH/JUNIOR/SENIOR
    CONSTRAINT fk_student_department
        FOREIGN KEY (department) REFERENCES Department(DepartmentID),
    CONSTRAINT chk_student_acad_year
        CHECK (
            current_academic_year IN ('FRESHMAN','SOPHOMORE','JUNIOR','SENIOR')
            OR current_academic_year IS NULL
        )
);

-- StudentID helper table
-- StudentID = year_joined + DepartmentID + last_number;
-- e.g. '2023CS1', '2023CS2', '2024MGS1', '2021ACF13'
CREATE TABLE StudentIdSequence (
    YearJoined SMALLINT    NOT NULL,
    Department VARCHAR(30) NOT NULL,
    LastNumber INT         NOT NULL,
    CONSTRAINT pk_StudentIdSequence
        PRIMARY KEY (YearJoined, Department),
    CONSTRAINT fk_sis_department
        FOREIGN KEY (Department) REFERENCES Department(DepartmentID)
);

-- INSTRUCTORS
CREATE TABLE Instructor (
    InstructorID VARCHAR(30) NOT NULL PRIMARY KEY, -- eg. CS1, MGS10
    fname        VARCHAR(50) NOT NULL,
    lname        VARCHAR(50) NOT NULL,
    email        VARCHAR(100) NOT NULL UNIQUE,     -- eg. fname.lname.InstructorID@lums.edu.pk
    Department   VARCHAR(30) NOT NULL,
    CONSTRAINT fk_instructor_dept
        FOREIGN KEY (Department) REFERENCES Department(DepartmentID)
);

-- InstructorID helper table
CREATE TABLE InstructorIdSequence (
    Department VARCHAR(30) NOT NULL PRIMARY KEY,
    LastNumber INT         NOT NULL,
    CONSTRAINT fk_iis_department
        FOREIGN KEY (Department) REFERENCES Department(DepartmentID)
);

-- ENROLLMENTS (Student <-> Course)
-- this will also keep track of each students' course histories --
CREATE TABLE Enrollment (
    StudentID   VARCHAR(30) NOT NULL,
    CourseCode  VARCHAR(10) NOT NULL,
    Completed   BIT NOT NULL DEFAULT 0,

    CONSTRAINT pk_enrollment
        PRIMARY KEY (StudentID, CourseCode),

    CONSTRAINT fk_enrollment_student
        FOREIGN KEY (StudentID)  REFERENCES Student(StudentID),
    CONSTRAINT fk_enrollment_course
        FOREIGN KEY (CourseCode) REFERENCES Course(CourseCode)
);

-- TEACHING ASSIGNMENTS (Instructor <-> Course)
CREATE TABLE TeachingAssignment (
    InstructorID VARCHAR(30) NOT NULL,
    CourseCode   VARCHAR(10) NOT NULL,

    CONSTRAINT pk_teachingassignment
        PRIMARY KEY (InstructorID, CourseCode),

    CONSTRAINT fk_ta_instructor
        FOREIGN KEY (InstructorID) REFERENCES Instructor(InstructorID),
    CONSTRAINT fk_ta_course
        FOREIGN KEY (CourseCode)   REFERENCES Course(CourseCode)
);

-- COURSE PREREQUISITES (Course <-> Course)
-- prevent students from enrolling in a course if they haven't completed its pre-req course --
CREATE TABLE CoursePrerequisite (
    CourseCode       VARCHAR(10) NOT NULL,
    PrerequisiteCode VARCHAR(10) NOT NULL,
    CONSTRAINT pk_cp PRIMARY KEY (CourseCode, PrerequisiteCode), -- Composite key
    CONSTRAINT fk_cp_course
        FOREIGN KEY (CourseCode)       REFERENCES Course(CourseCode),
    CONSTRAINT fk_cp_prereq
        FOREIGN KEY (PrerequisiteCode) REFERENCES Course(CourseCode)
);

-- The CORE and ELECTIVE courses tables will help implement rule checks when students select courses 👇 --
-- DEGREE CORE COURSES (Department <-> Course)
CREATE TABLE DegreeCoreCourse (
    Department VARCHAR(30) NOT NULL,
    CourseCode VARCHAR(10) NOT NULL,
    CONSTRAINT pk_dcc PRIMARY KEY (Department, CourseCode), -- Composite key
    CONSTRAINT fk_dcc_degree
        FOREIGN KEY (Department) REFERENCES Department(DepartmentID),
    CONSTRAINT fk_dcc_course
        FOREIGN KEY (CourseCode) REFERENCES Course(CourseCode)
);

-- DEGREE ELECTIVE COURSES (Department <-> Course)
CREATE TABLE DegreeElectiveCourse (
    Department VARCHAR(30) NOT NULL,
    CourseCode VARCHAR(10) NOT NULL,
    CONSTRAINT pk_dec PRIMARY KEY (Department, CourseCode), -- Composite key
    CONSTRAINT fk_dec_degree
        FOREIGN KEY (Department) REFERENCES Department(DepartmentID),
    CONSTRAINT fk_dec_course
        FOREIGN KEY (CourseCode) REFERENCES Course(CourseCode)
);

-- Rules for student course enrollment:
--  1. enroll in min 3 and max 5 courses 
--  2. max 3 core courses and remaining any electives