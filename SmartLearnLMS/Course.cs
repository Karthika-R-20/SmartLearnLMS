using System;
public class Course
{
    // ==================== PROPERTIES ====================

    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public string Description { get; set; }
    public string InstructorName { get; set; }
    public string Category { get; set; }
    public int MaxStudents { get; set; }
    public int CurrentEnrollments { get; set; }

    // ==================== CONSTRUCTOR ====================

    public Course(int courseId, string courseName, string description,
                  string instructorName, string category, int currentEnrollment)
    {
        CourseId = courseId;
        CourseName = courseName;
        Description = description;
        InstructorName = instructorName;
        Category = category;
        MaxStudents = 50;
        CurrentEnrollments = currentEnrollment;    // always starts at 0
    }

    // ==================== METHODS ====================

    public void DisplayCourseInfo()
    {
        Console.WriteLine($"Course ID    : {CourseId}");
        Console.WriteLine($"Course Name  : {CourseName}");
        Console.WriteLine($"Description  : {Description}");
        Console.WriteLine($"Instructor   : {InstructorName}");
        Console.WriteLine($"Category     : {Category}");
        Console.WriteLine($"Maximum No. of Students     : {MaxStudents}");
        Console.WriteLine($"Enrollments  : {CurrentEnrollments}");
    }

    // Only increase if space is available
    public void IncrementEnrollment()
    {
        if (CurrentEnrollments < MaxStudents)
        {
            CurrentEnrollments++;
            Console.WriteLine($"Enrollment successful. Students enrolled: {CurrentEnrollments}/{MaxStudents}");
        }
        else
        {
            Console.WriteLine($"Cannot enroll. Course '{CourseName}' is full ({MaxStudents}/{MaxStudents}).");
        }
    }

    // Only decrease if at least 1 student is enrolled
    public void DecrementEnrollment()
    {
        if (CurrentEnrollments > 0)
        {
            CurrentEnrollments--;
            Console.WriteLine($"Enrollment removed. Students enrolled: {CurrentEnrollments}/{MaxStudents}");
        }
        else
        {
            Console.WriteLine($"Cannot remove. No students are currently enrolled in '{CourseName}'.");
        }
    }
    // Returns true if the course can still accept new enrollments
    public bool CanEnroll()
    {
        return CurrentEnrollments < MaxStudents;
    }
}