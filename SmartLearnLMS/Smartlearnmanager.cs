using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public static class SmartLearnManager
{
    // ==================== PART 1: DICTIONARY LOOKUPS ====================

    // Instant O(1) lookup — no looping through the whole list
    public static Dictionary<int, Course>  CourseById     = new Dictionary<int, Course>();
    public static Dictionary<string, User> UserByUsername = new Dictionary<string, User>();

    // Call this after every Add / Remove on either list so dictionaries stay in sync
    public static void RebuildDictionaries(List<User> users, List<Course> courses)
    {
        CourseById.Clear();
        foreach (Course c in courses)
            CourseById[c.CourseId] = c;

        UserByUsername.Clear();
        foreach (User u in users)
            UserByUsername[u.Username] = u;
    }

    // TryGetValue — safe, no exception if key is missing
    public static Course GetCourseById(int id)
    {
        CourseById.TryGetValue(id, out Course course);
        return course;
    }

    public static User GetUserByUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return null;
        UserByUsername.TryGetValue(username, out User user);
        return user;
    }

    // ==================== PART 2: LINQ SEARCH FEATURES ====================

    // Step 1 — Search courses in Title AND Description, sorted alphabetically
    public static List<Course> SearchCourses(List<Course> courses, string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return new List<Course>();

        string k = keyword.ToLower();

        return courses
            .Where(c => c.CourseName.ToLower().Contains(k) ||
                        c.Description.ToLower().Contains(k))
            .OrderBy(c => c.CourseName)
            .ToList();
    }

    // Step 2 — Search students by Username OR Email, sorted by username
    public static List<Student> SearchStudents(List<User> users, string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return new List<Student>();

        string k = keyword.ToLower();

        return users
            .OfType<Student>()
            .Where(s => s.Username.ToLower().Contains(k) ||
                        (s.Email != null && s.Email.ToLower().Contains(k)))
            .OrderBy(s => s.Username)
            .ToList();
    }

    // Step 3 — Filter courses by category, highest enrollment first
    public static List<Course> FilterCoursesByCategory(List<Course> courses, string category)
    {
        if (string.IsNullOrWhiteSpace(category)) return new List<Course>();

        return courses
            .Where(c => c.Category.ToLower() == category.ToLower())
            .OrderByDescending(c => c.CurrentEnrollments)
            .ToList();
    }

    // Step 4 — Search instructors by Username, Email, or Department
    public static List<Instructor> SearchInstructors(List<User> users, string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return new List<Instructor>();

        string k = keyword.ToLower();

        return users
            .OfType<Instructor>()
            .Where(i => i.Username.ToLower().Contains(k) ||
                        (i.Email != null && i.Email.ToLower().Contains(k)) ||
                        i.Department.ToLower().Contains(k))
            .OrderBy(i => i.Username)
            .ToList();
    }

    // ==================== PART 3: LINQ ANALYTICS — STUDENT ====================

    // Top N students by average progress (descending)
    public static List<Student> GetTopStudents(List<User> users, int count)
    {
        return users
            .OfType<Student>()
            .OrderByDescending(s => s.GetAverageProgress())
            .Take(count)
            .ToList();
    }

    // Students enrolled in at least 1 course
    public static List<Student> GetActiveStudents(List<User> users)
    {
        return users
            .OfType<Student>()
            .Where(s => s.EnrolledCourseIds.Count > 0)
            .ToList();
    }

    // Students grouped into Low / Medium / High performance buckets
    public static Dictionary<string, List<Student>> GetStudentsByPerformance(List<User> users)
    {
        var groups = new Dictionary<string, List<Student>>
        {
            { "High (80%+)",     new List<Student>() },
            { "Medium (50-79%)", new List<Student>() },
            { "Low (<50%)",      new List<Student>() }
        };

        foreach (Student s in users.OfType<Student>())
        {
            double avg = s.GetAverageProgress();
            if      (avg >= 80) groups["High (80%+)"].Add(s);
            else if (avg >= 50) groups["Medium (50-79%)"].Add(s);
            else                groups["Low (<50%)"].Add(s);
        }

        return groups;
    }

    // System-wide average progress across all students
    public static double CalculateSystemAverageProgress(List<User> users)
    {
        var students = users.OfType<Student>().ToList();
        if (students.Count == 0) return 0;
        return students.Average(s => s.GetAverageProgress());
    }

    // ==================== PART 3: LINQ ANALYTICS — COURSE ====================

    // Top N courses by enrollment count
    public static List<Course> GetPopularCourses(List<Course> courses, int count)
    {
        return courses
            .OrderByDescending(c => c.CurrentEnrollments)
            .Take(count)
            .ToList();
    }

    // Courses with fewer than 5 enrolled students — need promotion
    public static List<Course> GetCoursesNeedingStudents(List<Course> courses)
    {
        return courses
            .Where(c => c.CurrentEnrollments < 5)
            .OrderBy(c => c.CurrentEnrollments)
            .ToList();
    }

    // Top N courses by average star rating (only those with at least one rating)
    public static List<Course> GetHighestRatedCourses(List<Course> courses, int count)
    {
        return courses
            .Where(c => c.GetTotalRatings() > 0)
            .OrderByDescending(c => c.GetAverageRating())
            .Take(count)
            .ToList();
    }

    // All courses taught by an instructor — partial, case-insensitive match
    public static List<Course> GetCoursesByInstructor(List<Course> courses, string instructorName)
    {
        if (string.IsNullOrWhiteSpace(instructorName)) return new List<Course>();

        return courses
            .Where(c => c.InstructorName.ToLower().Contains(instructorName.ToLower()))
            .ToList();
    }

    // ==================== PART 3: LINQ ANALYTICS — SYSTEM-WIDE ====================

    // All courses in a category, sorted by average rating (highest first)
    public static List<Course> GetCoursesByCategory(List<Course> courses, string category)
    {
        return courses
            .Where(c => c.Category.ToLower() == category.ToLower())
            .OrderByDescending(c => c.GetAverageRating())
            .ToList();
    }

    // The category with the most total enrollments across all its courses
    public static string GetMostEnrolledCategory(List<Course> courses)
    {
        if (courses.Count == 0) return "None";

        return courses
            .GroupBy(c => c.Category)
            .OrderByDescending(g => g.Sum(c => c.CurrentEnrollments))
            .First().Key;
    }

    // True if any student has 100% progress in at least one course
    public static bool HasStudentCompletedAny(List<User> users)
    {
        return users.OfType<Student>().Any(s => s.GetCompletedCourses().Count > 0);
    }

    // True if every in-person and hybrid course is at maximum capacity
    public static bool AreAllCoursesFilled(List<Course> courses)
    {
        var limited = courses.Where(c => c is InPersonCourse || c is HybridCourse).ToList();
        if (limited.Count == 0) return false;
        return limited.All(c => c.GetAvailableSeats() == 0);
    }

    // ==================== ADVANCED LINQ — Task 1: Performance Levels ====================

    // GroupBy students into performance buckets, then print each group
    public static void GetStudentsByPerformanceLevel(List<User> users)
    {
        Console.WriteLine("==============================");
        Console.WriteLine("  Student Performance Levels ");
        Console.WriteLine("==============================");

        // GroupBy classifies each student; ordering puts High first alphabetically
        var groups = users
            .OfType<Student>()
            .GroupBy(s =>
                s.GetAverageProgress() >= 80 ? "High (80%+)" :
                s.GetAverageProgress() >= 50 ? "Medium (50-79%)" : "Low (<50%)")
            .OrderByDescending(g => g.Key);   // High > Medium > Low

        bool anyGroup = false;
        foreach (var group in groups)
        {
            anyGroup = true;
            Console.WriteLine($"\n  [{group.Key}] — {group.Count()} student(s)");
            foreach (Student s in group.OrderByDescending(st => st.GetAverageProgress()))
                Console.WriteLine($"    • {s.Username,-20} avg: {s.GetAverageProgress():0.0}%");
        }

        if (!anyGroup)
            Console.WriteLine("  No students in the system yet.");

        Console.WriteLine("==============================");
    }

    // ==================== ADVANCED LINQ — Task 2: Course Statistics Dashboard ====================

    // Uses Sum, Average, Min, Max, and GroupBy to build a statistics report
    public static void GetCourseStatistics(List<Course> courses)
    {
        if (courses.Count == 0)
        {
            Console.WriteLine("  No courses in the system.");
            return;
        }

        double avgEnrollment = courses.Average(c => c.CurrentEnrollments);
        int    maxEnrollment = courses.Max(c => c.CurrentEnrollments);
        int    minEnrollment = courses.Min(c => c.CurrentEnrollments);

        Course highest = courses.OrderByDescending(c => c.CurrentEnrollments).First();
        Course lowest  = courses.OrderBy(c => c.CurrentEnrollments).First();

        var byCategory = courses
            .GroupBy(c => c.Category)
            .OrderBy(g => g.Key);

        Console.WriteLine("╔════════════════════════════════╗");
        Console.WriteLine("║     Course Statistics          ║");
        Console.WriteLine("╚════════════════════════════════╝");
        Console.WriteLine($"  Total Courses    : {courses.Count}");
        Console.WriteLine($"  Avg Enrollment   : {avgEnrollment:0.00}");
        Console.WriteLine($"  Highest Enrolled : {highest.CourseName} ({maxEnrollment} students)");
        Console.WriteLine($"  Lowest Enrolled  : {lowest.CourseName} ({minEnrollment} students)");
        Console.WriteLine("\n  ── By Category ─────────────────");

        foreach (var g in byCategory)
        {
            int totalInCat = g.Sum(c => c.CurrentEnrollments);
            Console.WriteLine($"    {g.Key}: {g.Count()} course(s), {totalInCat} enrolled");
        }

        Console.WriteLine("==============================");
    }

    // ==================== ADVANCED LINQ — Task 3: Pagination ====================

    // Returns one page of courses using Skip() and Take(); prints page info
    public static List<Course> GetPaginatedCourses(List<Course> courses, int pageNumber, int pageSize)
    {
        int totalCourses = courses.Count;
        int totalPages   = (int)Math.Ceiling((double)totalCourses / pageSize);
        if (totalPages == 0) totalPages = 1;

        // Clamp page number to valid range
        pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));

        var page = courses
            .OrderBy(c => c.CourseName)                       // sort alphabetically first
            .Skip((pageNumber - 1) * pageSize)                // skip previous pages
            .Take(pageSize)                                   // take this page only
            .ToList();

        Console.WriteLine($"  Page {pageNumber} of {totalPages}  |  " +
                          $"Showing {page.Count} of {totalCourses} course(s)");
        return page;
    }

    // ==================== PART 4: DISPLAY METHODS ====================

    // Formatted table of course search / filter results
    public static void DisplaySearchResults(List<Course> results)
    {
        if (results.Count == 0)
        {
            Console.WriteLine("  No matching courses found.");
            return;
        }

        Console.WriteLine("==============================");
        Console.WriteLine($"  Found {results.Count} course(s):");
        Console.WriteLine("==============================");

        int i = 1;
        foreach (Course c in results)
        {
            string rating = c.GetTotalRatings() > 0
                ? $"{c.GetAverageRating():0.0}★ ({c.GetTotalRatings()} reviews)"
                : "No ratings yet";

            Console.WriteLine($"  {i++}. [{c.GetCourseType()}] {c.CourseName}");
            Console.WriteLine($"       Category   : {c.Category}");
            Console.WriteLine($"       Instructor : {c.InstructorName}");
            Console.WriteLine($"       Enrolled   : {c.CurrentEnrollments}");
            Console.WriteLine($"       Rating     : {rating}");
            Console.WriteLine("  ------------------------------");
        }
    }

    // Student list with key info — username, email, courses, progress
    public static void DisplayStudentList(List<Student> students)
    {
        if (students.Count == 0)
        {
            Console.WriteLine("  No students found.");
            return;
        }

        Console.WriteLine("==============================");
        Console.WriteLine($"  {students.Count} Student(s) Found:");
        Console.WriteLine("==============================");

        int i = 1;
        foreach (Student s in students)
        {
            Console.WriteLine($"  {i++}. {s.Username}");
            Console.WriteLine($"       Email    : {s.Email}");
            Console.WriteLine($"       Courses  : {s.EnrolledCourseIds.Count}");
            Console.WriteLine($"       Avg Prog : {s.GetAverageProgress():0.0}%");
            Console.WriteLine($"       Active   : {(s.IsActive ? "Yes" : "No")}");
            Console.WriteLine("  ------------------------------");
        }
    }

    // System-wide statistics dashboard for admins
    public static void DisplayAnalytics(List<User> users, List<Course> courses, List<Enrollment> enrollments)
    {
        var students    = users.OfType<Student>().ToList();
        var instructors = users.OfType<Instructor>().ToList();
        var admins      = users.OfType<Admin>().ToList();

        // Use Distinct() to count unique categories
        int uniqueCategories = courses.Select(c => c.Category).Distinct().Count();

        Console.WriteLine("╔════════════════════════════════╗");
        Console.WriteLine("║      System Analytics          ║");
        Console.WriteLine("╚════════════════════════════════╝");
        Console.WriteLine("  ── Users ──────────────────────");
        Console.WriteLine($"  Total Users          : {users.Count}");
        Console.WriteLine($"  Students             : {students.Count}");
        Console.WriteLine($"  Instructors          : {instructors.Count}");
        Console.WriteLine($"  Admins               : {admins.Count}");
        Console.WriteLine($"  Active Users         : {users.Count(u => u.IsActive)}");
        Console.WriteLine("  ── Courses ─────────────────────");
        Console.WriteLine($"  Total Courses        : {courses.Count}");
        Console.WriteLine($"  Categories           : {uniqueCategories}");
        Console.WriteLine($"  Total Enrollments    : {enrollments.Count}");
        Console.WriteLine("  ── Progress ────────────────────");
        Console.WriteLine($"  System Avg Progress  : {CalculateSystemAverageProgress(users):0.00}%");
        Console.WriteLine($"  Active Students      : {GetActiveStudents(users).Count}");
        Console.WriteLine($"  Any Course Completed : {(HasStudentCompletedAny(users) ? "Yes" : "No")}");
        Console.WriteLine("  ── Popularity ──────────────────");
        Console.WriteLine($"  Most Enrolled Cat    : {GetMostEnrolledCategory(courses)}");
        Console.WriteLine($"  All Phys. Seats Full : {(AreAllCoursesFilled(courses) ? "Yes" : "No")}");
        Console.WriteLine("================================");
    }

    // Courses organised by category with enrollment totals
    public static void DisplayCategoryBreakdown(List<Course> courses)
    {
        var byCategory = courses
            .GroupBy(c => c.Category)
            .OrderBy(g => g.Key);

        Console.WriteLine("==============================");
        Console.WriteLine("     Courses by Category      ");
        Console.WriteLine("==============================");

        foreach (var group in byCategory)
        {
            int totalEnrolled = group.Sum(c => c.CurrentEnrollments);
            Console.WriteLine($"\n  [{group.Key}]  {group.Count()} course(s) — {totalEnrolled} total enrolled");

            foreach (Course c in group.OrderBy(c => c.CourseName))
            {
                string seats = c is OnlineCourse
                    ? "Unlimited"
                    : $"{c.GetAvailableSeats()} seat(s) left";
                Console.WriteLine($"    • {c.CourseName,-30} {c.CurrentEnrollments,3} enrolled | {seats}");
            }
        }

        Console.WriteLine("==============================");
    }

    // ==================== FILE PERSISTENCE ====================

    private static readonly string DataFolder   = "Data";
    private static readonly string StudentsFile = Path.Combine(DataFolder, "students.json");
    private static readonly string CoursesFile  = Path.Combine(DataFolder, "courses.json");

    // Saves students and courses to JSON files in the Data/ folder
    public static void SaveAllData(List<User> users, List<Course> courses)
    {
        try
        {
            // Create the Data folder if it does not exist
            Directory.CreateDirectory(DataFolder);

            var options = new JsonSerializerOptions { WriteIndented = true };

            // ── Students ──────────────────────────────────────────────
            var studentData = users
                .OfType<Student>()
                .Select(s => new StudentSaveData
                {
                    Username    = s.Username,
                    Email       = s.Email,
                    Password    = s.Password,
                    IsActive    = s.IsActive,
                    Progress    = s.CourseProgress
                                   .Select(kvp => new ProgressEntry
                                   {
                                       CourseId = kvp.Key,
                                       Value    = kvp.Value
                                   }).ToList()
                })
                .ToList();

            File.WriteAllText(StudentsFile, JsonSerializer.Serialize(studentData, options));

            // ── Courses ───────────────────────────────────────────────
            var courseData = courses.Select(c => BuildCourseRecord(c)).ToList();
            File.WriteAllText(CoursesFile, JsonSerializer.Serialize(courseData, options));

            Console.WriteLine($"  ✔ Saved: {studentData.Count} student(s) and {courseData.Count} course(s).");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ❌ Save error: {ex.Message}");
        }
    }

    // Builds a typed DTO record for one course so its subtype can be reconstructed on load
    private static CourseSaveData BuildCourseRecord(Course c)
    {
        var rec = new CourseSaveData
        {
            Type              = c.GetCourseType(),
            CourseId          = c.CourseId,
            CourseName        = c.CourseName,
            Description       = c.Description,
            InstructorName    = c.InstructorName,
            Category          = c.Category,
            CurrentEnrollments= c.CurrentEnrollments
        };

        if (c is OnlineCourse oc)
            rec.VideoDurationMinutes = oc.VideoDurationMinutes;

        if (c is InPersonCourse ip)
        {
            rec.MaxStudents = ip.MaxStudents;
            rec.RoomNumber  = ip.RoomNumber;
            rec.Building    = ip.Building;
        }

        if (c is HybridCourse hc)
        {
            rec.MaxStudents         = hc.MaxStudents;
            rec.OnlineVideoDuration = hc.OnlineVideoDuration;
            rec.RoomNumber          = hc.RoomNumber;
            rec.Building            = hc.Building;
        }

        return rec;
    }

    // Loads students and courses from disk; skips items already in memory
    public static void LoadAllData(List<User> users, List<Course> courses)
    {
        // ── Students ──────────────────────────────────────────────────
        if (File.Exists(StudentsFile))
        {
            try
            {
                string json  = File.ReadAllText(StudentsFile);
                var    saved = JsonSerializer.Deserialize<List<StudentSaveData>>(json);
                int    count = 0;

                foreach (StudentSaveData sd in saved)
                {
                    // Skip if already present (seeded or previously loaded)
                    if (users.Any(u => u.Username == sd.Username)) continue;

                    Student s = new Student(sd.Username, sd.Password, sd.Email);
                    s.IsActive = sd.IsActive;

                    foreach (ProgressEntry pe in sd.Progress)
                    {
                        if (!s.EnrolledCourseIds.Contains(pe.CourseId))
                            s.EnrolledCourseIds.Add(pe.CourseId);
                        s.CourseProgress[pe.CourseId] = pe.Value;
                    }

                    users.Add(s);
                    count++;
                }

                Console.WriteLine($"  ✔ Loaded {count} student(s) from file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ⚠ Could not load students.json: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("  ℹ No student data file found — starting fresh.");
        }

        // ── Courses ───────────────────────────────────────────────────
        if (File.Exists(CoursesFile))
        {
            try
            {
                string json     = File.ReadAllText(CoursesFile);
                var    elements = JsonSerializer.Deserialize<List<CourseSaveData>>(json);
                int    count    = 0;

                foreach (CourseSaveData rec in elements)
                {
                    // Skip seeded / already-loaded courses
                    if (courses.Any(c => c.CourseId == rec.CourseId)) continue;

                    Course course = null;

                    if (rec.Type == "Online")
                    {
                        course = new OnlineCourse(
                            rec.CourseId, rec.CourseName, rec.Description,
                            rec.InstructorName, rec.Category,
                            rec.VideoDurationMinutes ?? 0);
                    }
                    else if (rec.Type == "In-Person")
                    {
                        course = new InPersonCourse(
                            rec.CourseId, rec.CourseName, rec.Description,
                            rec.InstructorName, rec.Category,
                            rec.MaxStudents ?? 20,
                            rec.RoomNumber ?? "TBD", rec.Building ?? "TBD");
                    }
                    else if (rec.Type == "Hybrid")
                    {
                        course = new HybridCourse(
                            rec.CourseId, rec.CourseName, rec.Description,
                            rec.InstructorName, rec.Category,
                            rec.MaxStudents ?? 20,
                            rec.OnlineVideoDuration ?? 0,
                            rec.RoomNumber ?? "TBD", rec.Building ?? "TBD");
                    }

                    if (course != null)
                    {
                        course.CurrentEnrollments = rec.CurrentEnrollments;
                        courses.Add(course);
                        count++;
                    }
                }

                Console.WriteLine($"  ✔ Loaded {count} course(s) from file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ⚠ Could not load courses.json: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("  ℹ No course data file found — seeding defaults.");
        }
    }

    // ==================== DATA-TRANSFER OBJECTS ====================

    // Used for both serialisation and deserialisation — nullable fields handle optional values
    private class CourseSaveData
    {
        public string Type               { get; set; }
        public int    CourseId           { get; set; }
        public string CourseName         { get; set; }
        public string Description        { get; set; }
        public string InstructorName     { get; set; }
        public string Category           { get; set; }
        public int    CurrentEnrollments { get; set; }
        // Optional — only present for the relevant subtype
        public int?   VideoDurationMinutes { get; set; }
        public int?   MaxStudents          { get; set; }
        public int?   OnlineVideoDuration  { get; set; }
        public string RoomNumber           { get; set; }
        public string Building             { get; set; }
    }

    private class StudentSaveData
    {
        public string             Username { get; set; }
        public string             Email    { get; set; }
        public string             Password { get; set; }
        public bool               IsActive { get; set; }
        public List<ProgressEntry> Progress { get; set; } = new List<ProgressEntry>();
    }

    private class ProgressEntry
    {
        public int CourseId { get; set; }
        public int Value    { get; set; }
    }
}