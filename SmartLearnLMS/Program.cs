using System;
using System.Collections.Generic;

namespace SmartLearn
{
    class Program
    {
        // ==================== WEEK 2 DATA STRUCTURES ====================

        // Lists of class objects — Week 2 approach
        static List<User> users = new List<User>();
        static List<Course> courses = new List<Course>();
        static List<Enrollment> enrollments = new List<Enrollment>();

        // ==================== SESSION STATE ====================

        static bool isLoggedIn = false;
        static User currentUser = null;

        // ==================== ENTRY POINT ====================

        static void Main(string[] args)
        {
            // Seed test accounts into the users List
            //SeedUser("admin", "admin123", "admin@smartlearn.com", "Admin");
            //SeedUser("student1", "pass123", "student@smartlearn.com", "Student");
            //SeedUser("instructor1", "pass123", "instructor@smartlearn.com", "Instructor");

            // Load sample courses into the courses List
            LoadSampleCourses();

            // Main program loop: routes based on login state
            while (true)
            {
                if (!isLoggedIn)
                    ShowMainMenu();
                else
                    ShowDashboard();
            }
        }

        // Adds a seeded User object directly to the users List
        //static void SeedUser(string username, string password, string email, string role)
        //{
           // users.Add(new User(username, password, email, role));
       // }

        // ==================== LOAD SAMPLE COURSES ====================

        // Populates the courses List with realistic Course objects
        // Constructor: Course(courseId, courseName, description, instructorName, category, maxStudents)
        static void LoadSampleCourses()
        {
            courses.Clear();

            courses.Add(new Course(1, "C# Programming Fundamentals",
                "Learn the basics of C# including variables, loops, and OOP",
                "Prof. Smith", "Programming", 50, 25));

            courses.Add(new Course(2, "Introduction to SQL Server",
                "Database fundamentals including queries, joins, and stored procedures",
                "Prof. Johnson", "Database", 50, 20));

            courses.Add(new Course(3, "Web Development with ASP.NET Core",
                "Build modern web applications using ASP.NET Core MVC",
                "Prof. Williams", "Web Development", 50, 30));

            courses.Add(new Course(4, "Machine Learning with Python",
                "Introduction to ML algorithms, data preprocessing, and model training",
                "Prof. Davis", "Data Science", 50, 20));

            courses.Add(new Course(5, "Cloud Computing with Azure",
                "Deploy and manage applications on Microsoft Azure cloud platform",
                "Prof. Martinez", "Cloud", 50, 15));

            courses.Add(new Course(6, "Cybersecurity Essentials",
                "Learn ethical hacking, network security, and threat prevention",
                "Prof. Brown", "Security", 50, 18));

            courses.Add(new Course(7, "Mobile App Development with Flutter",
                "Build cross-platform mobile apps for iOS and Android using Flutter",
                "Prof. Taylor", "Mobile", 50, 22));
        }

        // ==================== MAIN MENU (Not Logged In) ====================

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

        // Routes logged-in user to their role-specific dashboard
        static void ShowDashboard()
        {
            if (currentUser.Role == "Student") ShowStudentDashboard();
            else if (currentUser.Role == "Instructor") ShowInstructorDashboard();
            else if (currentUser.Role == "Admin") ShowAdminDashboard();
        }

        // ==================== REGISTER USER ====================

        // Handles complete registration flow — collects and validates all fields
        static void RegisterUser()
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("       User Registration     ");
            Console.WriteLine("==============================");

            // Step 1: Get and validate username
            Console.Write("  Enter Username: ");
            string username = Console.ReadLine()?.Trim();

            if (!ValidateUsername(username))
            {
                Pause("Press any key to go back..."); return;
            }

            // Step 2: Check if username already exists in the users List
            User existingUser = users.Find(u => u.Username.ToLower() == username.ToLower());
            if (existingUser != null)
            {
                ShowError("Username '" + username + "' already exists. Please choose another.");
                Pause("Press any key to go back..."); return;
            }

            // Step 3: Get and validate password
            Console.Write("  Enter Password: ");
            string password = Console.ReadLine();

            if (!ValidatePassword(password))
            {
                Pause("Press any key to go back..."); return;
            }

            // Step 3b: Confirm password matches
            Console.Write("  Confirm Password: ");
            string confirmPassword = Console.ReadLine();

            if (confirmPassword != password)
            {
                ShowError("Passwords do not match. Please try again.");
                Pause("Press any key to go back..."); return;
            }

            // Step 4: Get and validate email
            Console.Write("  Enter Email: ");
            string email = Console.ReadLine()?.Trim();

            if (!ValidateEmail(email))
            {
                Pause("Press any key to go back..."); return;
            }

            // Step 5: Get and validate role
            Console.WriteLine("  Select Role:  1. Student   2. Instructor   3. Admin");
            Console.Write("  Enter Role (1-3): ");
            string roleInput = Console.ReadLine()?.Trim();
            string role = GetRoleName(roleInput);

            if (role == null)
            {
                ShowError("Invalid role. Please enter 1, 2, or 3.");
                Pause("Press any key to go back..."); return;
            }

            // Step 6: Create a new User object with the validated data
            User newUser = new User(username, password, email, role);

            // Step 7: Add the new User object to the users List
            users.Add(newUser);

            // Step 8: Display success message using User.DisplayInfo()
            Console.WriteLine("\n  ✔ Registration Successful!");
            Console.WriteLine("  ------------------------------");
            newUser.DisplayInfo();
            Console.WriteLine("  ------------------------------");
            Console.WriteLine("  You can now log in with your credentials.");
            Pause("Press any key to continue...");
        }

        // Converts role number input to role name string. Returns null if invalid.
        static string GetRoleName(string roleInput)
        {
            if (roleInput == "1") return "Student";
            if (roleInput == "2") return "Instructor";
            if (roleInput == "3") return "Admin";
            return null;
        }

        // ==================== VALIDATION METHODS ====================

        // Validates username: not empty, length 3-20, no spaces
        static bool ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                ShowError("Username cannot be empty."); return false;
            }
            if (username.Length < 3)
            {
                ShowError("Username must be at least 3 characters long."); return false;
            }
            if (username.Length > 20)
            {
                ShowError("Username cannot exceed 20 characters."); return false;
            }
            if (username.Contains(" "))
            {
                ShowError("Username cannot contain spaces."); return false;
            }
            return true;
        }

        // Validates password: not empty, minimum 6 characters, max 30 characters
        static bool ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Password cannot be empty."); return false;
            }
            if (password.Length < 6)
            {
                ShowError("Password must be at least 6 characters long."); return false;
            }
            if (password.Length > 30)
            {
                ShowError("Password cannot exceed 30 characters."); return false;
            }
            return true;
        }

        // Validates email: not empty, must have @ and dot, not already registered
        static bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ShowError("Email cannot be empty."); return false;
            }
            if (!email.Contains("@"))
            {
                ShowError("Invalid email. Email must contain '@'."); return false;
            }
            if (!email.Contains("."))
            {
                ShowError("Invalid email. Email must contain a '.' after '@'."); return false;
            }

            // Search the users List to detect duplicate email registrations
            User emailMatch = users.Find(u => u.Email.ToLower() == email.ToLower());
            if (emailMatch != null)
            {
                ShowError("Email '" + email + "' is already registered."); return false;
            }
            return true;
        }

        // ==================== LOGIN ====================

        // Handles complete login flow: searches users List, validates password, sets session state
        static void Login()
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("            Login            ");
            Console.WriteLine("==============================");

            // Step 1: Get username from user input
            Console.Write("  Username: ");
            string username = Console.ReadLine()?.Trim();

            // Step 2: Search users List for a User with matching username
            User foundUser = users.Find(u => u.Username.ToLower() == username.ToLower());

            // Step 3: If not found, show error and return
            if (foundUser == null)
            {
                ShowError("Username '" + username + "' not found. Please register first.");
                Pause("Press any key to go back..."); return;
            }

            // Step 4: Get password from user input
            Console.Write("  Password: ");
            string password = Console.ReadLine();

            // Step 5: Use the User's ValidatePassword() method to verify
            if (!foundUser.ValidatePassword(password))
            {
                ShowError("Incorrect password. Please try again.");
                Pause("Press any key to go back..."); return;
            }

            // Step 6: Set session state — store the full User object
            isLoggedIn = true;
            currentUser = foundUser;

            // Step 7: Display welcome message with user's role
            Console.WriteLine("\n  ✔ Login successful! Welcome back, " + currentUser.Username + ".");
            Console.WriteLine("  Role     : " + currentUser.Role);
            Console.WriteLine("  Redirecting to your dashboard...");
            Pause("Press any key to continue...");
        }

        // ==================== LOGOUT ====================

        // Resets all session state and returns user to the login/register menu
        static void Logout()
        {
            string name = currentUser.Username;

            // Reset session state
            isLoggedIn = false;
            currentUser = null;

            Console.WriteLine("\n  ✔ Logged out successfully. Goodbye, " + name + "!");
            Pause("Press any key to continue...");
        }

        // ==================== COURSE BROWSING ====================

        // Course browsing sub-menu — loops until user chooses Back
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

        // Displays all courses from the courses List using a for loop
        static void DisplayAllCourses()
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("       Available Courses     ");
            Console.WriteLine("==============================");

            for (int i = 0; i < courses.Count; i++)
            {
                Course c = courses[i];
                Console.WriteLine("  " + (i + 1) + ". [" + c.Category + "] " + c.CourseName);
                Console.WriteLine("       Instructor : " + c.InstructorName);
                Console.WriteLine("       Spots Left : " + (c.MaxStudents - c.CurrentEnrollments) + "/" + c.MaxStudents);
            }

            Console.WriteLine("==============================");
            Pause("Press any key to go back...");
        }

        // Searches courses List using case-insensitive CourseName and Category matching
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
                    ShowError("Keyword cannot be empty. Please try again.");
                }
                else
                {
                    Console.WriteLine("\n------------------------------");
                    Console.WriteLine("  Search Results for: \"" + keyword + "\"");
                    Console.WriteLine("------------------------------");

                    int matchCount = 0;

                    // Search against CourseName and Category using ToLower() + Contains()
                    foreach (Course course in courses)
                    {
                        if (course.CourseName.ToLower().Contains(keyword.ToLower()) ||
                            course.Category.ToLower().Contains(keyword.ToLower()))
                        {
                            matchCount++;
                            Console.WriteLine("  " + matchCount + ". " + course.CourseName +
                                              " [" + course.Category + "]");
                        }
                    }

                    if (matchCount == 0)
                        Console.WriteLine("  No results found. Try a different keyword.");
                    else
                        Console.WriteLine("\n  " + matchCount + " course(s) found.");

                    Console.WriteLine("------------------------------");
                }

                Console.Write("\nSearch again? (y/n): ");
                string again = Console.ReadLine();
                if (again != null) again = again.Trim().ToLower();
                searchAgain = (again == "y") ? 'y' : 'n';

            } while (searchAgain == 'y');
        }

        // ==================== BROWSE AND ENROLL ====================

        // Displays all courses and allows a Student to enroll
        // Only accessible to users with Role == "Student"
        static void BrowseAndEnrollCourses()
        {
            // Role check — only students can enroll
            if (currentUser.Role != "Student")
            {
                ShowError("Only students can enroll in courses.");
                Pause("Press any key to go back..."); return;
            }

            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("      Browse & Enroll        ");
            Console.WriteLine("==============================");

            // Step 1: Display all courses with number, title, category, enrollment status
            for (int i = 0; i < courses.Count; i++)
            {
                Course c = courses[i];
                string status = c.CanEnroll()
                    ? "(" + c.CurrentEnrollments + "/" + c.MaxStudents + " enrolled)"
                    : "(FULL)";
                Console.WriteLine("  " + (i + 1) + ". [" + c.Category + "] " + c.CourseName + " " + status);
            }

            Console.WriteLine("==============================");

            // Step 2: Ask student to enter course number
            Console.Write("  Enter course number to enroll (0 to cancel): ");
            string input = Console.ReadLine();

            // Step 3: Validate the course number
            if (!int.TryParse(input, out int courseNumber) || courseNumber == 0)
            {
                ShowError("Enrollment cancelled.");
                Pause("Press any key to go back..."); return;
            }

            if (courseNumber < 1 || courseNumber > courses.Count)
            {
                ShowError("Invalid course number. Please enter a number between 1 and " + courses.Count + ".");
                Pause("Press any key to go back..."); return;
            }

            // Get selected course (list is 0-indexed, input is 1-indexed)
            Course selectedCourse = courses[courseNumber - 1];

            // Step 4: Check if course can accept enrollments using CanEnroll()
            if (!selectedCourse.CanEnroll())
            {
                ShowError("Sorry, '" + selectedCourse.CourseName + "' is full and cannot accept new enrollments.");
                Pause("Press any key to go back..."); return;
            }

            // Step 5: Check if student is already enrolled in this course
            Enrollment existing = enrollments.Find(e =>
                e.StudentUsername == currentUser.Username &&
                e.CourseId == selectedCourse.CourseId);

            if (existing != null)
            {
                ShowError("You are already enrolled in '" + selectedCourse.CourseName + "'.");
                Pause("Press any key to go back..."); return;
            }

            // Step 6: Create new Enrollment object
            int enrollmentId = enrollments.Count + 1;
            Enrollment newEnrollment = new Enrollment(enrollmentId,
                                                      currentUser.Username,
                                                      selectedCourse.CourseId);

            // Step 7: Add to enrollments list
            enrollments.Add(newEnrollment);

            // Step 8: Call course's IncrementEnrollment() to update count
            selectedCourse.IncrementEnrollment();

            // Step 9: Display success message
            Console.WriteLine("\n  ✔ Enrollment Successful!");
            Console.WriteLine("  ------------------------------");
            Console.WriteLine("  Course   : " + selectedCourse.CourseName);
            Console.WriteLine("  Category : " + selectedCourse.Category);
            Console.WriteLine("  Student  : " + currentUser.Username);
            newEnrollment.DisplayInfo();
            Console.WriteLine("  ------------------------------");
            Pause("Press any key to continue...");
        }

        // ==================== MY ENROLLED COURSES ====================

        // Shows all courses the current student is enrolled in
        static void ShowMyEnrolledCourses()
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("     My Enrolled Courses     ");
            Console.WriteLine("  Student: " + currentUser.Username);
            Console.WriteLine("==============================");

            // Use FindAll to get all enrollments belonging to the current student
            List<Enrollment> myEnrollments = enrollments.FindAll(
                e => e.StudentUsername == currentUser.Username);

            // Step 3: If no enrollments found, display friendly message
            if (myEnrollments.Count == 0)
            {
                Console.WriteLine("\n  You haven't enrolled in any courses yet.");
                Console.WriteLine("  Go to 'Browse & Enroll Courses' to get started!");
                Console.WriteLine("==============================");
                Pause("Press any key to go back..."); return;
            }

            // Loop through each enrollment the student has
            int displayNumber = 1;
            foreach (Enrollment enrollment in myEnrollments)
            {
                // Find the matching Course from the courses List using CourseId
                Course course = courses.Find(c => c.CourseId == enrollment.CourseId);

                Console.WriteLine("  " + displayNumber + ". " + (course != null ? course.CourseName : "Unknown Course"));
                Console.WriteLine("       Enrollment ID : " + enrollment.EnrollmentId);
                Console.WriteLine("       Enrolled On   : " + enrollment.EnrollmentDate.ToString("dd-MM-yyyy"));
                Console.WriteLine("       Progress      : " + enrollment.ProgressPercentage + "%");
                Console.WriteLine("       Completed     : " + (enrollment.IsCompleted ? "Yes ✔" : "No"));

                // Show category and instructor if course was found
                if (course != null)
                {
                    Console.WriteLine("       Category      : " + course.Category);
                    Console.WriteLine("       Instructor    : " + course.InstructorName);
                }

                Console.WriteLine("  ------------------------------");
                displayNumber++;
            }

            Console.WriteLine("  Total Enrolled: " + myEnrollments.Count + " course(s)");
            Console.WriteLine("==============================");
            Pause("Press any key to go back...");
        }

        // ==================== DASHBOARDS ====================

        // Student dashboard — Browse & Enroll replaces plain Browse for logged-in students
        static void ShowStudentDashboard()
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("     Student Dashboard       ");
            Console.WriteLine("  Hello, " + currentUser.Username + "!");
            Console.WriteLine("==============================");
            Console.WriteLine("1. Browse & Enroll Courses");
            Console.WriteLine("2. My Courses");
            Console.WriteLine("3. Progress");
            Console.WriteLine("4. Take Quiz");
            Console.WriteLine("5. Logout");
            Console.WriteLine("==============================");
            Console.Write("Enter your choice (1-5): ");

            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > 5)
            {
                ShowError("Invalid input! Please enter 1 to 5.");
                Pause("Press any key to try again..."); return;
            }

            if (choice == 1) BrowseAndEnrollCourses();
            else if (choice == 2) ShowMyEnrolledCourses();
            else if (choice == 5) Logout();
            else ShowComingSoon();
        }

        // Instructor dashboard
        static void ShowInstructorDashboard()
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("    Instructor Dashboard     ");
            Console.WriteLine("  Hello, " + currentUser.Username + "!");
            Console.WriteLine("==============================");
            Console.WriteLine("1. My Courses");
            Console.WriteLine("2. Create Course");
            Console.WriteLine("3. View Students");
            Console.WriteLine("4. Grade");
            Console.WriteLine("5. Logout");
            Console.WriteLine("==============================");
            Console.Write("Enter your choice (1-5): ");

            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > 5)
            {
                ShowError("Invalid input! Please enter 1 to 5.");
                Pause("Press any key to try again..."); return;
            }

            if (choice == 5) Logout();
            else ShowComingSoon();
        }

        // Admin dashboard
        static void ShowAdminDashboard()
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("       Admin Dashboard       ");
            Console.WriteLine("  Hello, " + currentUser.Username + "!");
            Console.WriteLine("==============================");
            Console.WriteLine("1. Manage Users");
            Console.WriteLine("2. Manage Courses");
            Console.WriteLine("3. Reports");
            Console.WriteLine("4. Settings");
            Console.WriteLine("5. Logout");
            Console.WriteLine("==============================");
            Console.Write("Enter your choice (1-5): ");

            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > 5)
            {
                ShowError("Invalid input! Please enter 1 to 5.");
                Pause("Press any key to try again..."); return;
            }

            if (choice == 5) Logout();
            else ShowComingSoon();
        }

        // ==================== HELPERS ====================

        static void ShowComingSoon()
        {
            Console.WriteLine("\n  Feature coming in Week 2.");
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