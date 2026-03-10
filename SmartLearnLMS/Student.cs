using System;
using System.Collections.Generic;

public class Student : User
{
    public List<int> EnrolledCourseIds { get; set; }
    public Dictionary<int, int> CourseProgress { get; set; }

    public Student(string username, string password, string email)
        : base(username, password, email, "Student")
    {
        EnrolledCourseIds = new List<int>();
        CourseProgress = new Dictionary<int, int>();
    }

    public void EnrollInCourse(int courseId)
    {
        if (!EnrolledCourseIds.Contains(courseId))
        {
            EnrolledCourseIds.Add(courseId);
            CourseProgress[courseId] = 0;
            Console.WriteLine("✓ Successfully enrolled in Course ID: " + courseId);
        }
        else
        {
            Console.WriteLine("❌ Already enrolled in this course!");
        }
    }

    public void UpdateProgress(int courseId, int percentage)
    {
        if (CourseProgress.ContainsKey(courseId))
        {
            CourseProgress[courseId] = percentage;
            Console.WriteLine("✓ Progress updated to " + percentage + "% for Course ID: " + courseId);
        }
        else
        {
            Console.WriteLine("❌ Not enrolled in this course.");
        }
    }

    // TASK 4.1: Modified to accept courses and enrollments lists
    // Shows enrolled courses with category, instructor, days since enrollment, and warnings
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

            // Find matching Enrollment object for enrollment date
            Enrollment enrollment = enrollments.Find(e =>
                e.StudentUsername == Username && e.CourseId == courseId);

            // Calculate days since enrollment
            int daysSinceEnrollment = 0;
            if (enrollment != null)
                daysSinceEnrollment = (int)(DateTime.Now - enrollment.EnrollmentDate).TotalDays;

            // Display course header
            Console.WriteLine("  Course ID   : " + courseId);

            if (course != null)
            {
                Console.WriteLine("  Title       : " + course.CourseName);
                Console.WriteLine("  Category    : " + course.Category);
                Console.WriteLine("  Instructor  : " + course.InstructorName);
            }

            Console.WriteLine("  Progress    : " + progress + "%");
            Console.WriteLine("  Enrolled    : " + daysSinceEnrollment + " day(s) ago");

            // Warning: enrolled > 30 days but progress < 50%
            if (daysSinceEnrollment > 30 && progress < 50)
            {
                Console.WriteLine("  ⚠ WARNING   : Enrolled over 30 days with less than 50% progress!");
            }

            Console.WriteLine("  ------------------------------");
        }
    }

    // Original overload kept — calls new version with empty lists for backward compatibility
    public void ShowEnrolledCourses()
    {
        if (EnrolledCourseIds.Count == 0)
        {
            Console.WriteLine("  No enrolled courses.");
            return;
        }

        foreach (int courseId in EnrolledCourseIds)
        {
            int progress = CourseProgress[courseId];
            Console.WriteLine("  Course ID : " + courseId + " → " + progress + "% completed");
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
        {
            Console.WriteLine("  Not enrolled in any courses.");
            return 0;
        }

        int totalProgress = 0;
        foreach (int courseId in EnrolledCourseIds)
            totalProgress += CourseProgress[courseId];

        return (double)totalProgress / EnrolledCourseIds.Count;
    }

    public void DropCourse(int courseId)
    {
        if (!EnrolledCourseIds.Contains(courseId))
        {
            Console.WriteLine("❌ Cannot drop. Not enrolled in Course ID: " + courseId);
            return;
        }

        EnrolledCourseIds.Remove(courseId);
        CourseProgress.Remove(courseId);
        Console.WriteLine("✓ Successfully dropped Course ID: " + courseId);
    }
}