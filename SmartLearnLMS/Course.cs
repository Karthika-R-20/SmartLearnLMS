using System;

public abstract class Course : IEnrollable, ISearchable, IRatable
{
    // ==================== PROPERTIES ====================

    public int    CourseId           { get; set; }
    public string Description        { get; set; }
    public string InstructorName     { get; set; }
    public string Category           { get; set; }
    public int    CurrentEnrollments { get; set; }

    // ==================== COURSE NAME PROPERTY ====================

    private string _courseName;
    public string CourseName
    {
        get { return _courseName; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Console.WriteLine("  ✗ Title cannot be empty.");
                return;
            }
            if (value.Length > 100)
            {
                Console.WriteLine("  ✗ Title must be 1-100 characters");
                return;
            }
            _courseName = value;
        }
    }

    // ==================== PRIVATE FIELDS (ratings) ====================
 
    // Private — only accessible through AddRating(), GetAverageRating(), GetTotalRatings()
    private List<int>    ratings = new List<int>();
    private List<string> reviews = new List<string>();

    // ==================== CONSTRUCTOR ====================
    // protected — only derived classes can call this, prevents "new Course(...)" directly
    // CurrentEnrollments always starts at 0 — a new course has no students yet
    protected Course(int courseId, string courseName, string description,
                  string instructorName, string category)
    {
        CourseId           = courseId;
        CourseName         = courseName;
        Description        = description;
        InstructorName     = instructorName;
        Category           = category;
        CurrentEnrollments = 0;               // FIX: always starts at 0
    }

    // ==================== ABSTRACT METHODS ====================

    public abstract bool CanEnroll(Student student); 
    public abstract void DisplayCourseInfo(); 
    public abstract string GetCourseType(); 
    public abstract int GetAvailableSeats();

    // ==================== IEnrollable IMPLEMENTATION ====================
 
    // Enroll: checks CanEnroll first, updates count, calls student method, notifies if INotifiable
    public void Enroll(Student student)
    {
        // Step 1: ask the derived class whether this student can enroll
        if (!CanEnroll(student))
        {
            Console.WriteLine($"  Cannot enroll {student.Username} in '{CourseName}'.");
            return;
        }
 
        // Step 2: increment the course enrollment count
        CurrentEnrollments++;
 
        // Step 3: tell the Student object to record this enrollment
        student.EnrollInCourse(CourseId);
 
        Console.WriteLine($"  {student.Username} enrolled in '{CourseName}'.");
 
        // Step 4: if student implements INotifiable, send a confirmation message
        if (student is INotifiable notifiable)
        {
            notifiable.SendNotification(
                $"You have been enrolled in '{CourseName}' ({GetCourseType()}).");
        }
    }
 
    // Drop: decrements count and shows confirmation
    public void Drop(Student student)
    {
        if (CurrentEnrollments > 0)
            CurrentEnrollments--;
 
        Console.WriteLine($"  {student.Username} has been dropped from '{CourseName}'.");
    }
 
    // ==================== ISearchable IMPLEMENTATION ====================
 
    // Returns true if keyword matches Title, Description, Category, or InstructorName
    public bool MatchesSearch(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return false;
 
        string k = keyword.ToLower();
 
        return CourseName.ToLower().Contains(k)       ||
               Description.ToLower().Contains(k) ||
               Category.ToLower().Contains(k)    ||
               InstructorName.ToLower().Contains(k);
    }
 
    // Returns a single formatted summary line used in search results
    public string GetSearchSummary()
    {
        return $"[{GetCourseType()}] {CourseName} | {Category} | Instructor: {InstructorName}";
    }
 
    // ==================== IRatable IMPLEMENTATION ====================
 
    // Validates 1-5, adds star rating and review text to the private lists
    public void AddRating(int stars, string review)
    {
        if (stars < 1 || stars > 5)
        {
            Console.WriteLine("  Invalid rating. Stars must be between 1 and 5.");
            return;
        }
 
        ratings.Add(stars);
        reviews.Add(review);
 
        Console.WriteLine($"  Rating of {stars} star(s) added for '{CourseName}'.");
    }
 
    // Returns average of all ratings, or 0 if no ratings exist yet
    public double GetAverageRating()
    {
        if (ratings.Count == 0)
            return 0;
 
        int total = 0;
        foreach (int star in ratings)
            total += star;
 
        return (double)total / ratings.Count;
    }
 
    // Returns the total number of ratings submitted
    public int GetTotalRatings()
    {
        return ratings.Count;
    }
}