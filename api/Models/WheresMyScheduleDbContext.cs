using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace api.Models;

public partial class WheresMyScheduleDbContext : DbContext
{
    public WheresMyScheduleDbContext()
    {
    }

    public WheresMyScheduleDbContext(DbContextOptions<WheresMyScheduleDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<DepartmentCourse> DepartmentCourses { get; set; }

    public virtual DbSet<CourseCart> CourseCarts { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<Instructor> Instructors { get; set; }

    public virtual DbSet<InstructorIdSequence> InstructorIdSequences { get; set; }

    public virtual DbSet<School> Schools { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentIdSequence> StudentIdSequences { get; set; }

    public virtual DbSet<VDepartmentStudentCount> VDepartmentStudentCounts { get; set; }

    public virtual DbSet<VwAdminDashboardStat> VwAdminDashboardStats { get; set; }

    public virtual DbSet<VwAvailableCourse> VwAvailableCourses { get; set; }

    public virtual DbSet<VwFullPrerequisiteList> VwFullPrerequisiteLists { get; set; }

    public virtual DbSet<VwStudentSchedule> VwStudentSchedules { get; set; }

    public virtual DbSet<VwStudentTranscript> VwStudentTranscripts { get; set; }

    public virtual DbSet<Waitlist> Waitlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost,1433;Database=WheresMyScheduleDB;User Id=sa;Password=Raiyaan123!@#;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("PK__AuditLog__A17F23B830222700");

            entity.ToTable("AuditLog", "Std");

            entity.Property(e => e.AuditId).HasColumnName("AuditID");
            entity.Property(e => e.ActionDescription)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.AdminId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("AdminID");
            entity.Property(e => e.CourseCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StudentId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("StudentID");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseCode).HasName("PK__Course__FC00E0010A20F89D");

            entity.ToTable("Course", "Course");

            entity.Property(e => e.CourseCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CourseTitle)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DayOfWeek)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RemainingSeats).HasDefaultValue(-1);
            entity.Property(e => e.Venue)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasMany(d => d.IsPrerequisiteFor).WithMany(p => p.Prerequisites)
                .UsingEntity<Dictionary<string, object>>(
                    "CoursePrerequisite",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseCode")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_cp_course"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("PrerequisiteCode")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_cp_prereq"),
                    j =>
                    {
                        j.HasKey("CourseCode", "PrerequisiteCode").HasName("pk_cp");
                        j.ToTable("CoursePrerequisite", "Course");
                        j.IndexerProperty<string>("CourseCode")
                            .HasMaxLength(10)
                            .IsUnicode(false);
                        j.IndexerProperty<string>("PrerequisiteCode")
                            .HasMaxLength(10)
                            .IsUnicode(false);
                    });

            entity.HasMany(d => d.Instructors).WithMany(p => p.TaughtCourses)
                .UsingEntity<Dictionary<string, object>>(
                    "TeachingAssignment",
                    r => r.HasOne<Instructor>().WithMany()
                        .HasForeignKey("InstructorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_ta_instructor"),
                    l => l.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseCode")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_ta_course"),
                    j =>
                    {
                        j.HasKey("CourseCode", "InstructorId").HasName("pk_teachingassignment");
                        j.ToTable("TeachingAssignment", "Inst");
                        j.IndexerProperty<string>("CourseCode")
                            .HasMaxLength(10)
                            .IsUnicode(false);
                        j.IndexerProperty<string>("InstructorId")
                            .HasMaxLength(30)
                            .IsUnicode(false)
                            .HasColumnName("InstructorID");
                    });


        });

        modelBuilder.Entity<DepartmentCourse>(entity =>
        {
            entity.HasKey(e => e.CourseCode).HasName("PK__Courses__FC00E00111F08F3C");

            entity.ToTable("Courses", "Dept");

            entity.Property(e => e.CourseCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.DepartmentId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("DepartmentID");

            entity.HasOne(d => d.Department).WithMany(p => p.DepartmentCourses)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_c_dept");
        });

        modelBuilder.Entity<CourseCart>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.CourseCode }).HasName("pk_coursecart");

            entity.ToTable("CourseCart", "Std");

            entity.Property(e => e.StudentId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("StudentID");
            entity.Property(e => e.CourseCode)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.CourseCodeNavigation).WithMany(p => p.CourseCarts)
                .HasForeignKey(d => d.CourseCode)
                .HasConstraintName("fk_cart_course");

            entity.HasOne(d => d.Student).WithMany(p => p.CourseCarts)
                .HasForeignKey(d => new { d.GraduationYear, d.StudentId })
                .HasConstraintName("fk_cart_student");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BCDDC63EF08");

            entity.ToTable("Department", "Dept");

            entity.Property(e => e.DepartmentId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("DepartmentID");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasMany(d => d.CoreCourses).WithMany(p => p.CoreForDepartments)
                .UsingEntity<Dictionary<string, object>>(
                    "DegreeCoreCourse",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseCode")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_dcc_course"),
                    l => l.HasOne<Department>().WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_dcc_degree"),
                    j =>
                    {
                        j.HasKey("DepartmentId", "CourseCode").HasName("pk_dcc");
                        j.ToTable("DegreeCoreCourse", "Dept");
                        j.IndexerProperty<string>("DepartmentId")
                            .HasMaxLength(30)
                            .IsUnicode(false)
                            .HasColumnName("DepartmentID");
                        j.IndexerProperty<string>("CourseCode")
                            .HasMaxLength(10)
                            .IsUnicode(false);
                    });

            entity.HasMany(d => d.ElectiveCourses).WithMany(p => p.ElectiveForDepartments)
                .UsingEntity<Dictionary<string, object>>(
                    "DegreeElectiveCourse",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseCode")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_dec_course"),
                    l => l.HasOne<Department>().WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_dec_degree"),
                    j =>
                    {
                        j.HasKey("DepartmentId", "CourseCode").HasName("pk_dec");
                        j.ToTable("DegreeElectiveCourse", "Dept");
                        j.IndexerProperty<string>("DepartmentId")
                            .HasMaxLength(30)
                            .IsUnicode(false)
                            .HasColumnName("DepartmentID");
                        j.IndexerProperty<string>("CourseCode")
                            .HasMaxLength(10)
                            .IsUnicode(false);
                    });
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.CourseCode }).HasName("pk_enrollment");

            entity.ToTable("Enrollment", "Std", tb =>
                {
                    tb.HasTrigger("trg_CheckCreditLimit");
                    tb.HasTrigger("trg_PreventAlumniEnrollment");
                });

            entity.Property(e => e.StudentId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("StudentID");
            entity.Property(e => e.CourseCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.EnrollmentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.CourseCodeNavigation).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.CourseCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_enrollment_course");

            entity.HasOne(d => d.Student).WithMany(p => p.Enrollments)
                .HasForeignKey(d => new { d.GraduationYear, d.StudentId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_enrollment_student");
        });

        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.HasKey(e => e.InstructorId).HasName("PK__Instruct__9D010B7B5A6085E2");

            entity.ToTable("Instructor", "Inst");

            entity.HasIndex(e => e.Email, "UQ__Instruct__A9D10534DD9AFB4B").IsUnique();

            entity.Property(e => e.InstructorId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("InstructorID");
            entity.Property(e => e.DepartmentId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("DepartmentID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Fname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FName");
            entity.Property(e => e.Lname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LName");

            entity.HasOne(d => d.Department).WithMany(p => p.Instructors)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_instructor_dept");
        });

        modelBuilder.Entity<InstructorIdSequence>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Instruct__B2079BCD3038BDC9");

            entity.ToTable("InstructorIdSequence", "Inst");

            entity.Property(e => e.DepartmentId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("DepartmentID");

            entity.HasOne(d => d.Department).WithOne(p => p.InstructorIdSequence)
                .HasForeignKey<InstructorIdSequence>(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_iis_department");
        });

        modelBuilder.Entity<School>(entity =>
        {
            entity.HasKey(e => e.SchoolId).HasName("PK__School__3DA4677BABE55A29");

            entity.ToTable("School", "School");

            entity.Property(e => e.SchoolId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("SchoolID");
            entity.Property(e => e.SchoolName)
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.HasMany(d => d.Departments).WithMany(p => p.Schools)
                .UsingEntity<Dictionary<string, object>>(
                    "Department1",
                    r => r.HasOne<Department>().WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_sd_department"),
                    l => l.HasOne<School>().WithMany()
                        .HasForeignKey("SchoolId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_sd_school"),
                    j =>
                    {
                        j.HasKey("SchoolId", "DepartmentId").HasName("pk_schooldepartment");
                        j.ToTable("Department", "School");
                        j.IndexerProperty<string>("SchoolId")
                            .HasMaxLength(30)
                            .IsUnicode(false)
                            .HasColumnName("SchoolID");
                        j.IndexerProperty<string>("DepartmentId")
                            .HasMaxLength(30)
                            .IsUnicode(false)
                            .HasColumnName("DepartmentID");
                    });
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => new { e.GraduationYear, e.StudentId }).HasName("pk_student");

            entity.ToTable("Student", "Std");

            entity.HasIndex(e => new { e.Email, e.GraduationYear }, "uq_student_email").IsUnique();

            entity.Property(e => e.StudentId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("StudentID");
            entity.Property(e => e.CurrentAcademicYear)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.DepartmentId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("DepartmentID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Fname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FName");
            entity.Property(e => e.Lname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LName");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.SchoolId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("SchoolID");

            entity.HasOne(d => d.Department).WithMany(p => p.Students)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_student_department");

            entity.HasOne(d => d.School).WithMany(p => p.Students)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_student_school");
        });

        modelBuilder.Entity<StudentIdSequence>(entity =>
        {
            entity.HasKey(e => new { e.YearJoined, e.DepartmentId }).HasName("pk_StudentIdSequence");

            entity.ToTable("StudentIdSequence", "Std");

            entity.Property(e => e.DepartmentId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("DepartmentID");

            entity.HasOne(d => d.Department).WithMany(p => p.StudentIdSequences)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sis_department");
        });

        modelBuilder.Entity<VDepartmentStudentCount>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_DepartmentStudentCounts", "Analytics");

            entity.Property(e => e.DepartmentId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("DepartmentID");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwAdminDashboardStat>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_AdminDashboardStats", "School");

            entity.Property(e => e.DepartmentName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwAvailableCourse>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_AvailableCourses", "Course");

            entity.Property(e => e.CourseCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CourseTitle)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DayOfWeek)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Venue)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwFullPrerequisiteList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_FullPrerequisiteList", "Course");

            entity.Property(e => e.CourseCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CourseTitle)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PrerequisiteCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.PrerequisiteTitle)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwStudentSchedule>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_StudentSchedule", "Std");

            entity.Property(e => e.CourseCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CourseTitle)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DayOfWeek)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StudentId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("StudentID");
            entity.Property(e => e.Venue)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwStudentTranscript>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_StudentTranscript", "Std");

            entity.Property(e => e.CourseCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CourseTitle)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EnrollmentDate).HasColumnType("datetime");
            entity.Property(e => e.StudentId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("StudentID");
        });

        modelBuilder.Entity<Waitlist>(entity =>
        {
            entity.HasKey(e => new { e.CourseCode, e.StudentId }).HasName("pk_waitlist");

            entity.ToTable("Waitlist", "Course");

            entity.Property(e => e.CourseCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.StudentId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("StudentID");
            entity.Property(e => e.WaitlistTimestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.CourseCodeNavigation).WithMany(p => p.Waitlists)
                .HasForeignKey(d => d.CourseCode)
                .HasConstraintName("fk_waitlist_course");

            entity.HasOne(d => d.Student).WithMany(p => p.Waitlists)
                .HasForeignKey(d => new { d.GraduationYear, d.StudentId })
                .HasConstraintName("fk_waitlist_student");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
