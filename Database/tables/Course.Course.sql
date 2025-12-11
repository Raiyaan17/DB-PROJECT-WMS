USE WheresMyScheduleDB;
GO

CREATE TABLE Course.Course (
    CourseCode        VARCHAR(10)  NOT NULL PRIMARY KEY, -- e.g. 'CS100'
    CourseTitle       VARCHAR(100) NOT NULL,
    TotalCredits      TINYINT      NOT NULL,
    Capacity          SMALLINT     NOT NULL,
    Venue             VARCHAR(50)  NULL,
    DayOfWeek         VARCHAR(10)  NULL,
    StartTime         TIME         NULL,
    EndTime           TIME         NULL,
    EnrolledCount     INT          NOT NULL DEFAULT 0,
    RemainingSeats    AS (Capacity - EnrolledCount),
    IsActive          BIT          NOT NULL DEFAULT 1
);
GO
