using System;
using System.Collections.Generic;

public class Instructor : User, INotifiable
{
    // ==================== PROPERTIES ====================

    public List<int> CourseIds { get; set; }
    public string Department {get; set;}

    // ==================== PRIVATE FIELDS ====================
    // Private — only accessible through SendNotification() and GetNotificationHistory()
    private List<string> notifications = new List<string>();

    // ==================== CONSTRUCTOR ====================

    public Instructor(string username, string password, string email)
        : base(username, password, email, "Instructor")
    {
        CourseIds = new List<int>();
        Department = "Computer Science";
    }

     // ==================== INOTIFIABLE IMPLEMENTATION ====================
 
    // Adds a timestamped message to the private list and prints it to the console
    public void SendNotification(string message)
    {
        // Timestamp format: dd-MM-yyyy HH:mm e.g. 02-04-2026 14:35
        string timestamp  = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
        string fullEntry  = $"[{timestamp}] {message}";
 
        // Store in private list
        notifications.Add(fullEntry);
 
        // Display immediately with a bell icon
        Console.WriteLine("  🔔 " + fullEntry);
    }
 
    // Returns a copy of the list — caller cannot modify the private original
    public List<string> GetNotificationHistory()
    {
        return new List<string>(notifications);
    }

    // ==================== OVERRIDE ====================

        public override string GetUserType()
    {
        return "Instructor";
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();   // prints Username, Email, Role, Active, DateRegistered
        Console.WriteLine(" Department: " + Department);
        Console.WriteLine("  Courses Teaching: " + CourseIds.Count);
    }

    // ==================== METHODS ====================

    public void AddCourse(int courseId)
    {
        if (CourseIds.Contains(courseId))
        {
            Console.WriteLine("  Course ID " + courseId + " is already assigned to " + Username + ".");
            return;
        }
        CourseIds.Add(courseId);
        Console.WriteLine("  ✔ Course ID " + courseId + " successfully assigned to " + Username + ".");
    }

    public void RemoveCourse(int courseId)
    {
        if (!CourseIds.Contains(courseId))
        {
            Console.WriteLine("  Course ID " + courseId + " is not assigned to " + Username + ".");
            return;
        }
        CourseIds.Remove(courseId);
        Console.WriteLine("  ✔ Course ID " + courseId + " removed from " + Username + "'s courses.");
    }

    // FIX 7: removed redundant no-param overload — all callers use this version
    // Shows course title, students enrolled, and average progress per course
    public void ShowMyCourses(List<Course> courses, List<Enrollment> enrollments)
    {
        Console.WriteLine("==============================");
        Console.WriteLine("  Courses for : " + Username);
        Console.WriteLine("==============================");

        if (CourseIds.Count == 0)
        {
            Console.WriteLine("  No courses assigned yet.");
            Console.WriteLine("==============================");
            return;
        }

        int displayNumber = 1;
        foreach (int courseId in CourseIds)
        {
            // Find full Course object for title and details
            Course course = courses.Find(c => c.CourseId == courseId);

            // Get all enrollments for this specific course
            List<Enrollment> courseEnrollments = enrollments.FindAll(e => e.CourseId == courseId);

            // Calculate average progress across enrolled students
            double avgProgress = 0;
            if (courseEnrollments.Count > 0)
            {
                int total = 0;
                foreach (Enrollment e in courseEnrollments)
                    total += e.ProgressPercentage;
                avgProgress = (double)total / courseEnrollments.Count;
            }

            Console.WriteLine("  " + displayNumber + ". " +
                (course != null ? course.CourseName : "Course ID " + courseId));

            if (course != null)
            {
                Console.WriteLine("       Category         : " + course.Category);
            }

            Console.WriteLine("       Students Enrolled: " + courseEnrollments.Count);
            Console.WriteLine("       Average Progress  : " + avgProgress.ToString("0.00") + "%");
            Console.WriteLine("  ------------------------------");
            displayNumber++;
        }

        Console.WriteLine("  Total Courses  : " + CourseIds.Count);
        Console.WriteLine("  Total Students : " + GetStudentCount(enrollments));
        Console.WriteLine("==============================");
    }

    // Counts unique students enrolled across all this instructor's courses
    public int GetStudentCount(List<Enrollment> enrollments)
    {
        List<string> uniqueStudents = new List<string>();
        foreach (Enrollment enrollment in enrollments)
        {
            if (CourseIds.Contains(enrollment.CourseId))
            {
                if (!uniqueStudents.Contains(enrollment.StudentUsername))
                    uniqueStudents.Add(enrollment.StudentUsername);
            }
        }
        return uniqueStudents.Count;
    }

    // FIX 8: removed internal Console.WriteLine — pure lookup, caller handles messaging
    // Returns null if course not found or not assigned to this instructor
    public Course GetCourseById(int id, List<Course> courses)
    {
        if (!CourseIds.Contains(id))
            return null;

        return courses.Find(c => c.CourseId == id);
    }

    public void NotifyEnrollment(string studentUsername, int courseId)
    {
        SendNotification($"Student '{studentUsername}' has enrolled in your Course ID {courseId}.");
    }
    public override void DisplayDashboard()
    {
        Console.WriteLine("╔════════════════════════════════╗");
        Console.WriteLine("║      INSTRUCTOR DASHBOARD         ║");
        Console.WriteLine("╚════════════════════════════════╝");

        Console.WriteLine($"Welcome, Professor {Username}!");
        Console.WriteLine($"Department : {Department}");
        Console.WriteLine($"Teaching {CourseIds.Count} Courses");

        Console.WriteLine();
        Console.WriteLine("[1] My Courses");
        Console.WriteLine("[2] Create New Course");
        Console.WriteLine("[3] View Student Roster");
        Console.WriteLine("[4] CourseIds.Count");
        Console.WriteLine("[5] Logout");
    }
}