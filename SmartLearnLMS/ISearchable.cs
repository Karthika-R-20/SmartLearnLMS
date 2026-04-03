public interface ISearchable
{
    bool MatchesSearch(string keyword);
    string GetSearchSummary(); 
}