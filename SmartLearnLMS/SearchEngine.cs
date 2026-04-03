using System;
using System.Collections.Generic;

public static class SearchEngine
{
    // ==================== SEARCH ====================

    // Loops through all items, returns only those whose MatchesSearch() returns true
    public static List<ISearchable> Search(List<ISearchable> items, string keyword)
    {
        List<ISearchable> results = new List<ISearchable>();

        foreach (ISearchable item in items)
        {
            if (item.MatchesSearch(keyword))
                results.Add(item);
        }

        return results;
    }

    // ==================== DISPLAY RESULTS ====================

    // Shows count header and one summary line per result, or "No results found" if empty
    public static void DisplayResults(List<ISearchable> results)
    {
        if (results.Count == 0)
        {
            Console.WriteLine("  No results found.");
            return;
        }

        Console.WriteLine("==============================");
        Console.WriteLine("  Search Results: " + results.Count + " found");
        Console.WriteLine("==============================");

        foreach (ISearchable item in results)
        {
            Console.WriteLine("  • " + item.GetSearchSummary());
        }

        Console.WriteLine("==============================");
    }
}