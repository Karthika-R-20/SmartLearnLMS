public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime DateRegistered { get; set; }
    public User(string username, string password, string email, string role)
    {
        Username = username;
        Password = password;
        Email = email;
        Role = role;
        IsActive = true;
        DateRegistered = DateTime.Now;
    }
    public void DisplayInfo()
    {
        Console.WriteLine($"Username: {Username}");
        Console.WriteLine($"Email: {Email}");
        Console.WriteLine($"Role: {Role}");
        Console.WriteLine($"IsActive: {IsActive}");
        Console.WriteLine($"Date Registered: {DateRegistered:dd-MM-yyyy HH:mm}");
    }
    public bool ValidatePassword(string inputPassword)
    {
        return Password == inputPassword;
    }
    public void ChangePassword(string newPassword)
    {
        Password = newPassword;
        Console.WriteLine("Password updated successfully.");
    }
    public void Deactivate()
    {
        IsActive = false;
        Console.WriteLine("Account deactivated.");
    }
}