public interface INotifiable
{
    void SendNotification(string message); 
    List<string> GetNotificationHistory(); 
}