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
            // Load sample courses first — must exist before anything references them
            InitializeCourses();

            // Seed typed objects — Student, Instructor, Admin (not plain User)
            users.Add(new Student("student1",    "pass123",  "student@smartlearn.com"));
            users.Add(new Instructor("instructor1", "pass123", "instructor@smartlearn.com"));
            users.Add(new Admin("admin",          "admin123", "admin@smartlearn.com"));

            // Assign courses to the seeded instructor after courses are loaded
            Instructor seededInstructor = users.Find(u => u is Instructor) as Instructor;
            if (seededInstructor != null)
            {
                seededInstructor.AddCourse(101);
                seededInstructor.AddCourse(102);
                seededInstructor.AddCourse(103);
            }

            // Main program loop
            while (true)
            {
                if (!isLoggedIn)
                    ShowMainMenu();
                else
                    ShowRoleMenu();
            }
        }

        // ==================== ROLE MENU ====================
        // Polymorphic — currentUser.DisplayDashboard() automatically calls the correct
        // override (StudentDashboard / InstructorDashboard / AdminDashboard).
        // No if-else or type-checking needed here at all.

        static void ShowRoleMenu()
        {
            Console.Clear();
            currentUser.DisplayDashboard();   // polymorphic call — no if-else
            HandleMenuChoice();               // reads input and routes for whoever is logged in
        }

        // Routes the menu choice to the correct handler based on the logged-in user type.
        // The cast is safe because we already know who is logged in from the Login() step.
        static void HandleMenuChoice()
        {
            Console.Write("\nEnter your choice (1-5): ");

            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > 5)
            {
                ShowError("Invalid input! Please enter 1-5.");
                Pause("Press any key to try again..."); return;
            }

            if (currentUser is Student student)
            {
                if      (choice == 1) BrowseCourses();
                else if (choice == 2) ShowMyEnrolledCourses(student);
                else if (choice == 3) UpdateStudentProgress(student);
                else if (choice == 4) ShowStudentStats(student);
                else if (choice == 5) Logout();
            }
            else if (currentUser is Instructor instructor)
            {
                if      (choice == 1) ShowInstructorCourses(instructor);
                else if (choice == 2) AddInstructorCourse(instructor);
                else if (choice == 3) ShowInstructorStudents(instructor);
                else if (choice == 4) ShowInstructorStudentCount(instructor);
                else if (choice == 5) Logout();
            }
            else if (currentUser is Admin admin)
            {
                if      (choice == 1) admin.ViewAllUsers(users);
                else if (choice == 2) DeactivateUserAsAdmin(admin);
                else if (choice == 3) admin.GetSystemStats(users, courses, enrollments);
                else if (choice == 4) UniversalSearch(users, courses);
                else if (choice == 5) Logout();
            }
        }

        // ==================== LOAD SAMPLE COURSES ====================

        static void InitializeCourses()
        {
            courses.Clear();

            // ── Online Courses (IDs 101–105) ─────────────────────────────────
            courses.Add(new OnlineCourse(101,
                "C# Fundamentals",
                "Master C# syntax, OOP principles, and the .NET ecosystem",
                "Prof. Smith", "Programming", 450));

            courses.Add(new OnlineCourse(102,
                "Python for Beginners",
                "Start coding with Python — variables, loops, functions, and files",
                "Prof. Johnson", "Programming", 360));

            courses.Add(new OnlineCourse(103,
                "Web Development Basics",
                "Build websites with HTML, CSS, and JavaScript from scratch",
                "Prof. Williams", "Web Development", 540));

            courses.Add(new OnlineCourse(104,
                "Data Structures",
                "Arrays, linked lists, stacks, queues, trees, and graphs in depth",
                "Prof. Davis", "Computer Science", 600));

            courses.Add(new OnlineCourse(105,
                "Machine Learning Intro",
                "Supervised and unsupervised learning, model evaluation, and scikit-learn",
                "Prof. Martinez", "Data Science", 720));

            // ── In-Person Courses (IDs 201–203) ──────────────────────────────
            courses.Add(new InPersonCourse(201,
                "Database Design Workshop",
                "Relational modelling, normalisation, SQL, and stored procedures",
                "Prof. Brown", "Database",
                25, "B-101", "Engineering Building"));

            courses.Add(new InPersonCourse(202,
                "Network Security Lab",
                "Hands-on ethical hacking, penetration testing, and defence strategies",
                "Prof. Taylor", "Security",
                20, "C-205", "CS Building"));

            courses.Add(new InPersonCourse(203,
                "Mobile App Development",
                "Build iOS and Android apps using Flutter and Dart in a lab setting",
                "Prof. Anderson", "Mobile",
                30, "A-301", "Tech Center"));

            // ── Hybrid Courses (IDs 301–302) ─────────────────────────────────
            courses.Add(new HybridCourse(301,
                "Full-Stack Development",
                "React front-end meets ASP.NET Core back-end — build real projects",
                "Prof. Thompson", "Web Development",
                30, 720, "C-201", "CS Building"));

            courses.Add(new HybridCourse(302,
                "Cloud Computing",
                "Azure and AWS fundamentals with hands-on lab deployments",
                "Prof. Garcia", "Cloud",
                25, 600, "D-101", "Engineering Building"));
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

            // Step 3: Get password — User.cs setter validates length and digit requirement
            Console.Write("  Enter Password: ");
            string password = Console.ReadLine();

            // Step 4: Confirm password match before creating object
            Console.Write("  Confirm Password: ");
            string confirmPassword = Console.ReadLine();

            if (confirmPassword != password)
            {
                ShowError("Passwords do not match.");
                Pause("Press any key to go back..."); return;
            }

            // Step 5: Get email — User.cs setter validates @ symbol
            Console.Write("  Enter Email: ");
            string email = Console.ReadLine()?.Trim();

            // Step 5a: Check email uniqueness here — User.cs has no access to the users list
            User emailMatch = users.Find(u => u.Email != null && u.Email.ToLower() == email.ToLower());
            if (emailMatch != null)
            {
                ShowError("Email '" + email + "' is already registered.");
                Pause("Press any key to go back..."); return;
            }

            // Step 6: Get role choice
            Console.WriteLine("  Select Role:  1. Student   2. Instructor   3. Admin");
            Console.Write("  Enter Role (1-3): ");
            string roleInput = Console.ReadLine()?.Trim();

            // Step 7: Create the correct typed object — constructors call User setters internally
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

            // Step 8: If setter rejected password or email, backing field will be null — abort
            if (newUser.Password == null || newUser.Email == null)
            {
                ShowError("Registration failed. Please fix the errors above and try again.");
                Pause("Press any key to go back..."); return;
            }

            // Step 9: Add to users list and confirm
            users.Add(newUser);

            Console.WriteLine("\n  ✔ Registration Successful!");
            Console.WriteLine("  ------------------------------");
            newUser.DisplayInfo();
            Console.WriteLine("  ------------------------------");
            Console.WriteLine("  You can now log in with your credentials.");
            Pause("Press any key to continue...");
        }

        // ==================== VALIDATION METHODS ====================
        // Note: Password and Email rules live in User.cs setters — no duplication here
        // Username is validated here because uniqueness requires access to the users list

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

            // Step 4: Show welcome message — Role is already set by the typed constructor
            Console.WriteLine("\n  ✔ Login successful! Welcome back, " + currentUser.Username + ".");
            Console.WriteLine("  Account Type : " + currentUser.Role);
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
                Console.WriteLine("╔════════════════════════════════╗");
                Console.WriteLine("║        Browse Courses          ║");
                Console.WriteLine("╚════════════════════════════════╝");
                Console.WriteLine("[1] View All Courses");
                Console.WriteLine("[2] Search Courses");
                Console.WriteLine("[3] Enroll in a Course");
                Console.WriteLine("[4] Back");
                Console.WriteLine("================================");
                Console.Write("Enter your choice (1-4): ");

                string input = Console.ReadLine();

                if (!int.TryParse(input, out choice) || choice < 1 || choice > 4)
                {
                    ShowError("Invalid input! Please enter 1, 2, 3, or 4.");
                    Pause("Press any key to try again..."); continue;
                }

                if      (choice == 1) DisplayAllCourses();
                else if (choice == 2) SearchCourses();
                else if (choice == 3)
                {
                    // Only students can enroll
                    if (isLoggedIn && currentUser is Student student)
                        EnrollStudentInCourse(student);
                    else
                    {
                        ShowError("You must be logged in as a Student to enroll.");
                        Pause("Press any key to go back...");
                    }
                }

            } while (choice != 4);
        }

        static void SearchCourses()
        {
            char searchAgain;
            do
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════╗");
                Console.WriteLine("║        Search Courses          ║");
                Console.WriteLine("╚════════════════════════════════╝");
                Console.Write("  Enter keyword to search: ");

                string keyword = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    ShowError("Keyword cannot be empty.");
                }
                else
                {
                    // Use SearchEngine with ISearchable — Course already implements it
                    List<ISearchable> searchable = new List<ISearchable>();
                    foreach (Course c in courses) searchable.Add(c);

                    List<ISearchable> results = SearchEngine.Search(searchable, keyword);

                    Console.WriteLine("\n  Results for: \"" + keyword + "\"");
                    SearchEngine.DisplayResults(results);
                }

                Console.Write("\nSearch again? (y/n): ");
                string again = Console.ReadLine()?.Trim().ToLower();
                searchAgain = (again == "y") ? 'y' : 'n';

            } while (searchAgain == 'y');
        }

        

        // ==================== ENROLL STUDENT IN COURSE ====================

        // Fully polymorphic — calls course.DisplayCourseInfo(), course.CanEnroll(), course.Enroll()
        // No type-checking or if/else based on course type anywhere in this method
        static void EnrollStudentInCourse(Student student)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║       Available Courses        ║");
            Console.WriteLine("╚════════════════════════════════╝");

            // Polymorphic display — each course type renders its own format
            foreach (Course course in courses)
            {
                course.DisplayCourseInfo();

                // Mark already-enrolled courses for the student's awareness
                if (student.EnrolledCourseIds.Contains(course.CourseId))
                    Console.WriteLine("  ✔ You are already enrolled in this course.");

                // Polymorphic enrollment status — no type-checking needed
                if (!course.CanEnroll(student))
                    Console.WriteLine("  ✗ Full — enrollment closed.");

                Console.WriteLine();
            }

            Console.Write("Enter Course ID to enroll (0 to cancel): ");

            if (!int.TryParse(Console.ReadLine(), out int courseId) || courseId == 0)
            {
                Pause("Press any key to go back..."); return;
            }

            Course selectedCourse = courses.Find(c => c.CourseId == courseId);

            if (selectedCourse == null)
            {
                ShowError("Course ID " + courseId + " not found.");
                Pause("Press any key to go back..."); return;
            }

            // Polymorphic CanEnroll — each course type enforces its own rules
            // Note: already-enrolled check handled inside student.EnrollInCourse()
            if (!selectedCourse.CanEnroll(student))
            {
                ShowError("This course is full and cannot accept new enrollments.");
                Pause("Press any key to go back..."); return;
            }

            // Polymorphic Enroll — handles count increment, student.EnrollInCourse(), notification
            selectedCourse.Enroll(student);

            // Create Enrollment record to track progress and date
            int enrollmentId = enrollments.Count + 1;
            enrollments.Add(new Enrollment(enrollmentId, student.Username, courseId));

            // Notify instructor if one is assigned to this course
            Instructor courseInstructor = users.Find(u =>
                u is Instructor i && i.CourseIds.Contains(courseId)) as Instructor;
            if (courseInstructor != null)
                courseInstructor.NotifyEnrollment(student.Username, courseId);

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

        // Builds a simple text progress bar e.g. [████████░░] 80%
        static string BuildProgressBar(int percentage)
        {
            int filled = percentage / 10;
            int empty  = 10 - filled;
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

            // Step 3: Get progress percentage — Student.ProgressPercentage setter validates 0-100
            Console.Write("Enter progress percentage (0-100): ");
            if (!int.TryParse(Console.ReadLine(), out int progress))
            {
                Console.WriteLine("  Invalid input. Please enter a number.");
                Pause("Press any key to go back..."); return;
            }

            // Step 4: Update progress — setter rejects out-of-range values with error message
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
                if (course.CurrentEnrollments > 0) course.CurrentEnrollments--;
            }

            // Step 5: Find matching Enrollment record and remove it from the global list
            Enrollment enrollment = enrollments.Find(e =>
                e.StudentUsername == student.Username &&
                e.CourseId == courseId);

            if (enrollment != null)
                enrollments.Remove(enrollment);

            Console.WriteLine("\n  ✔ Drop complete for '" + courseName + "'.");

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
            Console.WriteLine("\n  Available Courses:");
            foreach (Course c in courses)
                Console.WriteLine($"    [{c.CourseId}] {c.CourseName} ({c.GetCourseType()})");
            Console.WriteLine();

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

        // Polymorphic display — each course type calls its own DisplayCourseInfo()
        static void DisplayAllCourses()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║          All Courses           ║");
            Console.WriteLine("╚════════════════════════════════╝");
            Console.WriteLine("  Total: " + courses.Count + " course(s)");
            Console.WriteLine();

            foreach (Course course in courses)
            {
                course.DisplayCourseInfo();   // polymorphic — Online/InPerson/Hybrid each render differently
                Console.WriteLine();
            }

            Pause("Press any key to go back...");
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

            // Step 4: Call Admin's DeactivateUser() — it handles self-deactivation guard internally
            admin.DeactivateUser(userToDeactivate);
            Pause("Press any key to continue...");
        }

        // ==================== UNIVERSAL SEARCH ====================
 
        // Searches across both courses and students in one combined operation
        static void UniversalSearch(List<User> users, List<Course> courses)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║       Universal Search         ║");
            Console.WriteLine("╚════════════════════════════════╝");
 
            // Step 1: Build combined ISearchable list — all courses first
            List<ISearchable> searchItems = new List<ISearchable>();
 
            foreach (Course course in courses)
                searchItems.Add(course);
 
            // Step 2: Add users that implement ISearchable (Students only)
            foreach (User user in users)
            {
                if (user is ISearchable searchableUser)
                    searchItems.Add(searchableUser);
            }
 
            // Step 3: Ask for keyword
            Console.Write("  Enter search keyword: ");
            string keyword = Console.ReadLine()?.Trim();
 
            if (string.IsNullOrWhiteSpace(keyword))
            {
                ShowError("Keyword cannot be empty.");
                Pause("Press any key to go back..."); return;
            }
 
            // Step 4 & 5: Search and display via SearchEngine
            Console.WriteLine("\n  Results for: \"" + keyword + "\"");
            List<ISearchable> results = SearchEngine.Search(searchItems, keyword);
            SearchEngine.DisplayResults(results);
 
            Pause("Press any key to go back...");
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