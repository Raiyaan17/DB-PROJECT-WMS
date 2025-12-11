-- write a select query to output 10 latest students by graduation year with all their info
SELECT TOP 10 * FROM Std.Student ORDER BY GraduationYear DESC, StudentId DESC;

-- show the in cart courses
SELECT * FROM Std.CourseCart;

-- return courses
SELECT * FROM Course.Course;

-- print waitlist
SELECT * FROM Course.Waitlist;

-- print enrolled
SELECT * FROM Std.Enrollment;

SELECT CourseCode, Capacity, EnrolledCount, RemainingSeats FROM Course.Course WHERE 
     CourseCode IN ('ACCT100', 'BIO101', 'CHEM101', 'CLCA1000', 'CS100');