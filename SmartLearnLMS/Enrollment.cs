using System;

public class Enrollment
{
    // ==================== PROPERTIES ====================

    public int EnrollmentId { get; set; }
    public string StudentUsername { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public int ProgressPercentage { get; set; }
    public bool IsCompleted { get; set; }

    // ==================== CONSTRUCTOR ====================

    public Enrollment(int enrollmentId, string studentUsername, int courseId)
    {
        EnrollmentId = enrollmentId;
        StudentUsername = studentUsername;
        CourseId = courseId;
        EnrollmentDate = DateTime.Now;  // automatically set to current date
        ProgressPercentage = 0;             // starts at 0% when first enrolled
        IsCompleted = false;         // not completed when first enrolled
    }

    // ==================== METHODS ====================

    // Updates the student's progress — validates percentage is between 0 and 100
    public void UpdateProgress(int percentage)
    {
        if (percentage < 0 || percentage > 100)
        {
            Console.WriteLine("Invalid percentage. Please enter a value between 0 and 100.");
            return;
        }

        ProgressPercentage = percentage;
        Console.WriteLine($"Progress updated to {ProgressPercentage}% for Enrollment ID: {EnrollmentId}");

        // Automatically mark complete if progress reaches 100%
        if (ProgressPercentage == 100)
        {
            MarkComplete();
        }
    }

    // Marks the enrollment as completed
    public void MarkComplete()
    {
        IsCompleted = true;
        Console.WriteLine($"Enrollment ID {EnrollmentId} marked as completed. Congratulations!");
    }

    // Displays all enrollment details
    public void DisplayInfo()
    {
        Console.WriteLine($"Enrollment ID  : {EnrollmentId}");
        Console.WriteLine($"Student        : {StudentUsername}");
        Console.WriteLine($"Course ID      : {CourseId}");
        Console.WriteLine($"Enrolled On    : {EnrollmentDate:dd-MM-yyyy HH:mm}");
        Console.WriteLine($"Progress       : {ProgressPercentage}%");
        Console.WriteLine($"Completed      : {(IsCompleted ? "Yes" : "No")}");
    }
}