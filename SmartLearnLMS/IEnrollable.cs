public interface IEnrollable
{
    void Enroll(Student student); 
    void Drop(Student student); 
    bool CanEnroll(Student student); 
    int GetAvailableSeats();
}