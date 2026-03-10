using System;
using System.Collections.Generic;

public class Instructor : User
{
    public List<int> CourseIds { get; set; }

    public Instructor(string username, string password, string email)
        : base(username, password, email, "Instructor")
    {
        CourseIds = new List<int>();
    }

    public void AddCourse(int courseId)
    {
        if (CourseIds.Contains(courseId))
        {
            Console.WriteLine("Course ID " + courseId + " is already assigned to " + Username + ".");
            return;
        }
        CourseIds.Add(courseId);
        Console.WriteLine("Course ID " + courseId + " successfully assigned to " + Username + ".");
    }

    public void RemoveCourse(int courseId)
    {
        if (!CourseIds.Contains(courseId))
        {
            Console.WriteLine("Course ID " + courseId + " is not assigned to " + Username + ".");
            return;
        }
        CourseIds.Remove(courseId);
        Console.WriteLine("Course ID " + courseId + " removed from " + Username + "'s courses.");
    }

    // TASK 4.2: Enhanced to show title, student count, and average progress per course
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
            List<Enrollment> courseEnrollments = enrollments.FindAll(
                e => e.CourseId == courseId);

            // Calculate average progress across all enrolled students
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
                Console.WriteLine("       Max Students     : " + course.MaxStudents);
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

    // Original ShowMyCourses kept — shows IDs only, no extra data needed
    public void ShowMyCourses()
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

        for (int i = 0; i < CourseIds.Count; i++)
            Console.WriteLine("  " + (i + 1) + ". Course ID : " + CourseIds[i]);

        Console.WriteLine("------------------------------");
        Console.WriteLine("  Total Courses : " + CourseIds.Count);
        Console.WriteLine("==============================");
    }

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

    public Course GetCourseById(int id, List<Course> courses)
    {
        if (!CourseIds.Contains(id))
        {
            Console.WriteLine("Course ID " + id + " is not assigned to " + Username + ".");
            return null;
        }

        Course foundCourse = courses.Find(c => c.CourseId == id);
        if (foundCourse == null)
        {
            Console.WriteLine("Course ID " + id + " not found in the courses list.");
            return null;
        }

        return foundCourse;
    }
}