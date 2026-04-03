using System;
using System.Collections.Generic;

public class Student : User, ISearchable, INotifiable 
{
    // ==================== PROPERTIES ====================

    public List<int>            EnrolledCourseIds { get; set; }
    public Dictionary<int, int> CourseProgress    { get; set; }

    // ==================== PROGRESS PERCENTAGE PROPERTY ====================

    private int _progressPercentage;
    public int ProgressPercentage
    {
        get { return _progressPercentage; }
        set
        {
            if (value < 0 || value > 100)
            {
                Console.WriteLine("  ✗ Progress must be 0-100");
                return;
            }
            _progressPercentage = value;
        }
    }

    // ==================== PRIVATE FIELDS ====================
    // Private — only accessible through SendNotification() and GetNotificationHistory()
    private List<string> notifications = new List<string>();

    // ==================== CONSTRUCTOR ====================

    public Student(string username, string password, string email)
        : base(username, password, email, "Student")
    {
        EnrolledCourseIds = new List<int>();
        CourseProgress    = new Dictionary<int, int>();
    }

    // ==================== ISEARCHABLE IMPLEMENTATION ====================
 
    // Returns true if keyword matches Username or Email — case insensitive
    public bool MatchesSearch(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return false;
 
        string k = keyword.ToLower();
 
        return Username.ToLower().Contains(k) ||
               Email.ToLower().Contains(k);
    }
 
    // Returns a formatted one-line summary for search results
    public string GetSearchSummary()
    {
        return $"[Student] {Username} ({Email}) - {EnrolledCourseIds.Count} course(s)";
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
        return "Student";
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();   // prints Username, Email, Role, Active, DateRegistered
        Console.WriteLine("  Enrolled Courses: " + EnrolledCourseIds.Count);
        Console.WriteLine("  Avg Progress    : " + GetAverageProgress().ToString("0.0") + "%");
    }

    // ==================== METHODS ====================

    public void EnrollInCourse(int courseId)
    {
        if (!EnrolledCourseIds.Contains(courseId))
        {
            EnrolledCourseIds.Add(courseId);
            CourseProgress[courseId] = 0;
            Console.WriteLine("  ✔ Successfully enrolled in Course ID: " + courseId);
        }
        else
        {
            Console.WriteLine("  ❌ Already enrolled in this course!");
        }
    }

    // Overload: enrols the student AND notifies the course instructor
    public void EnrollInCourse(int courseId, Instructor instructor)
    {
        bool alreadyEnrolled = EnrolledCourseIds.Contains(courseId);

        // Reuse the base enrol logic
        EnrollInCourse(courseId);

        // Only notify when this is a new enrolment
        if (!alreadyEnrolled)
        {
            instructor.SendNotification(
                $"Student '{Username}' has enrolled in your Course ID {courseId}.");
        }
    }

    public void UpdateProgress(int courseId, int percentage)
    {
        if (!CourseProgress.ContainsKey(courseId))
        {
            Console.WriteLine("  ❌ Not enrolled in this course.");
            return;
        }

        // Run through validated property — invalid values are rejected with error message
        ProgressPercentage = percentage;

        // Only update dictionary if setter accepted the value
        if (ProgressPercentage == percentage)
        {
            CourseProgress[courseId] = ProgressPercentage;
            Console.WriteLine("  ✔ Progress updated to " + percentage + "% for Course ID: " + courseId);
        }
    }

    // Shows enrolled courses with category, instructor, days since enrollment, and 30-day warning
    public void ShowEnrolledCourses(List<Course> courses, List<Enrollment> enrollments)
    {
        if (EnrolledCourseIds.Count == 0)
        {
            Console.WriteLine("  No enrolled courses.");
            return;
        }

        foreach (int courseId in EnrolledCourseIds)
        {
            int progress = CourseProgress[courseId];

            // Find matching Course object for name, category, instructor
            Course course = courses.Find(c => c.CourseId == courseId);

            // Find matching Enrollment for enrollment date calculation
            Enrollment enrollment = enrollments.Find(e =>
                e.StudentUsername == Username && e.CourseId == courseId);

            // Calculate days since enrollment
            int daysSinceEnrollment = 0;
            if (enrollment != null)
                daysSinceEnrollment = (int)(DateTime.Now - enrollment.EnrollmentDate).TotalDays;

            Console.WriteLine("  Course ID   : " + courseId);

            if (course != null)
            {
                Console.WriteLine("  Title       : " + course.CourseName);
                Console.WriteLine("  Category    : " + course.Category);
                Console.WriteLine("  Instructor  : " + course.InstructorName);
            }

            Console.WriteLine("  Progress    : " + progress + "%");
            Console.WriteLine("  Enrolled    : " + daysSinceEnrollment + " day(s) ago");

            // FIX 4: warning when enrolled > 30 days but progress < 50%
            if (daysSinceEnrollment > 30 && progress < 50)
                Console.WriteLine("  ⚠ WARNING   : Enrolled over 30 days with less than 50% progress!");

            Console.WriteLine("  ------------------------------");
        }
    }

    public List<int> GetCompletedCourses()
    {
        List<int> completedCourses = new List<int>();
        foreach (int courseId in EnrolledCourseIds)
        {
            if (CourseProgress[courseId] == 100)
                completedCourses.Add(courseId);
        }
        return completedCourses;
    }

    public double GetAverageProgress()
    {
        if (EnrolledCourseIds.Count == 0)
            return 0;

        int totalProgress = 0;
        foreach (int courseId in EnrolledCourseIds)
            totalProgress += CourseProgress[courseId];

        return (double)totalProgress / EnrolledCourseIds.Count;
    }

    public void DropCourse(int courseId)
    {
        if (!EnrolledCourseIds.Contains(courseId))
        {
            Console.WriteLine("  ❌ Cannot drop. Not enrolled in Course ID: " + courseId);
            return;
        }

        EnrolledCourseIds.Remove(courseId);
        CourseProgress.Remove(courseId);
        Console.WriteLine("  ✔ Successfully dropped Course ID: " + courseId);
    }

    // ==================== DASHBOARD ====================

    // Displays a professional dashboard using box-drawing characters
    // Returns the menu choice so the caller (Program.cs) can act on it
    public override void DisplayDashboard()
    {
        Console.WriteLine("╔════════════════════════════════╗");
        Console.WriteLine("║      STUDENT DASHBOARD         ║");
        Console.WriteLine("╚════════════════════════════════╝");

        Console.WriteLine($"Welcome, {Username}!");
        Console.WriteLine($"Enrolled Courses: {EnrolledCourseIds.Count}");

        Console.WriteLine();
        Console.WriteLine("[1] Browse Available Courses");
        Console.WriteLine("[2] My Enrolled Courses");
        Console.WriteLine("[3] Update Progress");
        Console.WriteLine("[4] My Statistics");
        Console.WriteLine("[5] Logout");
    }
}