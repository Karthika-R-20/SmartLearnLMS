using System;
using System.Collections.Generic;

public class HybridCourse : Course
{
    // ==================== PROPERTIES ====================

    public int         OnlineVideoDuration { get; set; }
    public string      RoomNumber          { get; set; }
    public string      Building            { get; set; }
    public List<string> InPersonSessions   { get; set; }

   // ==================== MAX STUDENTS PROPERTY ====================

    private int _maxStudents;
    public int MaxStudents
    {
        get { return _maxStudents; }
        set
        {
            if (value < 0)
            {
                Console.WriteLine(" ✗ Max students must be greater than 0");
                return;
            }
            _maxStudents = value;
        }
    }
    // ==================== CONSTRUCTOR ====================

    public HybridCourse(int courseId, string courseName, string description,
                        string instructorName, string category,
                        int maxStudents, int onlineVideoDuration,
                        string roomNumber, string building)
        : base(courseId, courseName, description, instructorName, category)
    {
        MaxStudents         = maxStudents;
        OnlineVideoDuration = onlineVideoDuration;
        RoomNumber          = roomNumber;
        Building            = building;
        InPersonSessions    = new List<string>();
    }

    // ==================== ABSTRACT METHOD IMPLEMENTATIONS ====================

    // Limited by physical room capacity — same logic as InPersonCourse
    public override bool CanEnroll(Student student)
    {
        return CurrentEnrollments < MaxStudents;
    }

    // Physical seats remaining
    public override int GetAvailableSeats()
    {
        return MaxStudents - CurrentEnrollments;
    }

    public override string GetCourseType()
    {
        return "Hybrid";
    }

    public override void DisplayCourseInfo()
    {
        double hours        = (double)OnlineVideoDuration / 60;
        double avgRating    = GetAverageRating();
        int    totalRatings = GetTotalRatings();

        string ratingLine = totalRatings > 0 ? $"{avgRating:0.0}★ ({totalRatings} reviews)" : "No ratings yet";

        string status = CanEnroll(null) ? "Open for Enrollment" : "Full - Enrollment Closed";

        Console.WriteLine("╔═══════════════════════════════════════╗");
        Console.WriteLine("║         HYBRID: " + CourseName.PadRight(22) + "║");
        Console.WriteLine("╚═══════════════════════════════════════╝");
        Console.WriteLine("  Course ID        : " + CourseId);
        Console.WriteLine("  Instructor       : " + InstructorName);
        Console.WriteLine("  Category         : " + Category);
        Console.WriteLine("  Online Component : " + OnlineVideoDuration + " minutes (" + hours.ToString("0.#") + " hours)");
        Console.WriteLine("  In-Person        : " + Building + ", Room " + RoomNumber);
        Console.WriteLine("  Capacity         : " + CurrentEnrollments + "/" + MaxStudents);
        Console.WriteLine("  Seats Remaining  : " + GetAvailableSeats());
        Console.WriteLine("  Rating           : " + ratingLine);
        Console.WriteLine("  Status           : " + status);

        // Show scheduled in-person sessions if any have been added
        if (InPersonSessions.Count > 0)
        {
            Console.WriteLine("  Sessions         :");
            foreach (string session in InPersonSessions)
                Console.WriteLine("    - " + session);
        }
    }
}