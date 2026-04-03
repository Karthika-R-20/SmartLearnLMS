using System;

public class InPersonCourse : Course
{
    // ==================== PROPERTIES ====================
    public string RoomNumber  { get; set; }
    public string Building    { get; set; }

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

    public InPersonCourse(int courseId, string courseName, string description,
                          string instructorName, string category,
                          int maxStudents, string roomNumber, string building)
        : base(courseId, courseName, description, instructorName, category)
    {
        MaxStudents = maxStudents;
        RoomNumber  = roomNumber;
        Building    = building;
    }

    // ==================== ABSTRACT METHOD IMPLEMENTATIONS ====================

    // Has a seat limit — only allow if space is available
    public override bool CanEnroll(Student student)
    {
        return CurrentEnrollments < MaxStudents;
    }

    // Physical seats remaining in the room
    public override int GetAvailableSeats()
    {
        return MaxStudents - CurrentEnrollments;
    }

    public override string GetCourseType()
    {
        return "In-Person";
    }

    public override void DisplayCourseInfo()
    {
        double avgRating    = GetAverageRating();
        int    totalRatings = GetTotalRatings();

        string ratingLine  = totalRatings > 0 ? $"{avgRating:0.0}★ ({totalRatings} reviews)" : "No ratings yet";

        string status = CanEnroll(null) ? "Open for Enrollment" : "Full - Enrollment Closed";

        Console.WriteLine("╔═══════════════════════════════════════╗");
        Console.WriteLine("║ IN-PERSON: " + CourseName.PadRight(21) + "║");
        Console.WriteLine("╚═══════════════════════════════════════╝");
        Console.WriteLine("  Course ID      : " + CourseId);
        Console.WriteLine("  Instructor     : " + InstructorName);
        Console.WriteLine("  Category       : " + Category);
        Console.WriteLine("  Location       : " + Building + ", Room " + RoomNumber);
        Console.WriteLine("  Capacity       : " + CurrentEnrollments + "/" + MaxStudents);
        Console.WriteLine("  Seats Remaining: " + GetAvailableSeats());
        Console.WriteLine("  Rating         : " + ratingLine);
        Console.WriteLine("  Status         : " + status);
    }
}