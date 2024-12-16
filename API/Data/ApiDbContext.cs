using API.Models;

using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Secretary> Secretaries { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<ExamRequest> ExamRequests { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Modifications> Modifications { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<LabHolders> LabHolders { get; set; }
        public DbSet<ExamRequestRooms> ExamRequestRooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Specialization>()
               .HasOne(u => u.Faculty)
               .WithMany()
               .HasForeignKey(u => u.FacultyID)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Faculty)
                .WithMany()
                .HasForeignKey(u => u.FacultyID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Faculty)
                .WithMany()
                .HasForeignKey(u => u.FacultyID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Group)
                .WithMany()
                .HasForeignKey(s => s.GroupID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Secretary>()
                .HasOne(sc => sc.User)
                .WithMany()
                .HasForeignKey(sc => sc.UserID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Secretary>()
               .HasOne(sc => sc.Specialization)
               .WithMany()
               .HasForeignKey(sc => sc.SpecializationID)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Professor>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Professor>()
                .HasOne(p => p.Department)
                .WithMany()
                .HasForeignKey(p => p.DepartmentID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Modifications>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Group>()
                .HasOne(g => g.Specialization)
                .WithMany()
                .HasForeignKey(g => g.SpecializationID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Professor)
                .WithMany()
                .HasForeignKey(c => c.ProfessorID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Specialization)
                .WithMany()
                .HasForeignKey(c => c.SpecializationID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ExamRequest>()
                .HasOne(er => er.Course)
                .WithMany()
                .HasForeignKey(er => er.CourseID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExamRequest>()
                .HasOne(er => er.Group)
                .WithMany()
                .HasForeignKey(er => er.GroupID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ExamRequest>()
                .HasOne(er => er.Assistant)
                .WithMany()
                .HasForeignKey(er => er.AssistantID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ExamRequest>()
                .HasOne(er => er.Session)
                .WithMany()
                .HasForeignKey(er => er.SessionID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<LabHolders>()
                .HasKey(lh => lh.Id);


            modelBuilder.Entity<LabHolders>()
                .HasOne(lh => lh.Course)
                .WithMany()
                .HasForeignKey(lh => lh.CourseID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LabHolders>()
                .HasOne(lh => lh.Professor)
                .WithMany()  // Assuming one professor can hold many lab courses
                .HasForeignKey(lh => lh.ProfessorID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LabHolders>()
                .HasOne(lh => lh.Course)
                .WithMany()  // Assuming each course can have multiple lab holders
                .HasForeignKey(lh => lh.CourseID)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ExamRequestRooms>()
                .HasKey(er => new { er.ExamRequestID, er.RoomID });

            modelBuilder.Entity<ExamRequestRooms>()
                .HasOne(er => er.ExamRequest)
                .WithMany()
                .HasForeignKey(er => er.ExamRequestID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExamRequestRooms>()
                .HasOne(er => er.Room)
                .WithMany()
                .HasForeignKey(er => er.RoomID)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
