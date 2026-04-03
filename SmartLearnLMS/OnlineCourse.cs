using System;
using System.Collections.Generic;

public class OnlineCourse : Course
{
    // ==================== PROPERTIES ====================
    public int VideoDurationMinutes   { get; set; }
    // Auto-generated from CourseId — set in constructor after base() runs
    public string StreamingUrl         { get; set; }
 
    // ==================== CONSTRUCTOR ====================
 
    // Calls base Course constructor, then sets OnlineCourse-specific properties
    public OnlineCourse(int courseId, string courseName, string description,
                        string instructorName, string category,
                        int videoDurationMinutes)
        : base(courseId, courseName, description, instructorName, category)
    {
        VideoDurationMinutes = videoDurationMinutes;
 
        // CourseId is available here because base() already ran and set it
        StreamingUrl = "https://smartlearn.com/stream/" + CourseId;
    }
 
    // ==================== ABSTRACT METHOD IMPLEMENTATIONS ====================
 
    // Online courses have unlimited capacity — any student can always enroll
    public override bool CanEnroll(Student student)
    {
        return true;
    }
 
    // No seat limit — return int.MaxValue to represent unlimited
    public override int GetAvailableSeats()
    {
        return int.MaxValue;
    }
 
    // Identifies this course type throughout the system
    public override string GetCourseType()
    {
        return "Online";
    }
 
    // Displays full course info with box-drawing characters
    public override void DisplayCourseInfo()
    {
        double hours        = (double)VideoDurationMinutes / 60;
        double avgRating    = GetAverageRating();
        int    totalRatings = GetTotalRatings();
 
        string ratingLine  = totalRatings > 0 ? $"{avgRating:0.0}★ ({totalRatings} reviews)" : "No ratings yet";
 
        Console.WriteLine("╔═══════════════════════════════════════╗");
        Console.WriteLine("║ONLINE: " + CourseName.PadRight(23) + "║");
        Console.WriteLine("╚═══════════════════════════════════════╝");
        Console.WriteLine("  Course ID  : " + CourseId);
        Console.WriteLine("  Instructor : " + InstructorName);
        Console.WriteLine("  Category   : " + Category);
        Console.WriteLine("  Duration   : " + VideoDurationMinutes + " minutes (" + hours.ToString("0.#") + " hours)");
        Console.WriteLine("  Capacity   : Unlimited");
        Console.WriteLine("  Students   : " + CurrentEnrollments);
        Console.WriteLine("  Rating     : " + ratingLine);
        Console.WriteLine("  Status     : Open for Enrollment");
        Console.WriteLine("  Stream URL : " + StreamingUrl);
    }
}