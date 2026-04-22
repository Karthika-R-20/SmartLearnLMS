using System;
using System.Collections.Generic;
using System.Linq;                   // LINQ — Where, OrderBy, OfType, etc.

namespace SmartLearn
{
    class Program
    {
        // ==================== DATA STRUCTURES ====================

        static List<User>       users       = new List<User>();
        static List<Course>     courses     = new List<Course>();
        static List<Enrollment> enrollments = new List<Enrollment>();

        // ==================== SESSION STATE ====================

        static bool isLoggedIn  = false;
        static User currentUser = null;

        // ==================== ENTRY POINT ====================

        static void Main(string[] args)
        {
        
            // Step 1: Load persisted data (students + extra courses) before seeding
            SmartLearnManager.LoadAllData(users, courses);

            // Step 2: Seed default courses only when no saved courses exist
            if (courses.Count == 0)
                InitializeCourses();

            // Step 3: Seed default users only if they were not loaded from file
            if (!users.Any(u => u.Username == "student1"))
                users.Add(new Student("student1", "pass123", "student@smartlearn.com"));
            if (!users.Any(u => u.Username == "instructor1"))
                users.Add(new Instructor("instructor1", "pass123", "instructor@smartlearn.com"));
            if (!users.Any(u => u.Username == "admin"))
                users.Add(new Admin("admin", "admin123", "admin@smartlearn.com"));

            // Step 4: Assign courses to seeded instructor — guard prevents duplicates
            Instructor seededInstructor = users.Find(u => u is Instructor) as Instructor;
            if (seededInstructor != null)
            {
                if (!seededInstructor.CourseIds.Contains(101)) seededInstructor.AddCourse(101);
                if (!seededInstructor.CourseIds.Contains(102)) seededInstructor.AddCourse(102);
                if (!seededInstructor.CourseIds.Contains(103)) seededInstructor.AddCourse(103);
            }

            // Step 5: Build dictionaries for instant O(1) lookups
            SmartLearnManager.RebuildDictionaries(users, courses);

            Console.WriteLine("==============================");
            Pause("  Press any key to enter SmartLearn...");
            

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

        static void ShowRoleMenu()
        {
            Console.Clear();
            currentUser.DisplayDashboard();   // polymorphic — calls correct subclass override
            HandleMenuChoice();
        }

        static void HandleMenuChoice()
        {
            // Max options differ per role: Student=7, Instructor=6, Admin=5
            int maxChoice = currentUser is Student ? 7 : currentUser is Instructor ? 7 : 5;
            Console.Write($"\nEnter your choice (1-{maxChoice}): ");

            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > maxChoice)
            {
                ShowError($"Invalid input! Please enter 1-{maxChoice}.");
                Pause("Press any key to try again..."); return;
            }

            if (currentUser is Student student)
            {
                if      (choice == 1) BrowseCourses();
                else if (choice == 2) ShowMyEnrolledCourses(student);
                else if (choice == 3) UpdateStudentProgress(student);
                else if (choice == 4) ShowStudentStats(student);
                else if (choice == 5) RateCourse(student);            // IRatable
                else if (choice == 6) ShowStudentNotifications(student); // INotifiable
                else if (choice == 7) Logout();
            }
            else if (currentUser is Instructor instructor)
            {
                if      (choice == 1) ShowInstructorCourses(instructor);
                else if (choice == 2) CreateCourse(instructor);
                else if (choice == 3) AddInstructorCourse(instructor);
                else if (choice == 4) ShowInstructorStudents(instructor);
                else if (choice == 5) GradeAssignments();
                else if (choice == 6) ShowInstructorNotifications(instructor);
                else if (choice == 7) Logout();
            }
            else if (currentUser is Admin admin)
            {
                if      (choice == 1) admin.ViewAllUsers(users);
                else if (choice == 2) DeactivateUserAsAdmin(admin);
                else if (choice == 3) admin.GetSystemStats(users, courses, enrollments);
                else if (choice == 4) ShowAnalyticsMenu();
                else if (choice == 5) Logout();
            }
        }

        // ==================== LOAD SAMPLE COURSES ====================

        static void InitializeCourses()
        {
            // courses.Clear() intentionally removed — only called when file had no data

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
            Console.WriteLine("===================================");
            Console.WriteLine("     Welcome to SmartLearn LMS     ");
            Console.WriteLine("==========Main Menu================");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Browse All Courses");
            Console.WriteLine("4. Browse All Users");
            Console.WriteLine("5. Universal Search");
            Console.WriteLine("6. Analytics & LINQ Demo");
            Console.WriteLine("7. Exit");
            Console.WriteLine("==============================");
            Console.Write("Enter your choice (1-7): ");

            string input = Console.ReadLine();

            if (!int.TryParse(input, out int choice) || choice < 1 || choice > 7)
            {
                ShowError("Invalid input! Please enter 1-7.");
                Pause("Press any key to try again..."); return;
            }

            if      (choice == 1) RegisterUser();
            else if (choice == 2) Login();
            else if (choice == 3) BrowseCourses();
            else if (choice == 4) BrowseAllUsers();
            else if (choice == 5) UniversalSearch(users, courses);
            else if (choice == 6) ShowAnalyticsMenu();
            else if (choice == 7) ExitApp();
        }

        // ==================== BROWSE ALL USERS (Main Menu Option 4) ====================

        static void BrowseAllUsers()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║         All Users              ║");
            Console.WriteLine("╚════════════════════════════════╝");
            Console.WriteLine($"  Total: {users.Count} user(s)");
            Console.WriteLine();

            int i = 1;
            foreach (User u in users)
            {
                string type = u is Student    ? "Student"    :
                              u is Instructor ? "Instructor" : "Admin";

                Console.WriteLine($"  {i++}. [{type}] {u.Username}");
                Console.WriteLine($"       Email  : {u.Email}");
                Console.WriteLine($"       Active : {(u.IsActive ? "Yes" : "No")}");
                Console.WriteLine("  ------------------------------");
            }

            Pause("Press any key to go back...");
        }

        // ==================== REGISTER USER ====================

        static void RegisterUser()
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("       User Registration     ");
            Console.WriteLine("==============================");

            Console.Write("  Enter Username: ");
            string username = Console.ReadLine()?.Trim();

            if (!ValidateUsername(username)) { Pause("Press any key to go back..."); return; }

            User existingUser = users.Find(u => u.Username.ToLower() == username.ToLower());
            if (existingUser != null)
            {
                ShowError("Username '" + username + "' already exists. Please choose another.");
                Pause("Press any key to go back..."); return;
            }

            Console.Write("  Enter Password: ");
            string password = Console.ReadLine();

            Console.Write("  Confirm Password: ");
            string confirmPassword = Console.ReadLine();

            if (confirmPassword != password)
            {
                ShowError("Passwords do not match.");
                Pause("Press any key to go back..."); return;
            }

            Console.Write("  Enter Email: ");
            string email = Console.ReadLine()?.Trim();

            User emailMatch = users.Find(u => u.Email != null && u.Email.ToLower() == email.ToLower());
            if (emailMatch != null)
            {
                ShowError("Email '" + email + "' is already registered.");
                Pause("Press any key to go back..."); return;
            }

            Console.WriteLine("  Select Role:  1. Student   2. Instructor   3. Admin");
            Console.Write("  Enter Role (1-3): ");
            string roleInput = Console.ReadLine()?.Trim();

            User newUser = null;

            if      (roleInput == "1") newUser = new Student(username, password, email);
            else if (roleInput == "2") newUser = new Instructor(username, password, email);
            else if (roleInput == "3") newUser = new Admin(username, password, email);
            else
            {
                ShowError("Invalid role. Please enter 1, 2, or 3.");
                Pause("Press any key to go back..."); return;
            }

            if (newUser.Password == null || newUser.Email == null)
            {
                ShowError("Registration failed. Please fix the errors above and try again.");
                Pause("Press any key to go back..."); return;
            }

            users.Add(newUser);

            // Auto-save after registration + keep dictionaries in sync
            SmartLearnManager.RebuildDictionaries(users, courses);
            SmartLearnManager.SaveAllData(users, courses);

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

        // ==================== LOGIN ====================

        static void Login()
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("            Login            ");
            Console.WriteLine("==============================");

            Console.Write("  Username: ");
            string username = Console.ReadLine()?.Trim();

            // Dictionary O(1) lookup — falls back to linear search for safety
            User foundUser = SmartLearnManager.GetUserByUsername(username)
                          ?? users.Find(u => u.Username.ToLower() == username?.ToLower());

            if (foundUser == null)
            {
                ShowError("Username '" + username + "' not found. Please register first.");
                Pause("Press any key to go back..."); return;
            }

            Console.Write("  Password: ");
            string password = Console.ReadLine();

            if (!foundUser.ValidatePassword(password))
            {
                ShowError("Incorrect password. Please try again.");
                Pause("Press any key to go back..."); return;
            }

            isLoggedIn  = true;
            currentUser = foundUser;

            Console.WriteLine("\n  ✔ Login successful! Welcome back, " + currentUser.Username + ".");
            Console.WriteLine("  Account Type : " + currentUser.Role);
            Pause("Press any key to continue...");
        }

        // ==================== LOGOUT ====================

        static void Logout()
        {
            string name = currentUser.Username;
            isLoggedIn  = false;
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
                else if (choice == 2) SearchCoursesMenu();
                else if (choice == 3)
                {
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

        // LINQ-powered course search (replaces old SearchEngine-only version)
        static void SearchCoursesMenu()
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
                    // LINQ search — CourseName + Description, sorted alphabetically
                    List<Course> results = SmartLearnManager.SearchCourses(courses, keyword);

                    Console.WriteLine($"\n  Results for: \"{keyword}\"");
                    SmartLearnManager.DisplaySearchResults(results);
                }

                Console.Write("\nSearch again? (y/n): ");
                string again  = Console.ReadLine()?.Trim().ToLower();
                searchAgain   = (again == "y") ? 'y' : 'n';

            } while (searchAgain == 'y');
        }

        // ==================== ENROLL STUDENT IN COURSE ====================

        static void EnrollStudentInCourse(Student student)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║       Available Courses        ║");
            Console.WriteLine("╚════════════════════════════════╝");

            foreach (Course course in courses)
            {
                course.DisplayCourseInfo();

                if (student.EnrolledCourseIds.Contains(course.CourseId))
                    Console.WriteLine("  ✔ You are already enrolled in this course.");

                if (!course.CanEnroll(student))
                    Console.WriteLine("  ✗ Full — enrollment closed.");

                Console.WriteLine();
            }

            Console.Write("Enter Course ID to enroll (0 to cancel): ");

            if (!int.TryParse(Console.ReadLine(), out int courseId) || courseId == 0)
            {
                Pause("Press any key to go back..."); return;
            }

            // O(1) dictionary lookup
            Course selectedCourse = SmartLearnManager.GetCourseById(courseId)
                                 ?? courses.Find(c => c.CourseId == courseId);

            if (selectedCourse == null)
            {
                ShowError("Course ID " + courseId + " not found.");
                Pause("Press any key to go back..."); return;
            }

            if (!selectedCourse.CanEnroll(student))
            {
                ShowError("This course is full and cannot accept new enrollments.");
                Pause("Press any key to go back..."); return;
            }

            selectedCourse.Enroll(student);

            int enrollmentId = enrollments.Count + 1;
            enrollments.Add(new Enrollment(enrollmentId, student.Username, courseId));

            Instructor courseInstructor = users.Find(u =>
                u is Instructor i && i.CourseIds.Contains(courseId)) as Instructor;
            if (courseInstructor != null)
                courseInstructor.NotifyEnrollment(student.Username, courseId);

            // Auto-save after enrollment
            SmartLearnManager.SaveAllData(users, courses);

            Pause("Press any key to continue...");
        }

        // ==================== MY ENROLLED COURSES ====================

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

            student.ShowEnrolledCourses(courses, enrollments);

            Console.WriteLine("  Total: " + student.EnrolledCourseIds.Count + " course(s)");
            Console.WriteLine("==============================");
            Pause("Press any key to go back...");
        }

        // ==================== UPDATE STUDENT PROGRESS ====================

        static void UpdateStudentProgress(Student student)
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("      Update My Progress     ");
            Console.WriteLine("  Student: " + student.Username);
            Console.WriteLine("==============================");

            if (student.EnrolledCourseIds.Count == 0)
            {
                Console.WriteLine("  You are not enrolled in any courses yet.");
                Pause("Press any key to go back..."); return;
            }

            student.ShowEnrolledCourses(courses, enrollments);

            Console.Write("\nEnter Course ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int courseId))
            {
                Console.WriteLine("  Invalid course ID.");
                Pause("Press any key to go back..."); return;
            }

            Console.Write("Enter progress percentage (0-100): ");
            if (!int.TryParse(Console.ReadLine(), out int progress))
            {
                Console.WriteLine("  Invalid input. Please enter a number.");
                Pause("Press any key to go back..."); return;
            }

            student.UpdateProgress(courseId, progress);

            Enrollment enrollment = enrollments.Find(e =>
                e.StudentUsername == student.Username &&
                e.CourseId == courseId);

            if (enrollment != null)
            {
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

            // Auto-save after progress update
            SmartLearnManager.SaveAllData(users, courses);

            Pause("Press any key to continue...");
        }

        // ==================== DROP STUDENT COURSE ====================

        static void DropStudentCourse(Student student)
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("         Drop Course         ");
            Console.WriteLine("  Student: " + student.Username);
            Console.WriteLine("==============================");

            if (student.EnrolledCourseIds.Count == 0)
            {
                Console.WriteLine("  You are not enrolled in any courses.");
                Pause("Press any key to go back..."); return;
            }

            student.ShowEnrolledCourses(courses, enrollments);

            Console.Write("\nEnter Course ID to drop: ");
            if (!int.TryParse(Console.ReadLine(), out int courseId))
            {
                Console.WriteLine("  Invalid course ID.");
                Pause("Press any key to go back..."); return;
            }

            Course course     = SmartLearnManager.GetCourseById(courseId)
                             ?? courses.Find(c => c.CourseId == courseId);
            string courseName = course != null ? course.CourseName : "Course ID " + courseId;

            student.DropCourse(courseId);

            if (course != null && course.CurrentEnrollments > 0)
                course.CurrentEnrollments--;

            Enrollment enrollment = enrollments.Find(e =>
                e.StudentUsername == student.Username &&
                e.CourseId == courseId);

            if (enrollment != null)
                enrollments.Remove(enrollment);

            Console.WriteLine("\n  ✔ Drop complete for '" + courseName + "'.");
            Pause("Press any key to continue...");
        }

        // ==================== SHOW STUDENT STATS ====================

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

            List<int> completed = student.GetCompletedCourses();

            if (completed.Count > 0)
            {
                Console.WriteLine("  Completed Courses:");
                foreach (int courseId in completed)
                {
                    Course course = SmartLearnManager.GetCourseById(courseId)
                                 ?? courses.Find(c => c.CourseId == courseId);
                    if (course != null)
                        Console.WriteLine("    ✔ " + course.CourseName);
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

        // ==================== INSTRUCTOR MENUS ====================

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

            instructor.ShowMyCourses(courses, enrollments);
            Pause("Press any key to go back...");
        }

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
                    Course course = SmartLearnManager.GetCourseById(enrollment.CourseId)
                                 ?? courses.Find(c => c.CourseId == enrollment.CourseId);
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

        static void AddInstructorCourse(Instructor instructor)
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("     Add Course to Teach     ");
            Console.WriteLine("  Instructor: " + instructor.Username);
            Console.WriteLine("==============================");

            Console.WriteLine("\n  Available Courses:");
            foreach (Course c in courses)
                Console.WriteLine($"    [{c.CourseId}] {c.CourseName} ({c.GetCourseType()})");
            Console.WriteLine();

            Console.Write("\nEnter Course ID to add: ");
            if (!int.TryParse(Console.ReadLine(), out int courseId))
            {
                Console.WriteLine("  Invalid course ID.");
                Pause("Press any key to go back..."); return;
            }

            Course course = SmartLearnManager.GetCourseById(courseId)
                         ?? courses.Find(c => c.CourseId == courseId);
            if (course == null)
            {
                Console.WriteLine("  Course not found.");
                Pause("Press any key to go back..."); return;
            }

            instructor.AddCourse(courseId);
            Pause("Press any key to continue...");
        }

        static void ShowInstructorStudentCount(Instructor instructor)
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("      My Student Count       ");
            Console.WriteLine("  Instructor: " + instructor.Username);
            Console.WriteLine("==============================");

            int count = instructor.GetStudentCount(enrollments);
            Console.WriteLine("  Total students in your courses: " + count);
            Console.WriteLine("==============================");
            Pause("Press any key to go back...");
        }

        // ==================== DISPLAY ALL COURSES ====================

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
                course.DisplayCourseInfo();   // polymorphic
                Console.WriteLine();
            }

            Pause("Press any key to go back...");
        }

        // ==================== ADMIN HELPER METHODS ====================

        static void DeactivateUserAsAdmin(Admin admin)
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("       Deactivate User       ");
            Console.WriteLine("  Admin: " + admin.Username);
            Console.WriteLine("==============================");

            admin.ViewAllUsers(users);

            Console.Write("\nEnter username to deactivate: ");
            string username = Console.ReadLine()?.Trim();

            User userToDeactivate = SmartLearnManager.GetUserByUsername(username)
                                 ?? users.Find(u => u.Username == username);

            if (userToDeactivate == null)
            {
                Console.WriteLine("  User not found.");
                Pause("Press any key to go back..."); return;
            }

            admin.DeactivateUser(userToDeactivate);
            Pause("Press any key to continue...");
        }

        // ==================== ANALYTICS & REPORTS SUB-MENU ====================
        // Called from both Admin dashboard (option 4) and Main Menu (option 6)

        static void ShowAnalyticsMenu()
        {
            int choice;
            do
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════╗");
                Console.WriteLine("║     Analytics & Reports        ║");
                Console.WriteLine("╚════════════════════════════════╝");
                Console.WriteLine("[1]  Search Courses by Keyword");
                Console.WriteLine("[2]  Search Students by Keyword");
                Console.WriteLine("[3]  Search Instructors by Keyword");
                Console.WriteLine("[4]  Filter Courses by Category");
                Console.WriteLine("[5]  Student Analytics");
                Console.WriteLine("[6]  Course Analytics");
                Console.WriteLine("[7]  System-wide Analytics");
                Console.WriteLine("[8]  Student Performance Levels");
                Console.WriteLine("[9]  Course Statistics");
                Console.WriteLine("[10] Paginated Course Browser");
                Console.WriteLine("[11]  Category Breakdown");
                Console.WriteLine("[12]  Back to Admin Menu");
                Console.WriteLine("================================");
                Console.Write("Enter choice: ");

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    ShowError("Invalid input."); Pause("Press any key..."); continue;
                }

                Console.Clear();

                switch (choice)
                {
                    // ── Search ──────────────────────────────────────────────────
                    case 1:
                        Console.Write("  Enter keyword to search courses: ");
                        string kw = Console.ReadLine()?.Trim();
                        SmartLearnManager.DisplaySearchResults(
                            SmartLearnManager.SearchCourses(courses, kw));
                        Pause("Press any key to go back...");
                        break;

                    case 2:
                        Console.Write("  Enter keyword to search students: ");
                        string sk = Console.ReadLine()?.Trim();
                        SmartLearnManager.DisplayStudentList(
                            SmartLearnManager.SearchStudents(users, sk));
                        Pause("Press any key to go back...");
                        break;

                    case 3:
                        Console.Write("  Enter keyword to search instructors: ");
                        string ik = Console.ReadLine()?.Trim();
                        var instList = SmartLearnManager.SearchInstructors(users, ik);
                        Console.WriteLine($"  Found {instList.Count} instructor(s):");
                        Console.WriteLine("==============================");
                        foreach (var inst in instList)
                            Console.WriteLine(
                                $"  • {inst.Username,-20} | {inst.Department,-20} | {inst.Email}");
                        Console.WriteLine("==============================");
                        Pause("Press any key to go back...");
                        break;

                    case 4:
                        Console.Write("  Enter category to filter: ");
                        string cat = Console.ReadLine()?.Trim();
                        SmartLearnManager.DisplaySearchResults(
                            SmartLearnManager.FilterCoursesByCategory(courses, cat));
                        Pause("Press any key to go back...");
                        break;

                    // ── Analytics ────────────────────────────────────────────────
                    case 5:
                        // Student Analytics — Top students, active students, avg progress
                        Console.WriteLine("╔════════════════════════════════╗");
                        Console.WriteLine("║       Student Analytics        ║");
                        Console.WriteLine("╚════════════════════════════════╝");
                        Console.WriteLine($"  System Avg Progress : {SmartLearnManager.CalculateSystemAverageProgress(users):0.00}%");
                        Console.WriteLine($"  Active Students     : {SmartLearnManager.GetActiveStudents(users).Count}");
                        Console.WriteLine($"  Any Course Completed: {(SmartLearnManager.HasStudentCompletedAny(users) ? "Yes" : "No")}");
                        Console.WriteLine("\n  ── Top 3 Students ──────────────");
                        SmartLearnManager.DisplayStudentList(SmartLearnManager.GetTopStudents(users, 3));
                        Pause("Press any key to go back...");
                        break;

                    case 6:
                        // Course Analytics — Popular, needing students, highest rated
                        Console.WriteLine("╔════════════════════════════════╗");
                        Console.WriteLine("║       Course Analytics         ║");
                        Console.WriteLine("╚════════════════════════════════╝");
                        Console.WriteLine("  ── Top 3 Popular Courses ───────");
                        SmartLearnManager.DisplaySearchResults(SmartLearnManager.GetPopularCourses(courses, 3));
                        Console.WriteLine("  ── Courses Needing Promotion ───");
                        SmartLearnManager.DisplaySearchResults(SmartLearnManager.GetCoursesNeedingStudents(courses));
                        Console.WriteLine($"  Most Enrolled Category : {SmartLearnManager.GetMostEnrolledCategory(courses)}");
                        Console.WriteLine($"  All Physical Seats Full: {(SmartLearnManager.AreAllCoursesFilled(courses) ? "Yes" : "No")}");
                        Pause("Press any key to go back...");
                        break;

                    case 7:
                        // System-wide Analytics dashboard
                        SmartLearnManager.DisplayAnalytics(users, courses, enrollments);
                        Pause("Press any key to go back...");
                        break;

                    case 8:
                        SmartLearnManager.GetStudentsByPerformanceLevel(users);
                        Pause("Press any key to go back...");
                        break;

                    case 9:
                        SmartLearnManager.GetCourseStatistics(courses);
                        Pause("Press any key to go back...");
                        break;

                    case 10:
                        Console.Write("  Enter page number: ");
                        if (int.TryParse(Console.ReadLine(), out int page))
                        {
                            var paged = SmartLearnManager.GetPaginatedCourses(courses, page, 3);
                            SmartLearnManager.DisplaySearchResults(paged);
                        }
                        else
                        {
                            ShowError("Invalid page number.");
                        }
                        Pause("Press any key to go back...");
                        break;

                    case 11:
                        SmartLearnManager.DisplayCategoryBreakdown(courses);
                        Pause("Press any key to go back...");
                        break;

                    case 12:
                        break;  // exit loop — back to caller (Admin dashboard or Main Menu)

                    default:
                        ShowError("Invalid choice. Enter 1-12.");
                        Pause("Press any key to try again...");
                        break;
                }

            } while (choice != 12);
        }

        // ==================== CREATE NEW COURSE (Instructor option 2) ====================

        // Instructor creates an Online, InPerson, or Hybrid course.
        // New course is added to the global list + dictionaries + auto-saved.
        static void CreateCourse(Instructor instructor)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║        Create New Course       ║");
            Console.WriteLine("╚════════════════════════════════╝");
            Console.WriteLine($"  Instructor: {instructor.Username}");
            Console.WriteLine();

            // ── Step 1: Course ID ────────────────────────────────────────────
            // Auto-suggest next available ID so it never clashes
            int suggestedId = courses.Count > 0
                ? courses.Max(c => c.CourseId) + 1
                : 101;

            Console.Write($"  Enter Course ID (suggested: {suggestedId}, or type your own): ");
            string idInput = Console.ReadLine()?.Trim();
            int courseId   = string.IsNullOrWhiteSpace(idInput) ? suggestedId
                           : int.TryParse(idInput, out int parsed) ? parsed : suggestedId;

            if (courses.Any(c => c.CourseId == courseId))
            {
                ShowError($"Course ID {courseId} already exists. Please choose a different ID.");
                Pause("Press any key to go back..."); return;
            }

            // ── Step 2: Course Name ──────────────────────────────────────────
            Console.Write("  Enter Course Name: ");
            string courseName = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(courseName))
            {
                ShowError("Course name cannot be empty.");
                Pause("Press any key to go back..."); return;
            }

            // ── Step 3: Description ──────────────────────────────────────────
            Console.Write("  Enter Description: ");
            string description = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(description)) description = "No description provided.";

            // ── Step 4: Category ─────────────────────────────────────────────
            Console.WriteLine("  Common categories: Programming, Web Development, Data Science,");
            Console.WriteLine("                     Computer Science, Database, Security, Mobile, Cloud");
            Console.Write("  Enter Category: ");
            string category = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(category)) category = "General";

            // ── Step 5: Course Type ──────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("  Select Course Type:");
            Console.WriteLine("  [1] Online    — unlimited seats, video-based");
            Console.WriteLine("  [2] In-Person — fixed seats, room-based");
            Console.WriteLine("  [3] Hybrid    — online videos + in-person sessions");
            Console.Write("  Enter type (1-3): ");

            if (!int.TryParse(Console.ReadLine(), out int typeChoice) || typeChoice < 1 || typeChoice > 3)
            {
                ShowError("Invalid type. Please enter 1, 2, or 3.");
                Pause("Press any key to go back..."); return;
            }

            Course newCourse = null;

            // ── Online ───────────────────────────────────────────────────────
            if (typeChoice == 1)
            {
                Console.Write("  Enter total video duration (minutes): ");
                if (!int.TryParse(Console.ReadLine(), out int duration) || duration <= 0)
                {
                    ShowError("Invalid duration.");
                    Pause("Press any key to go back..."); return;
                }

                newCourse = new OnlineCourse(
                    courseId, courseName, description,
                    instructor.Username, category,
                    duration);
            }

            // ── In-Person ────────────────────────────────────────────────────
            else if (typeChoice == 2)
            {
                Console.Write("  Enter max students (seats in room): ");
                if (!int.TryParse(Console.ReadLine(), out int maxSeats) || maxSeats <= 0)
                {
                    ShowError("Invalid seat count.");
                    Pause("Press any key to go back..."); return;
                }

                Console.Write("  Enter Room Number (e.g. B-101): ");
                string room = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(room)) room = "TBD";

                Console.Write("  Enter Building Name: ");
                string building = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(building)) building = "TBD";

                newCourse = new InPersonCourse(
                    courseId, courseName, description,
                    instructor.Username, category,
                    maxSeats, room, building);
            }

            // ── Hybrid ───────────────────────────────────────────────────────
            else if (typeChoice == 3)
            {
                Console.Write("  Enter max students (physical seats): ");
                if (!int.TryParse(Console.ReadLine(), out int maxSeats) || maxSeats <= 0)
                {
                    ShowError("Invalid seat count.");
                    Pause("Press any key to go back..."); return;
                }

                Console.Write("  Enter online video duration (minutes): ");
                if (!int.TryParse(Console.ReadLine(), out int videoDuration) || videoDuration <= 0)
                {
                    ShowError("Invalid video duration.");
                    Pause("Press any key to go back..."); return;
                }

                Console.Write("  Enter Room Number (e.g. C-201): ");
                string room = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(room)) room = "TBD";

                Console.Write("  Enter Building Name: ");
                string building = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(building)) building = "TBD";

                newCourse = new HybridCourse(
                    courseId, courseName, description,
                    instructor.Username, category,
                    maxSeats, videoDuration, room, building);
            }

            if (newCourse == null)
            {
                ShowError("Course creation failed.");
                Pause("Press any key to go back..."); return;
            }

            // ── Step 6: Add to system ────────────────────────────────────────
            courses.Add(newCourse);

            // Automatically assign this course to the creating instructor
            instructor.AddCourse(courseId);

            // Rebuild dictionaries so instant lookups include the new course
            SmartLearnManager.RebuildDictionaries(users, courses);

            // Auto-save — assignment requires persisting new course
            SmartLearnManager.SaveAllData(users, courses);

            // ── Confirmation ─────────────────────────────────────────────────
            Console.WriteLine();
            Console.WriteLine("  ✔ Course created successfully!");
            Console.WriteLine("  ------------------------------");
            newCourse.DisplayCourseInfo();

            // Notify the instructor via INotifiable
            ((INotifiable)instructor).SendNotification(
                $"You created a new {newCourse.GetCourseType()} course: '{courseName}' (ID {courseId}).");

            Pause("Press any key to go back...");
        }

                // ==================== RATE A COURSE — IRatable (Student option 5) ====================

        // Student picks an enrolled course and submits a star rating + review via IRatable
        static void RateCourse(Student student)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║         Rate a Course          ║");
            Console.WriteLine("╚════════════════════════════════╝");

            if (student.EnrolledCourseIds.Count == 0)
            {
                Console.WriteLine("  You are not enrolled in any courses yet.");
                Pause("Press any key to go back..."); return;
            }

            // Show enrolled courses so the student can pick one
            Console.WriteLine("  Your enrolled courses:");
            Console.WriteLine("  ------------------------------");
            foreach (int id in student.EnrolledCourseIds)
            {
                Course c = SmartLearnManager.GetCourseById(id) ?? courses.Find(x => x.CourseId == id);
                if (c != null)
                {
                    string ratingLine = c.GetTotalRatings() > 0
                        ? $"{c.GetAverageRating():0.0}★ ({c.GetTotalRatings()} reviews)"
                        : "No ratings yet";
                    Console.WriteLine($"  [{c.CourseId}] {c.CourseName}  |  {ratingLine}");
                }
            }
            Console.WriteLine("  ------------------------------");

            // Step 1: select course
            Console.Write("\n  Enter Course ID to rate (0 to cancel): ");
            if (!int.TryParse(Console.ReadLine(), out int courseId) || courseId == 0)
            {
                Pause("Press any key to go back..."); return;
            }

            if (!student.EnrolledCourseIds.Contains(courseId))
            {
                ShowError("You are not enrolled in Course ID " + courseId + ".");
                Pause("Press any key to go back..."); return;
            }

            // Use IRatable reference — polymorphic, works for any Course subtype
            IRatable ratableCourse = courses.Find(c => c.CourseId == courseId) as IRatable;
            Course selectedCourse  = courses.Find(c => c.CourseId == courseId);

            if (ratableCourse == null)
            {
                ShowError("Course not found.");
                Pause("Press any key to go back..."); return;
            }

            // Step 2: star rating (IRatable.AddRating validates 1-5 internally)
            Console.Write("  Enter star rating (1-5): ");
            if (!int.TryParse(Console.ReadLine(), out int stars))
            {
                ShowError("Invalid input. Please enter a number 1-5.");
                Pause("Press any key to go back..."); return;
            }

            // Step 3: review text
            Console.Write("  Enter your review: ");
            string review = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(review)) review = "No comment.";

            // Step 4: submit via IRatable interface — validation inside AddRating()
            ratableCourse.AddRating(stars, review);

            if (stars >= 1 && stars <= 5)
            {
                Console.WriteLine($"  New average : {ratableCourse.GetAverageRating():0.0}★ " +
                                  $"({ratableCourse.GetTotalRatings()} total reviews)");

                // Notify the student as confirmation via INotifiable
                ((INotifiable)student).SendNotification(
                    $"You rated '{selectedCourse?.CourseName ?? "Course " + courseId}' {stars}★. Thank you!");
            }

            Pause("Press any key to go back...");
        }

        // ==================== STUDENT NOTIFICATIONS — INotifiable (Student option 6) ====================

        static void ShowStudentNotifications(Student student)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║      My Notifications          ║");
            Console.WriteLine("╚════════════════════════════════╝");

            // INotifiable.GetNotificationHistory() returns the private list copy
            List<string> history = ((INotifiable)student).GetNotificationHistory();

            if (history.Count == 0)
            {
                Console.WriteLine("  No notifications yet.");
                Console.WriteLine("  Enroll in a course or receive a grade to get notifications.");
            }
            else
            {
                Console.WriteLine($"  {history.Count} notification(s):");
                Console.WriteLine("  ------------------------------");
                for (int i = 0; i < history.Count; i++)
                    Console.WriteLine($"  {i + 1,2}. {history[i]}");
            }

            Console.WriteLine("==============================");
            Pause("Press any key to go back...");
        }

        // ==================== GRADE ASSIGNMENTS — Instructor option 4 ====================

        // Instructor selects one of their courses, picks an enrolled student,
        // enters a grade (0-100) which updates Enrollment progress and notifies
        // the student via INotifiable.
        static void GradeAssignments()
        {
            Console.WriteLine("Feature Coming Soon!");
        }

        // ==================== INSTRUCTOR NOTIFICATIONS — INotifiable (Instructor option 5) ====================

        static void ShowInstructorNotifications(Instructor instructor)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║      My Notifications          ║");
            Console.WriteLine("╚════════════════════════════════╝");
            Console.WriteLine($"  Instructor: {instructor.Username}");

            // INotifiable.GetNotificationHistory() — returns private list copy
            List<string> history = ((INotifiable)instructor).GetNotificationHistory();

            if (history.Count == 0)
            {
                Console.WriteLine("  No notifications yet.");
                Console.WriteLine("  Notifications appear when students enroll or are graded.");
            }
            else
            {
                Console.WriteLine($"  {history.Count} notification(s):");
                Console.WriteLine("  ------------------------------");
                for (int i = 0; i < history.Count; i++)
                    Console.WriteLine($"  {i + 1,2}. {history[i]}");
            }

            Console.WriteLine("==============================");
            Pause("Press any key to go back...");
        }

                // ==================== UNIVERSAL SEARCH ====================

        static void UniversalSearch(List<User> users, List<Course> courses)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║       Universal Search         ║");
            Console.WriteLine("╚════════════════════════════════╝");

            List<ISearchable> searchItems = new List<ISearchable>();

            foreach (Course course in courses)
                searchItems.Add(course);

            foreach (User user in users)
            {
                if (user is ISearchable searchableUser)
                    searchItems.Add(searchableUser);
            }

            Console.Write("  Enter search keyword: ");
            string keyword = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                ShowError("Keyword cannot be empty.");
                Pause("Press any key to go back..."); return;
            }

            Console.WriteLine("\n  Results for: \"" + keyword + "\"");
            List<ISearchable> results = SearchEngine.Search(searchItems, keyword);
            SearchEngine.DisplayResults(results);

            Pause("Press any key to go back...");
        }

        // ==================== HELPERS ====================

        static string BuildProgressBar(int percentage)
        {
            int filled = percentage / 10;
            int empty  = 10 - filled;
            return "[" + new string('█', filled) + new string('░', empty) + "]";
        }

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
            // Save all data before exiting (Task 3 — wire up program flow)
            Console.WriteLine("\n  Saving data before exit...");
            SmartLearnManager.SaveAllData(users, courses);
            Console.WriteLine("\nGoodbye! Thank you for using SmartLearn.");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}