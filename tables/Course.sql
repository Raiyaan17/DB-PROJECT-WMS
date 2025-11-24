-- COURSES
CREATE TABLE Course.Course (
    CourseCode   VARCHAR(10) NOT NULL PRIMARY KEY,      -- e.g. 'CS100'
    CourseTitle        VARCHAR(100) NOT NULL,
    TotalCredits      TINYINT     NOT NULL,
    Capacity          SMALLINT NOT NULL
);
GO

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
GO
