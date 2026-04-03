using System;
using System.Linq;

public abstract class User
{
    // ==================== PROPERTIES ====================

    public string   Username       { get; set; }
    public string   Role           { get; set; }
    public bool     IsActive       { get; set; }
    public DateTime DateRegistered { get; set; }

    // ==================== EMAIL PROPERTY ====================
 
    private string _email;
    public string Email
    {
        get { return _email; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Console.WriteLine("  ✗ Invalid email");
                return;
            }
            if (!value.Contains("@"))
            {
                Console.WriteLine("  ✗ Invalid email - must contain @");
                return;
            }
            _email = value;
        }
    }

    // ==================== PASSWORD PROPERTY ====================
 
    private string _password;
    public string Password
    {
        get { return _password; }
        set
        {
            if (value.Length < 8)
            {
                Console.WriteLine("  ✗ Password must be at least 8 characters");
                return;
            }
            if (!value.Any(char.IsDigit))
            {
                Console.WriteLine("  ✗ Password must contain at least 1 number");
                return;
            }
            _password = value;
        }
    }

    // ==================== CONSTRUCTOR ====================

    protected User(string username, string password, string email, string role)
    {
        Username       = username;
        Password       = password;
        Email          = email;
        Role           = role;
        IsActive       = true;
        DateRegistered = DateTime.Now;
    }

    // ==================== METHODS ====================

    // FIX 1: marked virtual so Student, Instructor, Admin can override it
    public virtual void DisplayInfo()
    {
        Console.WriteLine("  Username        : " + Username);
        Console.WriteLine("  Email           : " + Email);
        Console.WriteLine("  Role            : " + Role);
        Console.WriteLine("  Active          : " + (IsActive ? "Yes" : "No"));
        Console.WriteLine("  Date Registered : " + DateRegistered.ToString("dd-MM-yyyy HH:mm"));
    }

    public bool ValidatePassword(string inputPassword)
    {
        return Password == inputPassword;
    }

    public void ChangePassword(string newPassword)
    {
        Password = newPassword;
        Console.WriteLine("  Password updated successfully.");
    }

    public void Deactivate()
    {
        IsActive = false;
        Console.WriteLine("  Account deactivated.");
    }

    public abstract void DisplayDashboard();
    public abstract string GetUserType();
}