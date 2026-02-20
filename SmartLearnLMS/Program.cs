using System;
using System.Collections.Generic;

namespace SmartLearn
{
    class Program
    {
        // ==================== COURSE DATA ====================

        static string[] courses = new string[]
        {
            "C# Programming Fundamentals",
            "Introduction to SQL Server",
            "Web Development with ASP.NET Core",
            "Advanced C# Techniques",
            "Database Design Principles",
            "RESTful API Development",
            "Entity Framework Core",
            "Front-End Development with React",
            "Cloud Computing with Azure",
            "Software Testing and Quality Assurance"
        };

        // ==================== THREE DICTIONARIES ====================

        // Each dictionary uses username as the key
        static Dictionary<string, string> userPasswords = new Dictionary<string, string>();
        static Dictionary<string, string> userEmails = new Dictionary<string, string>();
        static Dictionary<string, string> userRoles = new Dictionary<string, string>();

        // ==================== SESSION STATE ====================

        static bool isLoggedIn = false;
        static string currentUser = "";
        static string currentRole = "";

        // ==================== ENTRY POINT ====================

        static void Main(string[] args)
        {
            // Seed test accounts into all three dictionaries
            SeedUser("admin", "admin123", "admin@smartlearn.com", "Admin");
            SeedUser("student1", "pass123", "student@smartlearn.com", "Student");
            SeedUser("instructor1", "pass123", "instructor@smartlearn.com", "Instructor");

            // Main program loop: routes based on login state
            while (true)
            {
                if (!isLoggedIn)
                    ShowMainMenu();
                else
                    ShowDashboard();
            }
        }

        // Adds a user to all three dictionaries at once
        static void SeedUser(string username, string password, string email, string role)
        {
            userPasswords[username] = password;
            userEmails[username] = email;
            userRoles[username] = role;
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
            if (currentRole == "Student") ShowStudentDashboard();
            else if (currentRole == "Instructor") ShowInstructorDashboard();
            else if (currentRole == "Admin") ShowAdminDashboard();
        }

        // ==================== REGISTER USER ====================

        // Handles complete registration flow — collects and validates all fields
        static void RegisterUser()
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("       User Registration     ");
            Console.WriteLine("==============================");

            // Step 1: Collect and validate username
            Console.Write("  Enter Username: ");
            string username = Console.ReadLine()?.Trim();

            if (!ValidateUsername(username))
            {
                Pause("Press any key to go back..."); return;
            }

            // Step 2: Collect and validate password
            Console.Write("  Enter Password: ");
            string password = Console.ReadLine();

            if (!ValidatePassword(password))
            {
                Pause("Press any key to go back..."); return;
            }

            // Step 3: Confirm password matches
            Console.Write("  Confirm Password: ");
            string confirmPassword = Console.ReadLine();

            if (confirmPassword != password)
            {
                ShowError("Passwords do not match. Please try again.");
                Pause("Press any key to go back..."); return;
            }

            // Step 4: Collect and validate email
            Console.Write("  Enter Email: ");
            string email = Console.ReadLine()?.Trim();

            if (!ValidateEmail(email))
            {
                Pause("Press any key to go back..."); return;
            }

            // Step 5: Collect and validate role
            Console.WriteLine("  Select Role:  1. Student   2. Instructor   3. Admin");
            Console.Write("  Enter Role (1-3): ");
            string roleInput = Console.ReadLine()?.Trim();
            string role = GetRoleName(roleInput);

            if (role == null)
            {
                ShowError("Invalid role. Please enter 1, 2, or 3.");
                Pause("Press any key to go back..."); return;
            }

            // Step 6: Store into all three dictionaries
            userPasswords[username] = password;
            userEmails[username] = email;
            userRoles[username] = role;

            // Step 7: Show success confirmation
            Console.WriteLine("\n  ✔ Registration Successful!");
            Console.WriteLine("  ------------------------------");
            Console.WriteLine("  Username : " + username);
            Console.WriteLine("  Email    : " + email);
            Console.WriteLine("  Role     : " + role);
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

        // Validates username: not empty, length 3-20, no spaces, not already taken
        // Returns true if valid, false + error message if invalid
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
            // Check all three dictionaries — username already registered
            if (userPasswords.ContainsKey(username))
            {
                ShowError("Username '" + username + "' already exists. Please choose another."); return false;
            }
            return true;
        }

        // Validates password: not empty, minimum 6 characters, max 30 characters
        // Returns true if valid, false + error message if invalid
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

        // Validates email: not empty, must have @ and dot, must not already be registered
        // Returns true if valid, false + error message if invalid
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

            // Search all stored email values to detect duplicate registrations
            foreach (string existingEmail in userEmails.Values)
            {
                if (existingEmail.ToLower() == email.ToLower())
                {
                    ShowError("Email '" + email + "' is already registered."); return false;
                }
            }
            return true;
        }

        // ==================== LOGIN ====================

        // Handles complete login flow: checks username, password, sets session state
        static void Login()
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("            Login            ");
            Console.WriteLine("==============================");

            // Step 1: Get username and check if it exists in the dictionary
            Console.Write("  Username: ");
            string username = Console.ReadLine()?.Trim();

            if (!userPasswords.ContainsKey(username))
            {
                ShowError("Username '" + username + "' not found. Please register first.");
                Pause("Press any key to go back..."); return;
            }

            // Step 2: Get password and verify it matches stored password
            Console.Write("  Password: ");
            string password = Console.ReadLine();

            if (userPasswords[username] != password)
            {
                ShowError("Incorrect password. Please try again.");
                Pause("Press any key to go back..."); return;
            }

            // Step 3: Retrieve user's role from role dictionary
            string role = userRoles[username];

            // Step 4: Set session state
            isLoggedIn = true;
            currentUser = username;
            currentRole = role;

            // Step 5: Show success message
            Console.WriteLine("\n  ✔ Login successful! Welcome back, " + currentUser + ".");
            Console.WriteLine("  Role     : " + currentRole);
            Console.WriteLine("  Redirecting to your dashboard...");
            Pause("Press any key to continue...");
        }

        // ==================== LOGOUT ====================

        // Resets all session state and returns user to the login/register menu
        static void Logout()
        {
            string name = currentUser;

            // Reset session state
            isLoggedIn = false;
            currentUser = "";
            currentRole = "";

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

        // Displays all courses in a numbered list using a for loop
        static void DisplayAllCourses()
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("       Available Courses     ");
            Console.WriteLine("==============================");

            for (int i = 0; i < courses.Length; i++)
                Console.WriteLine("  " + (i + 1) + ". " + courses[i]);

            Console.WriteLine("==============================");
            Pause("Press any key to go back...");
        }

        // Handles search functionality with case-insensitive matching
        // Allows multiple searches using a loop — user types 'n' to stop
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
                    // Case-insensitive search using ToLower() + Contains()
                    Console.WriteLine("\n------------------------------");
                    Console.WriteLine("  Search Results for: \"" + keyword + "\"");
                    Console.WriteLine("------------------------------");

                    int matchCount = 0;

                    foreach (string course in courses)
                    {
                        if (course.ToLower().Contains(keyword.ToLower()))
                        {
                            matchCount++;
                            Console.WriteLine("  " + matchCount + ". " + course);
                        }
                    }

                    if (matchCount == 0)
                        Console.WriteLine("  No results found. Try a different keyword.");
                    else
                        Console.WriteLine("\n  " + matchCount + " course(s) found.");

                    Console.WriteLine("------------------------------");
                }

                // Ask user if they want to search again
                Console.Write("\nSearch again? (y/n): ");
                string again = Console.ReadLine();
                if (again != null) again = again.Trim().ToLower();
                searchAgain = (again == "y") ? 'y' : 'n';

            } while (searchAgain == 'y');
        }

        // ==================== DASHBOARDS ====================

        // Student dashboard loop — runs until student chooses Logout
        static void ShowStudentDashboard()
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("     Student Dashboard       ");
            Console.WriteLine("  Hello, " + currentUser + "!");
            Console.WriteLine("==============================");
            Console.WriteLine("1. Browse Courses");
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

            if (choice == 1) BrowseCourses();
            else if (choice == 5) Logout();
            else ShowComingSoon();
        }

        // Instructor dashboard loop — runs until instructor chooses Logout
        static void ShowInstructorDashboard()
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("    Instructor Dashboard     ");
            Console.WriteLine("  Hello, " + currentUser + "!");
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

        // Admin dashboard loop — runs until admin chooses Logout
        static void ShowAdminDashboard()
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.WriteLine("       Admin Dashboard       ");
            Console.WriteLine("  Hello, " + currentUser + "!");
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

        // Standard coming-soon message for all unbuilt features
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