public interface IRatable
{
    void AddRating(int stars, string review); 
    double GetAverageRating(); 
    int GetTotalRatings(); 
}