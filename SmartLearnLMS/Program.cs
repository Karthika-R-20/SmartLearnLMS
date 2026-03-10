using System;
using System.Collections.Generic;

namespace SmartLearn
{
    class Program
    {
        // ==================== DATA STRUCTURES ====================

        static List<User> users = new List<User>();
        static List<Course> courses = new List<Course>();
        static List<Enrollment> enrollments = new List<Enrollment>();

        // ==================== SESSION STATE ====================

        static bool isLoggedIn = false;
        static User currentUser = null;

        // ==================== ENTRY POINT ====================

        static void Main(string[] args)
        {
            // Seed typed objects — Student, Instructor, Admin (not plain User)
            users.Add(new Student("student1", "pass123", "student@smartlearn.com"));
            users.Add(new Instructor("instructor1", "pass123", "instructor@smartlearn.com"));
            users.Add(new Admin("admin", "admin123", "admin@smartlearn.com"));

            // Assign courses to the seeded instructor
            Instructor seededInstructor = users.Find(u => u is Instructor) as Instructor;
            if (seededInstructor != null)
            {
                seededInstructor.AddCourse(1);
                seededInstructor.AddCourse(2);
                seededInstructor.AddCourse(3);
            }

            // Load sample courses
            LoadSampleCourses();

            // Main program loop
            while (true)
            {
                if (!isLoggedIn)
                    ShowMainMenu();
                else
                    ShowDashboard();
            }
        }

        // ==================== LOAD SAMPLE COURSES ====================

        static void LoadSampleCourses()
        {
            courses.Clear();

            courses.Add(new Course(1, "C# Programming Fundamentals",
                "Learn the basics of C# including variables, loops, and OOP",
                "Prof. Smith", "Programming", 25));

            courses.Add(new Course(2, "Introduction to SQL Server",
                "Database fundamentals including queries, joins, and stored procedures",
                "Prof. Johnson", "Database", 20));

            courses.Add(new Course(3, "Web Development with ASP.NET Core",
                "Build modern web applications using ASP.NET Core MVC",
                "Prof. Williams", "Web Development", 30));

            courses.Add(new Course(4, "Machine Learning with Python",
                "Introduction to ML algorithms, data preprocessing, and model training",
                "Prof. Davis", "Data Science", 20));

            courses.Add(new Course(5, "Cloud Computing with Azure",
                "Deploy and manage applications on Microsoft Azure cloud platform",
                "Prof. Martinez", "Cloud", 15));

            courses.Add(new Course(6, "Cybersecurity Essentials",
                "Learn ethical hacking, network security, and threat prevention",
                "Prof. Brown", "Security", 18));

            courses.Add(new Course(7, "Mobile App Development with Flutter",
                "Build cross-platform mobile apps for iOS and Android using Flutter",
                "Prof. Taylor", "Mobile", 22));
        }

        // ==================== MAIN MENU ====================

        static void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("     Welcome to SmartLearn   ");
            Console.WriteLine("==========Main Menu===========");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.WriteLine("3. Browse Courses");
            Console.WriteLine("4. Exit");
            Console.WriteLine("==============================");
            Console.Write("Enter your choice (1-4): ");

            string input = Console.ReadLine();

            if (!int.TryParse(input, out int choice) || choice < 1 || choice > 4)
            {
                ShowError("Invalid input! Please enter 1, 2, 3, or 4.");
                Pause("Press any key to try again..."); return;
            }

            if (choice == 1) Login();
            else if (choice == 2) RegisterUser();
            else if (choice == 3) BrowseCourses();
            else if (choice == 4) ExitApp();
        }

        // Routes to dashboard using "is" keyword type checking
        static void ShowDashboard()
        {
            // Use "is" keyword to check the actual type of currentUser
            if (currentUser is Student student)
                ShowStudentDashboard(student);
            else if (currentUser is Instructor instructor)
                ShowInstructorDashboard(instructor);
            else if (currentUser is Admin admin)
                ShowAdminDashboard(admin);
        }

        // ==================== REGISTER USER ====================

        // Creates correct typed object — Student, Instructor, or Admin
        static void RegisterUser()
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("       User Registration     ");
            Console.WriteLine("==============================");

            // Step 1: Get and validate username
            Console.Write("  Enter Username: ");
            string username = Console.ReadLine()?.Trim();

            if (!ValidateUsername(username)) { Pause("Press any key to go back..."); return; }

            // Step 2: Check username not already taken
            User existingUser = users.Find(u => u.Username.ToLower() == username.ToLower());
            if (existingUser != null)
            {
                ShowError("Username '" + username + "' already exists. Please choose another.");
                Pause("Press any key to go back..."); return;
            }

            // Step 3: Get and validate password
            Console.Write("  Enter Password: ");
            string password = Console.ReadLine();

            if (!ValidatePassword(password)) { Pause("Press any key to go back..."); return; }

            // Step 4: Confirm password
            Console.Write("  Confirm Password: ");
            string confirmPassword = Console.ReadLine();

            if (confirmPassword != password)
            {
                ShowError("Passwords do not match.");
                Pause("Press any key to go back..."); return;
            }

            // Step 5: Get and validate email
            Console.Write("  Enter Email: ");
            string email = Console.ReadLine()?.Trim();

            if (!ValidateEmail(email)) { Pause("Press any key to go back..."); return; }

            // Step 6: Get role choice
            Console.WriteLine("  Select Role:  1. Student   2. Instructor   3. Admin");
            Console.Write("  Enter Role (1-3): ");
            string roleInput = Console.ReadLine()?.Trim();

            // Step 7: Create the CORRECT typed object based on role choice
            User newUser = null;

            if (roleInput == "1")
                newUser = new Student(username, password, email);
            else if (roleInput == "2")
                newUser = new Instructor(username, password, email);
            else if (roleInput == "3")
                newUser = new Admin(username, password, email);
            else
            {
                ShowError("Invalid role. Please enter 1, 2, or 3.");
                Pause("Press any key to go back..."); return;
            }

            // Step 8: Add typed object to users list
            users.Add(newUser);

            // Step 9: Show success
            Console.WriteLine("\n  ✔ Registration Successful!");
            Console.WriteLine("  ------------------------------");
            newUser.DisplayInfo();
            Console.WriteLine("  ------------------------------");
            Console.WriteLine("  You can now log in with your credentials.");
            Pause("Press any key to continue...");
        }

        // ==================== VALIDATION METHODS ====================

        static bool ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            { ShowError("Username cannot be empty."); return false; }

            if (username.Length < 3)
            { ShowError("Username must be at least 3 characters long."); return false; }

            if (username.Length > 20)
            { ShowError("Username cannot exceed 20 characters."); return false; }

            if (username.Contains(" "))
            { ShowError("Username cannot contain spaces."); return false; }

            return true;
        }

        static bool ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            { ShowError("Password cannot be empty."); return false; }

            if (password.Length < 6)
            { ShowError("Password must be at least 6 characters long."); return false; }

            if (password.Length > 30)
            { ShowError("Password cannot exceed 30 characters."); return false; }

            return true;
        }

        static bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            { ShowError("Email cannot be empty."); return false; }

            if (!email.Contains("@"))
            { ShowError("Invalid email. Must contain '@'."); return false; }

            if (!email.Contains("."))
            { ShowError("Invalid email. Must contain a '.'"); return false; }

            User emailMatch = users.Find(u => u.Email.ToLower() == email.ToLower());
            if (emailMatch != null)
            { ShowError("Email '" + email + "' is already registered."); return false; }

            return true;
        }

        // ==================== LOGIN ====================

        static void Login()
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("            Login            ");
            Console.WriteLine("==============================");

            // Step 1: Get username and search users list
            Console.Write("  Username: ");
            string username = Console.ReadLine()?.Trim();

            User foundUser = users.Find(u => u.Username.ToLower() == username.ToLower());

            if (foundUser == null)
            {
                ShowError("Username '" + username + "' not found. Please register first.");
                Pause("Press any key to go back..."); return;
            }

            // Step 2: Get password and use User's ValidatePassword() method
            Console.Write("  Password: ");
            string password = Console.ReadLine();

            if (!foundUser.ValidatePassword(password))
            {
                ShowError("Incorrect password. Please try again.");
                Pause("Press any key to go back..."); return;
            }

            // Step 3: Set session state — store actual typed object
            isLoggedIn = true;
            currentUser = foundUser;

            // Step 4: Use "is" keyword to show typed welcome message
            string userType = "";
            if (currentUser is Student) userType = "Student";
            else if (currentUser is Instructor) userType = "Instructor";
            else if (currentUser is Admin) userType = "Admin";

            Console.WriteLine("\n  ✔ Login successful! Welcome back, " + currentUser.Username + ".");
            Console.WriteLine("  Account Type : " + userType);
            Pause("Press any key to continue...");
        }

        // ==================== LOGOUT ====================

        static void Logout()
        {
            string name = currentUser.Username;
            isLoggedIn = false;
            currentUser = null;
            Console.WriteLine("\n  ✔ Logged out successfully. Goodbye, " + name + "!");
            Pause("Press any key to continue...");
        }

        // ==================== COURSE BROWSING ====================

        static void BrowseCourses()
        {
            int choice;
            do
            {
                Console.Clear();
                Console.WriteLine("==============================");
                Console.WriteLine("        Browse Courses       ");
                Console.WriteLine("==============================");
                Console.WriteLine("1. View All Courses");
                Console.WriteLine("2. Search Courses");
                Console.WriteLine("3. Back");
                Console.WriteLine("==============================");
                Console.Write("Enter your choice (1-3): ");

                string input = Console.ReadLine();

                if (!int.TryParse(input, out choice) || choice < 1 || choice > 3)
                {
                    ShowError("Invalid input! Please enter 1, 2, or 3.");
                    Pause("Press any key to try again..."); continue;
                }

                if (choice == 1) DisplayAllCourses();
                else if (choice == 2) SearchCourses();

            } while (choice != 3);
        }

        static void SearchCourses()
        {
            char searchAgain;
            do
            {
                Console.Clear();
                Console.WriteLine("==============================");
                Console.WriteLine("        Search Courses       ");
                Console.WriteLine("==============================");
                Console.Write("Enter keyword to search: ");

                string keyword = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    ShowError("Keyword cannot be empty.");
                }
                else
                {
                    Console.WriteLine("\n------------------------------");
                    Console.WriteLine("  Results for: \"" + keyword + "\"");
                    Console.WriteLine("------------------------------");

                    int matchCount = 0;
                    foreach (Course course in courses)
                    {
                        if (course.CourseName.ToLower().Contains(keyword.ToLower()) ||
                            course.Category.ToLower().Contains(keyword.ToLower()))
                        {
                            matchCount++;
                            Console.WriteLine("  " + matchCount + ". " + course.CourseName + " [" + course.Category + "]");
                        }
                    }

                    if (matchCount == 0)
                        Console.WriteLine("  No results found. Try a different keyword.");
                    else
                        Console.WriteLine("\n  " + matchCount + " course(s) found.");

                    Console.WriteLine("------------------------------");
                }

                Console.Write("\nSearch again? (y/n): ");
                string again = Console.ReadLine()?.Trim().ToLower();
                searchAgain = (again == "y") ? 'y' : 'n';

            } while (searchAgain == 'y');
        }

        // ==================== STUDENT DASHBOARD ====================

        // Takes typed Student parameter — accesses Student-specific methods directly
        static void ShowStudentDashboard(Student student)
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("     Student Dashboard       ");
            Console.WriteLine("  Hello, " + student.Username + "!");
            Console.WriteLine("  Enrolled in: " + student.EnrolledCourseIds.Count + " course(s)");
            Console.WriteLine("==============================");
            Console.WriteLine("1. Browse & Enroll Courses");
            Console.WriteLine("2. My Enrolled Courses");
            Console.WriteLine("3. Update Progress");
            Console.WriteLine("4. Drop a Course");
            Console.WriteLine("5. My Statistics");
            Console.WriteLine("6. Logout");
            Console.WriteLine("==============================");
            Console.Write("Enter your choice (1-6): ");

            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > 6)
            {
                ShowError("Invalid input! Please enter 1 to 6.");
                Pause("Press any key to try again..."); return;
            }

            if (choice == 1) EnrollStudentInCourse(student);
            else if (choice == 2) ShowMyEnrolledCourses(student);
            else if (choice == 3) UpdateStudentProgress(student);
            else if (choice == 4) DropStudentCourse(student);
            else if (choice == 5) ShowStudentStats(student);
            else if (choice == 6) Logout();
        }

        // ==================== ENROLL STUDENT IN COURSE ====================

        // Coordinates between Student, Course, and Enrollment objects
        // Handles the entire enrollment workflow
        static void EnrollStudentInCourse(Student student)
        {
            Console.WriteLine("\n=== AVAILABLE COURSES ===");

            // Display all courses with full details
            foreach (Course course in courses)
            {
                // Mark already enrolled courses so student can see at a glance
                string enrolledTag = student.EnrolledCourseIds.Contains(course.CourseId)
                    ? " [ENROLLED]" : "";

                Console.WriteLine($"\n[{course.CourseId}] {course.CourseName}{enrolledTag}");
                Console.WriteLine($"  Category    : {course.Category}");
                Console.WriteLine($"  Instructor  : {course.InstructorName}");
                Console.WriteLine($"  Enrollment  : {course.CurrentEnrollments}/{course.MaxStudents}");
                Console.WriteLine($"  Available   : {(course.CanEnroll() ? "Yes" : "Full")}");
                Console.WriteLine("  ---");
            }

            Console.Write("\nEnter Course ID to enroll: ");

            // Validate input is a number
            if (!int.TryParse(Console.ReadLine(), out int courseId))
            {
                Console.WriteLine("  Invalid course ID.");
                Pause("Press any key to go back..."); return;
            }

            // Find the Course object from the courses list using CourseId
            Course selectedCourse = courses.Find(c => c.CourseId == courseId);

            if (selectedCourse == null)
            {
                Console.WriteLine("  Course not found.");
                Pause("Press any key to go back..."); return;
            }

            // Validate: course must not be full
            if (!selectedCourse.CanEnroll())
            {
                Console.WriteLine("  Course is full!");
                Pause("Press any key to go back..."); return;
            }

            // Validate: student must not already be enrolled
            if (student.EnrolledCourseIds.Contains(courseId))
            {
                Console.WriteLine("  Already enrolled in this course!");
                Pause("Press any key to go back..."); return;
            }

            // Enroll: call Student's EnrollInCourse() — adds to EnrolledCourseIds and sets progress to 0
            student.EnrollInCourse(courseId);

            // Update Course enrollment count
            selectedCourse.IncrementEnrollment();

            // Create Enrollment record and add to global enrollments list
            int enrollmentId = enrollments.Count + 1;
            Enrollment newEnrollment = new Enrollment(enrollmentId, student.Username, courseId);
            enrollments.Add(newEnrollment);

            Console.WriteLine($"\n  Successfully enrolled in '{selectedCourse.CourseName}'!");
            Pause("Press any key to continue...");
        }

        // Shows enrolled courses with full course names — not just IDs
        static void ShowMyEnrolledCourses(Student student)
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("     My Enrolled Courses     ");
            Console.WriteLine("  Student: " + student.Username);
            Console.WriteLine("==============================");

            if (student.EnrolledCourseIds.Count == 0)
            {
                Console.WriteLine("\n  You haven't enrolled in any courses yet.");
                Console.WriteLine("  Go to 'Browse & Enroll' to get started!");
                Pause("Press any key to go back..."); return;
            }

            // Call enhanced ShowEnrolledCourses — passes courses and enrollments for full details
            student.ShowEnrolledCourses(courses, enrollments);

            Console.WriteLine("  Total: " + student.EnrolledCourseIds.Count + " course(s)");
            Console.WriteLine("==============================");
            Pause("Press any key to go back...");
        }

        // Shows progress summary using Student's GetAverageProgress() and GetCompletedCourses()
        static void ShowMyProgress(Student student)
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("        My Progress          ");
            Console.WriteLine("  Student: " + student.Username);
            Console.WriteLine("==============================");

            if (student.EnrolledCourseIds.Count == 0)
            {
                Console.WriteLine("  No courses enrolled yet.");
                Pause("Press any key to go back..."); return;
            }

            // Use Student's own methods
            double average = student.GetAverageProgress();
            List<int> completed = student.GetCompletedCourses();

            Console.WriteLine("  Enrolled Courses : " + student.EnrolledCourseIds.Count);
            Console.WriteLine("  Completed Courses: " + completed.Count);
            Console.WriteLine("  Average Progress : " + average.ToString("0.0") + "%");
            Console.WriteLine("------------------------------");

            // Show each course with a progress bar
            foreach (int courseId in student.EnrolledCourseIds)
            {
                Course course = courses.Find(c => c.CourseId == courseId);
                int progress = student.CourseProgress[courseId];
                string bar = BuildProgressBar(progress);

                Console.WriteLine("  " + (course != null ? course.CourseName : "Course " + courseId));
                Console.WriteLine("  " + bar + " " + progress + "%");
                Console.WriteLine();
            }

            Console.WriteLine("==============================");
            Pause("Press any key to go back...");
        }

        // Builds a simple text progress bar e.g. [████████░░] 80%
        static string BuildProgressBar(int percentage)
        {
            int filled = percentage / 10;
            int empty = 10 - filled;
            return "[" + new string('█', filled) + new string('░', empty) + "]";
        }

        // ==================== UPDATE STUDENT PROGRESS ====================

        // Keeps Student and Enrollment data in sync by updating both at once
        static void UpdateStudentProgress(Student student)
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("      Update My Progress     ");
            Console.WriteLine("  Student: " + student.Username);
            Console.WriteLine("==============================");

            // Step 1: Show student's currently enrolled courses with current progress
            if (student.EnrolledCourseIds.Count == 0)
            {
                Console.WriteLine("  You are not enrolled in any courses yet.");
                Pause("Press any key to go back..."); return;
            }

            // Show enrolled courses with full details for course selection
            student.ShowEnrolledCourses(courses, enrollments);

            // Step 2: Get course ID from user
            Console.Write("\nEnter Course ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int courseId))
            {
                Console.WriteLine("  Invalid course ID.");
                Pause("Press any key to go back..."); return;
            }

            // Step 3: Get progress percentage from user and validate range
            Console.Write("Enter progress percentage (0-100): ");
            if (!int.TryParse(Console.ReadLine(), out int progress) || progress < 0 || progress > 100)
            {
                Console.WriteLine("  Invalid progress value. Must be between 0 and 100.");
                Pause("Press any key to go back..."); return;
            }

            // Step 4: Update progress in Student object (CourseProgress dictionary)
            student.UpdateProgress(courseId, progress);

            // Step 5: Find the matching Enrollment record and update it too
            // This keeps Student and Enrollment data in sync
            Enrollment enrollment = enrollments.Find(e =>
                e.StudentUsername == student.Username &&
                e.CourseId == courseId);

            if (enrollment != null)
            {
                // Call Enrollment's UpdateProgress() — also auto-calls MarkComplete() if 100
                enrollment.UpdateProgress(progress);
                Console.WriteLine("\n  Progress updated successfully!");
                Console.WriteLine("  Course ID : " + courseId);
                Console.WriteLine("  Progress  : " + progress + "%");

                if (progress == 100)
                    Console.WriteLine("  Status    : Completed!");
            }
            else
            {
                Console.WriteLine("\n  Warning: Enrollment record not found for Course ID " + courseId);
            }

            Pause("Press any key to continue...");
        }

        // ==================== DROP STUDENT COURSE ====================

        // Coordinates removing from Student, Course, and Enrollment — all three stay in sync
        static void DropStudentCourse(Student student)
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("         Drop Course         ");
            Console.WriteLine("  Student: " + student.Username);
            Console.WriteLine("==============================");

            // Step 1: Show enrolled courses so student knows what IDs are available
            if (student.EnrolledCourseIds.Count == 0)
            {
                Console.WriteLine("  You are not enrolled in any courses.");
                Pause("Press any key to go back..."); return;
            }

            // Show enrolled courses with full details for course selection
            student.ShowEnrolledCourses(courses, enrollments);

            // Step 2: Get course ID to drop
            Console.Write("\nEnter Course ID to drop: ");
            if (!int.TryParse(Console.ReadLine(), out int courseId))
            {
                Console.WriteLine("  Invalid course ID.");
                Pause("Press any key to go back..."); return;
            }

            // Store course name before dropping for the confirmation message
            Course course = courses.Find(c => c.CourseId == courseId);
            string courseName = course != null ? course.CourseName : "Course ID " + courseId;

            // Step 3: Call Student's DropCourse() — removes from EnrolledCourseIds and CourseProgress
            student.DropCourse(courseId);

            // Step 4: Find the Course object and decrement its enrollment count
            if (course != null)
            {
                course.DecrementEnrollment();
            }

            // Step 5: Find matching Enrollment record and remove it from the global list
            Enrollment enrollment = enrollments.Find(e =>
                e.StudentUsername == student.Username &&
                e.CourseId == courseId);

            if (enrollment != null)
            {
                enrollments.Remove(enrollment);
                Console.WriteLine("\n  Successfully dropped '" + courseName + "'.");
                Console.WriteLine("  Enrollment record removed.");
            }

            Pause("Press any key to continue...");
        }

        // ==================== SHOW STUDENT STATS ====================

        // Combines Student data with Course data for a full statistics display
        static void ShowStudentStats(Student student)
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("       Your Statistics       ");
            Console.WriteLine("==============================");
            Console.WriteLine($"  Username        : {student.Username}");
            Console.WriteLine($"  Total Enrolled  : {student.EnrolledCourseIds.Count} course(s)");
            Console.WriteLine($"  Completed       : {student.GetCompletedCourses().Count} course(s)");
            Console.WriteLine($"  Average Progress: {student.GetAverageProgress():F2}%");
            Console.WriteLine("------------------------------");

            // Get list of completed course IDs from Student
            List<int> completed = student.GetCompletedCourses();

            if (completed.Count > 0)
            {
                Console.WriteLine("  Completed Courses:");

                // Loop through completed IDs and find full Course object for the name
                foreach (int courseId in completed)
                {
                    Course course = courses.Find(c => c.CourseId == courseId);
                    if (course != null)
                    {
                        Console.WriteLine("    ✔ " + course.CourseName);
                    }
                }
            }
            else
            {
                Console.WriteLine("  No completed courses yet.");
                Console.WriteLine("  Update your progress to 100% to complete a course!");
            }

            Console.WriteLine("==============================");
            Pause("Press any key to go back...");
        }

        // ==================== INSTRUCTOR DASHBOARD ====================

        // Takes typed Instructor parameter — accesses Instructor-specific methods
        static void ShowInstructorDashboard(Instructor instructor)
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("    Instructor Dashboard     ");
            Console.WriteLine("  Hello, " + instructor.Username + "!");
            Console.WriteLine("  Teaching: " + instructor.CourseIds.Count + " course(s)");
            Console.WriteLine("==============================");
            Console.WriteLine("1. My Courses");
            Console.WriteLine("2. View My Students");
            Console.WriteLine("3. Add a Course to Teach");
            Console.WriteLine("4. My Student Count");
            Console.WriteLine("5. Logout");
            Console.WriteLine("==============================");
            Console.Write("Enter your choice (1-5): ");

            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > 5)
            {
                ShowError("Invalid input! Please enter 1 to 5.");
                Pause("Press any key to try again..."); return;
            }

            if (choice == 1) ShowInstructorCourses(instructor);
            else if (choice == 2) ShowInstructorStudents(instructor);
            else if (choice == 3) AddInstructorCourse(instructor);
            else if (choice == 4) ShowInstructorStudentCount(instructor);
            else if (choice == 5) Logout();
        }

        // Shows the instructor's assigned courses with full Course object details
        static void ShowInstructorCourses(Instructor instructor)
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("        My Courses           ");
            Console.WriteLine("  Instructor: " + instructor.Username);
            Console.WriteLine("==============================");

            if (instructor.CourseIds.Count == 0)
            {
                Console.WriteLine("  No courses assigned yet.");
                Pause("Press any key to go back..."); return;
            }

            // Call enhanced ShowMyCourses — shows title, student count, average progress
            instructor.ShowMyCourses(courses, enrollments);
            Pause("Press any key to go back...");
        }

        // Shows all students enrolled in the instructor's courses
        static void ShowInstructorStudents(Instructor instructor)
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("        My Students          ");
            Console.WriteLine("  Instructor: " + instructor.Username);
            Console.WriteLine("==============================");

            int count = 0;
            foreach (Enrollment enrollment in enrollments)
            {
                if (instructor.CourseIds.Contains(enrollment.CourseId))
                {
                    Course course = courses.Find(c => c.CourseId == enrollment.CourseId);
                    Console.WriteLine("  Student  : " + enrollment.StudentUsername);
                    Console.WriteLine("  Course   : " + (course != null ? course.CourseName : "Unknown"));
                    Console.WriteLine("  Progress : " + enrollment.ProgressPercentage + "%");
                    Console.WriteLine("  ------------------------------");
                    count++;
                }
            }

            if (count == 0)
                Console.WriteLine("  No students enrolled in your courses yet.");

            Console.WriteLine("  Total: " + instructor.GetStudentCount(enrollments) + " unique student(s)");
            Console.WriteLine("==============================");
            Pause("Press any key to go back...");
        }

        // ==================== INSTRUCTOR HELPER METHODS ====================

        // Lets instructor add a course to teach — validates course exists first
        static void AddInstructorCourse(Instructor instructor)
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("     Add Course to Teach     ");
            Console.WriteLine("  Instructor: " + instructor.Username);
            Console.WriteLine("==============================");

            // Step 1: Show all available courses so instructor can see IDs
            DisplayAllCourses();

            // Step 2: Get course ID from user
            Console.Write("\nEnter Course ID to add: ");
            if (!int.TryParse(Console.ReadLine(), out int courseId))
            {
                Console.WriteLine("  Invalid course ID.");
                Pause("Press any key to go back..."); return;
            }

            // Step 3: Find the Course object — validate it actually exists
            Course course = courses.Find(c => c.CourseId == courseId);
            if (course == null)
            {
                Console.WriteLine("  Course not found.");
                Pause("Press any key to go back..."); return;
            }

            // Step 4: Call instructor.AddCourse() — adds to CourseIds if not already present
            instructor.AddCourse(courseId);
            Pause("Press any key to continue...");
        }

        // Displays total unique student count across all instructor's courses
        static void ShowInstructorStudentCount(Instructor instructor)
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("      My Student Count       ");
            Console.WriteLine("  Instructor: " + instructor.Username);
            Console.WriteLine("==============================");

            // Call Instructor's GetStudentCount() — counts unique students across all courses
            int count = instructor.GetStudentCount(enrollments);
            Console.WriteLine("  Total students in your courses: " + count);
            Console.WriteLine("==============================");
            Pause("Press any key to go back...");
        }

        // Displays all courses in the system — used by AddInstructorCourse and enrollment
        // Fixed: uses CourseName not Title
        static void DisplayAllCourses()
        {
            Console.WriteLine("\n=== ALL COURSES ===");

            foreach (Course course in courses)
            {
                Console.WriteLine($"\n[{course.CourseId}] {course.CourseName}");
                Console.WriteLine($"  Category    : {course.Category}");
                Console.WriteLine($"  Instructor  : {course.InstructorName}");
                Console.WriteLine($"  Enrollment  : {course.CurrentEnrollments}/{course.MaxStudents}");
                Console.WriteLine("  ---");
            }
            Pause("Press any key to go back...");
        }

        // ==================== ADMIN DASHBOARD ====================

        // Takes typed Admin parameter — accesses Admin-specific methods
        static void ShowAdminDashboard(Admin admin)
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("       Admin Dashboard       ");
            Console.WriteLine("  Hello, " + admin.Username + "!");
            Console.WriteLine("==============================");
            Console.WriteLine("1. View All Users");
            Console.WriteLine("2. System Statistics");
            Console.WriteLine("3. Deactivate a User");
            Console.WriteLine("4. Manage Courses");
            Console.WriteLine("5. Logout");
            Console.WriteLine("==============================");
            Console.Write("Enter your choice (1-5): ");

            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > 5)
            {
                ShowError("Invalid input! Please enter 1 to 5.");
                Pause("Press any key to try again..."); return;
            }

            if (choice == 1) admin.ViewAllUsers(users);
            else if (choice == 2) admin.GetSystemStats(users, courses, enrollments);
            else if (choice == 3) DeactivateUserAsAdmin(admin);
            else if (choice == 5) Logout();
            else ShowComingSoon();

            if (choice != 5) Pause("Press any key to continue...");
        }

        // ==================== ADMIN HELPER METHOD ====================

        // Admin deactivates a user account — searches by username, validates before acting
        static void DeactivateUserAsAdmin(Admin admin)
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("       Deactivate User       ");
            Console.WriteLine("  Admin: " + admin.Username);
            Console.WriteLine("==============================");

            // Step 1: Show all users using Admin's ViewAllUsers() method
            admin.ViewAllUsers(users);

            // Step 2: Get username from admin input
            Console.Write("\nEnter username to deactivate: ");
            string username = Console.ReadLine()?.Trim();

            // Step 3: Find the User object in users list
            User userToDeactivate = users.Find(u => u.Username == username);

            if (userToDeactivate == null)
            {
                Console.WriteLine("  User not found.");
                Pause("Press any key to go back..."); return;
            }

            // Step 4: Validate — admin cannot deactivate their own account
            if (userToDeactivate == currentUser)
            {
                Console.WriteLine("  You cannot deactivate yourself!");
                Pause("Press any key to go back..."); return;
            }

            // Step 5: Call Admin's DeactivateUser() with the found User object
            admin.DeactivateUser(userToDeactivate);
            Pause("Press any key to continue...");
        }

        // ==================== HELPERS ====================

        static void ShowComingSoon()
        {
            Console.WriteLine("\n  Feature coming soon.");
            Pause("Press any key to continue...");
        }

        static void ShowError(string message)
        {
            Console.WriteLine("\n  ERROR: " + message);
        }

        static void Pause(string message)
        {
            Console.WriteLine(message);
            Console.ReadKey();
        }

        static void ExitApp()
        {
            Console.WriteLine("\nGoodbye! Thank you for using SmartLearn.");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}