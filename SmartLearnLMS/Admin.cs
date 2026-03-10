using System;
using System.Collections.Generic;

public class Admin : User
{
    public bool CanManageUsers { get; set; }
    public bool CanManageCourses { get; set; }

    public Admin(string username, string password, string email)
        : base(username, password, email, "Admin")
    {
        CanManageUsers = true;
        CanManageCourses = true;
    }

    public void DisplayPermissions()
    {
        Console.WriteLine("==============================");
        Console.WriteLine("   Admin Permissions         ");
        Console.WriteLine("  Username : " + Username);
        Console.WriteLine("==============================");
        Console.WriteLine("  Can Manage Users   : " + (CanManageUsers ? "Yes" : "No"));
        Console.WriteLine("  Can Manage Courses : " + (CanManageCourses ? "Yes" : "No"));
        Console.WriteLine("==============================");
    }

    public void ViewAllUsers(List<User> users)
    {
        if (!CanManageUsers)
        {
            Console.WriteLine("❌ Access Denied. You do not have permission to manage users.");
            return;
        }

        Console.WriteLine("==============================");
        Console.WriteLine("        All Users            ");
        Console.WriteLine("  Total : " + users.Count + " user(s)");
        Console.WriteLine("==============================");

        if (users.Count == 0)
        {
            Console.WriteLine("  No users found in the system.");
            Console.WriteLine("==============================");
            return;
        }

        for (int i = 0; i < users.Count; i++)
        {
            User user = users[i];
            string type = user is Student ? "Student" :
                          user is Instructor ? "Instructor" : "Admin";

            Console.WriteLine("  " + (i + 1) + ". " + user.Username + " [" + type + "]");
            Console.WriteLine("       Email    : " + user.Email);
            Console.WriteLine("       Active   : " + (user.IsActive ? "Yes" : "No"));
            Console.WriteLine("  ------------------------------");
        }

        int studentCount = users.FindAll(u => u is Student).Count;
        int instructorCount = users.FindAll(u => u is Instructor).Count;
        int adminCount = users.FindAll(u => u is Admin).Count;

        Console.WriteLine("  Students    : " + studentCount);
        Console.WriteLine("  Instructors : " + instructorCount);
        Console.WriteLine("  Admins      : " + adminCount);
        Console.WriteLine("==============================");
    }

    public void DeactivateUser(User user)
    {
        if (!CanManageUsers)
        {
            Console.WriteLine("❌ Access Denied. You do not have permission to manage users.");
            return;
        }

        if (user.Username == Username)
        {
            Console.WriteLine("❌ You cannot deactivate your own account.");
            return;
        }

        if (!user.IsActive)
        {
            Console.WriteLine("⚠ User '" + user.Username + "' is already deactivated.");
            return;
        }

        user.IsActive = false;
        Console.WriteLine("✓ User '" + user.Username + "' has been deactivated successfully.");
        Console.WriteLine("  Role : " + user.Role);
    }

    // TASK 4.3: Enhanced GetSystemStats with completion rate, popular courses, inactive count
    public void GetSystemStats(List<User> users, List<Course> courses, List<Enrollment> enrollments)
    {
        if (!CanManageUsers && !CanManageCourses)
        {
            Console.WriteLine("❌ Access Denied.");
            return;
        }

        // ── User counts ──────────────────────────────────────────
        int totalUsers = users.Count;
        int studentCount = users.FindAll(u => u is Student).Count;
        int instructorCount = users.FindAll(u => u is Instructor).Count;
        int adminCount = users.FindAll(u => u is Admin).Count;
        int activeUsers = users.FindAll(u => u.IsActive == true).Count;
        int inactiveUsers = users.FindAll(u => u.IsActive == false).Count;

        // ── Course counts ─────────────────────────────────────────
        int totalCourses = courses.Count;

        // ── Students with 100% completion on ALL their courses ────
        int fullCompletionCount = 0;
        foreach (User user in users)
        {
            if (user is Student student && student.EnrolledCourseIds.Count > 0)
            {
                List<int> completed = student.GetCompletedCourses();
                if (completed.Count == student.EnrolledCourseIds.Count)
                    fullCompletionCount++;
            }
        }

        // ── Most and least popular course by CurrentEnrollments ───
        Course mostPopular = null;
        Course leastPopular = null;

        foreach (Course course in courses)
        {
            if (mostPopular == null || course.CurrentEnrollments > mostPopular.CurrentEnrollments)
                mostPopular = course;

            if (leastPopular == null || course.CurrentEnrollments < leastPopular.CurrentEnrollments)
                leastPopular = course;
        }

        // ── Average progress across all enrollments ───────────────
        int totalProgress = 0;
        foreach (Enrollment e in enrollments)
            totalProgress += e.ProgressPercentage;
        double avgProgress = enrollments.Count > 0
            ? (double)totalProgress / enrollments.Count : 0;

        // ── Display ───────────────────────────────────────────────
        Console.WriteLine("==============================");
        Console.WriteLine("      System Statistics      ");
        Console.WriteLine("==============================");
        Console.WriteLine("  ── Users ──────────────────");
        Console.WriteLine("  Total Users       : " + totalUsers);
        Console.WriteLine("  Students          : " + studentCount);
        Console.WriteLine("  Instructors       : " + instructorCount);
        Console.WriteLine("  Admins            : " + adminCount);
        Console.WriteLine("  Active Users      : " + activeUsers);
        Console.WriteLine("  Inactive Users    : " + inactiveUsers);
        Console.WriteLine("  ── Courses ─────────────────");
        Console.WriteLine("  Total Courses     : " + totalCourses);
        Console.WriteLine("  Total Enrollments : " + enrollments.Count);
        Console.WriteLine("  Avg Progress      : " + avgProgress.ToString("0.00") + "%");
        Console.WriteLine("  ── Completion ──────────────");
        Console.WriteLine("  100% Complete     : " + fullCompletionCount + " student(s)");
        Console.WriteLine("  ── Popularity ──────────────");

        if (mostPopular != null)
            Console.WriteLine("  Most Popular      : " + mostPopular.CourseName +
                              " (" + mostPopular.CurrentEnrollments + " enrolled)");

        if (leastPopular != null && leastPopular != mostPopular)
            Console.WriteLine("  Least Popular     : " + leastPopular.CourseName +
                              " (" + leastPopular.CurrentEnrollments + " enrolled)");

        Console.WriteLine("==============================");
    }

    // Original 2-parameter overload kept for backward compatibility
    public void GetSystemStats(List<User> users, List<Course> courses)
    {
        GetSystemStats(users, courses, new List<Enrollment>());
    }
}